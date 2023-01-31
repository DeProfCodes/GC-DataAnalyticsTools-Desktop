SELECT ss.DatasourceId
	,ss.SessionIdOrCallIndex AS SessionId
	,ss.EndSampleId AS SampleId
	,ss.EndTime AS SessionEndTime
	,vR.Campaign
	,DATENAME(DD, ss.EndTime) AS 'Day' 
	,DATENAME(mm, ss.EndTime) AS 'Month' 
	,ss.IMSI
	,ss.IMEI
	,sn.Mcc AS ServingMCC
	,sn.Mnc AS ServingMNC
	,ss.EndLongitude AS Longitude
	,ss.EndLatitude AS Latitude
	,ss.LogfileName AS LogName
	,ss.SerialNumber AS SerialNumber
	
	,ss.RadioTechnologySequence
	,ssd.EndDataRadioBearer
	,ss.SessionType AS Service
	,COALESCE(nc.EutranAttachFailure, nc.PsAttachFailure) AS AttachFailure
	,COALESCE(nc.EutranAttachFailureAttachTime, nc.PsAttachFailureDuration) AS AttachFailureDuration
	,CASE 
		WHEN (nc.EutranAttachComplete IS NOT NULL)
			OR (nc.PsAttach IS NOT NULL)
			THEN nc.DATETIME
		ELSE NULL
		END AS AttachSetupTime
	,COALESCE(nc.EutranAttachCompleteAttachTime, nc.PsAttachAttachCompletionTime) AS AttachDuration
	,nc.DnsHostNameResolutionFailure AS DNSHostNameResolutionFailure
	,nc.DnsHostNameResolutionFailureDnsServerAddress AS FailureDNSServer
	,nc.DnsHostNameResolutionTime AS DNSHostNameResolutionSucess
	,nc.DnsHostNameResolutionTimeDnsServerAddress AS DNSHostNameSuccessServerAddress
	,ssd.DnsHostNameResolutionTimeResolutionTime AS DNSHostNameResolutionDuration
	,ssd.UriType AS Download_UploadType
	,CASE 
		WHEN (ssd.IsTimeBasedMeasurement = 1)
			THEN 'Yes'
		ELSE 'No'
		END AS TimeBasedMeasurement
	,ssd.Url AS TestNode
	,CASE 
		WHEN ssd.IPServiceAccessFailureMethodASampleId IS NOT NULL
			THEN 1
		ELSE 0
		END AS IPserviceAccessFailure
	,ssd.IPServiceSetupTimeMethodAServiceSetupTime AS IPServiceSetupTimeSec
	,CASE 
		WHEN ssd.DataTransferCutoffMethodASampleId IS NOT NULL
			THEN 1
		ELSE 0
		END AS DataTransferCutoff
	,ssd.DataTransferTimeMethodADuration AS DataTransferTimeSec
	,CASE 
		WHEN ssd.UriType LIKE '%Web%'
			THEN ssd.DataTransferTimeMethodADuration
		ELSE NULL
		END AS WebBrowsingSessionTime
	,ssd.MeanDataRateMethodAThroughputKbps AS MeanUserDataRateKbps
	,ssd.EndFileSize AS FileSizekB
	,ssd.MaxEpsServingCellCount AS [Compoment Carrier]
	,ssd.EndServiceStatus AS ServiceStatus
	,ssd.ErrorErrorCauseDetails AS ErrorCause
	,CASE 
		WHEN ssd.EndServiceStatus LIKE '%Stopped by User%'
			THEN 1
		ELSE 0
		END AS StoppedByUser

		,sn.Bandwidth
	,sn.FrequencyBand
	,sn.RnceNbCellId
	,sn.CellIdentity
	,sn.PCi
	,sn.LacTac
	
	
	,sn.LteRsrp AS RSRP
	,sn.LteRsrq AS RSRQ
	,sn.LteRssi AS RSSI
	,sn.LteSignalToNoiseRatio SINR
	,sn.WcdmaCpichEcNo AS EcNo
	,sn.WcdmaCpichRscp AS RSCP
	,sn.WcdmaScramblingCode AS ScramblingCode

	,ssd.ServiceAccessStartNrPci AS NrPCI
	,ssd.DNSResolutionStartNrPci AS [NrPCI DNSResolution]
	,ssd.AverageThroughputNrKbps
	,ssd.KbyteCountNr
	,ssd.TimeSpentOnNr

	,sn.EUArfcn
	,sn.NrArfcn
	,sn.[NrRsrp]
    ,sn.[NrRsrq]
    ,sn.[NrSignalToNoiseRatio] AS NrSINR
   
	,sn.[MimoConfig]
    ,sn.[MimoConfigCount]
	,sn.[CellId28]

	,vR.Region
	,vR.Geographic
	,vR.RouteRegion
	,vR.Area
	,vR.Route
	
	,vR.Car
	,vR.NUCS
	,left(ss.IMSI,5) as Operator
	,vR.Log_Network


FROM cdr.SessionSummaryData ssd
LEFT OUTER JOIN (
	SELECT ROW_NUMBER() OVER (
			PARTITION BY p.DatasourceId
			,p.SessionIdOrCallIndex ORDER BY p.DATETIME
			) AS rn
		,p.*
	FROM cdr.SessionNetwork p
	) sn ON ssd.DatasourceId = sn.DatasourceId
	AND ssd.SessionId = sn.SessionIdOrCallIndex
	AND rn = 1
LEFT OUTER JOIN cdr.SessionSummary ss ON ss.DatasourceId = ssd.DatasourceId
	AND ss.SessionIdOrCallIndex = ssd.SessionId

LEFT OUTER JOIN Logfile AS Logfile ON ss.LogfileId = Logfile.Id 

LEFT OUTER JOIN [cdr].[vRegion] vR ON vR.id = Logfile.Id 

LEFT OUTER JOIN (
	SELECT ROW_NUMBER() OVER (
			PARTITION BY p.DatasourceId
			,p.SessionId ORDER BY p.DATETIME
			) AS rown
		,p.*
	FROM dbo.NetworkConnectivity p
	) nc ON ssd.DatasourceId = nc.DatasourceId
	AND ssd.SessionId = nc.SessionId
	AND rown = 1
WHERE (
		
	 (Logfile.LogfileProcessingStateId <> 3) and (ssd.EndServiceStatus not like '%Invalid%')
	  --and ss.LogfileName like '%KZN%'OR ss.LogfileName like '%WES%' 
	-- and ss.LogfileName not like '%Adhoc%'
	 --148777
	 --and ss.LogfileName  like '%Car7_Nuc4_VDC_4G_R1_0503103244.trp%'  --and ss.LogfileName like '%MPU_Metro_NLP%'
	  --AND (ssd.ErrorErrorCauseDetails like '%SERVICE_BUSY%')
	 
	 
	 -- and ((left(ss.IMSI,5) =65501 and vR.Log_Network !='VDC')
		--OR (left(ss.IMSI,5) =65502 and vR.Log_Network !='Telkom')
		--OR (left(ss.IMSI,5) =65507 and vR.Log_Network !='Cell C')
		--OR (left(ss.IMSI,5) =65538 and vR.Log_Network !='Rain')
		--OR (left(ss.IMSI,5) =65510 and vR.Log_Network !='MTN'))
		)

		