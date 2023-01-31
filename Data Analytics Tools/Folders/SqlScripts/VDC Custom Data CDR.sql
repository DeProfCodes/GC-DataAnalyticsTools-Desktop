SELECT top 600000 ss.DatasourceId
	,ss.SessionIdOrCallIndex AS SessionId
	,ss.EndSampleId AS SampleId
	,ss.EndTime AS SessionEndTime
	,vR.Campaign
	,DATENAME(DD, ss.EndTime) AS 'Day' 
	,DATENAME(mm, ss.EndTime) AS 'Month' 

	,vR.Region
	,vR.Geographic
	,vR.RouteRegion
	,vR.Area
	,vR.Route	

	,left(ss.IMSI,5) as Operator
	,ss.IMSI
	,ss.IMEI
	,sn.Mcc AS ServingMCC
	,sn.Mnc AS ServingMNC
	,ss.EndLongitude AS Longitude
	,ss.EndLatitude AS Latitude
	,ss.LogfileName AS LogName
	,ss.SerialNumber AS SerialNumber
	
	,ss.EndRadioTechnology AS ServiceBearer
	,ss.RadioTechnologySequence
	,ssd.EndDataRadioBearer

	,ssd.MaxBandwithUsed
	,ssd.MaxEpsServingCellCount
						
	,CC.[Carrier Configured] 
	,CC.[CA Combination]	
	,sn.NrArfcn
    ,CC.[PCC EUARFCN],
						CC.[SCC1 EUARFCN], 
						CC.[SCC2 EUARFCN],
						CC.[SCC3 EUARFCN], 
                        CC.[SCC4 EUARFCN],
						
						CC.[Total Bandwidth Usage (MHz)], 
						CC.[PCC Bandwidth],  
						CC.[SCC1 Bandwidth],
						CC.[SCC2 Bandwidth], 
						CC.[SCC3 Bandwidth], 
						CC.[SCC4 Bandwidth], 
						
						CC.[PCC Phy Throughput Carrier], 
						CC.[SCC1 Phy Throughput Carrier], 
						CC.[SCC2 Phy Throughput Carrier], 
						CC.[SCC3 Phy Throughput Carrier], 
						CC.[SCC4 Phy Throughput Carrier],
						
						CC.[PCC PRB Carrier],
						CC.[SCC1 PRB Carrier],
						CC.[SCC2 PRB Carrier],
						CC.[SCC3 PRB Carrier],
						CC.[SCC4 PRB Carrier],
						
						CC.[PCC BLER Carrier],
						CC.[PCC CQI0 Carrier],
						CC.[PCC RSRP Carrier],
						CC.[PCC SINR Carrier],
						CC.[PCC RSSI Carrier]

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

	,ssd.ServerIpAddress

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
	,ssd.EndServiceStatus AS ServiceStatus
	,ssd.ErrorErrorCauseDetails AS ErrorCause
	,CASE 
		WHEN ssd.EndServiceStatus LIKE '%Stopped by User%'
			THEN 1
		ELSE 0
		END AS StoppedByUser

	,ssd.ServiceAccessStartNrPci AS NrPCI
	,ssd.DNSResolutionStartNrPci AS [NrPCI DNSResolution]
	,ssd.AverageThroughputNrKbps
	,ssd.KbyteCountNr
	,ssd.TimeSpentOnNr
	
	,sn.[NrRsrp]
    ,sn.[NrRsrq]
    ,sn.[NrSignalToNoiseRatio]
    ,sn.[CellId28]
    ,sn.[MimoConfig]
    ,sn.[MimoConfigCount]

	,       ssd.IPServiceSetupTimeMethodADateTime AS '[Service Access  Duration [s]',
			ssd.ServiceAccessStartRnceNodeB AS [Service Access RNC/eNodeB Start],
			ssd.ServiceAccessStartSectorCell AS [Service Access Cell ID/Sector Start],
			ssd.ServiceAccessStartPciSC AS [Service Access SC/PCI Start],
			ssd.ServiceAccessStartRssi AS 'Service Access RSSI Start (dBm)',
			ssd.ServiceAccessStartRsrp AS 'Service Access RSRP Start (dBm)',
			ssd.ServiceAccessStartSinr AS 'Service Access SINR Start (dB)',
			ssd.ServiceAccessStartRscp AS 'Service Access RSCP Start (dBm)',
			ssd.ServiceAccessStartEcNo AS 'Service Access Ec/No Start (dBm)',
			ssd.ServiceAccessStartLacTac AS 'Service Access LAC/TAC Start',	
			
			ssd.SessionEndRnceNodeB AS [Session RNC/eNodeB End],
			ssd.SessionEndSectorCell AS [Session Cell ID/Sector End],
			ssd.SessionEndNrPci AS [Session SC/PCI End],

			sn.[LteRssi] AS [Session RSSI End (dBm)],
			sn.LteRsrp AS [Session RSRP End (dBm)],
			sn.LteRsrq		AS [Session RSRQ End (dBm)],
			sn.LteSignalToNoiseRatio AS [Session SINR End (dB)],
			
			ssd.SessionEndRscp AS [Session RSCP End (dBm)],
			ssd.SessionEndEcNo AS [Session Ec/No End (dBm)] ,
			ssd.SessionEndLacTac AS [Session LAC/TAC End]
    
	,vr.[Floors level]
	,vR.[WalkTest Area]
	,vR.Car
	,vR.NUCS
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



LEFT OUTER JOIN [dbo].[Component_Carrier] CC ON CC.DatasourceId = ssd.DatasourceId
	AND CC.SessionId = ssd.SessionId

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
WHERE 
		
	 (Logfile.LogfileProcessingStateId <> 3)  and (ssd.EndServiceStatus not LIKE '%Invalid%')

except

SELECT ss.DatasourceId
	,ss.SessionIdOrCallIndex AS SessionId
	,ss.EndSampleId AS SampleId
	,ss.EndTime AS SessionEndTime
	,vR.Campaign
	,DATENAME(DD, ss.EndTime) AS 'Day' 
	,DATENAME(mm, ss.EndTime) AS 'Month' 

	,vR.Region
	,vR.Geographic
	,vR.RouteRegion
	,vR.Area
	,vR.Route	

	,left(ss.IMSI,5) as Operator
	,ss.IMSI
	,ss.IMEI
	,sn.Mcc AS ServingMCC
	,sn.Mnc AS ServingMNC
	,ss.EndLongitude AS Longitude
	,ss.EndLatitude AS Latitude
	,ss.LogfileName AS LogName
	,ss.SerialNumber AS SerialNumber
	
	,ss.EndRadioTechnology AS ServiceBearer
	,ss.RadioTechnologySequence
	,ssd.EndDataRadioBearer

	,ssd.MaxBandwithUsed
	,ssd.MaxEpsServingCellCount
						
	,CC.[Carrier Configured] 
	,CC.[CA Combination]	
	,sn.NrArfcn
    ,CC.[PCC EUARFCN],
						CC.[SCC1 EUARFCN], 
						CC.[SCC2 EUARFCN],
						CC.[SCC3 EUARFCN], 
                        CC.[SCC4 EUARFCN],
						
						CC.[Total Bandwidth Usage (MHz)], 
						CC.[PCC Bandwidth],  
						CC.[SCC1 Bandwidth],
						CC.[SCC2 Bandwidth], 
						CC.[SCC3 Bandwidth], 
						CC.[SCC4 Bandwidth], 
						
						CC.[PCC Phy Throughput Carrier], 
						CC.[SCC1 Phy Throughput Carrier], 
						CC.[SCC2 Phy Throughput Carrier], 
						CC.[SCC3 Phy Throughput Carrier], 
						CC.[SCC4 Phy Throughput Carrier],
						
						CC.[PCC PRB Carrier],
						CC.[SCC1 PRB Carrier],
						CC.[SCC2 PRB Carrier],
						CC.[SCC3 PRB Carrier],
						CC.[SCC4 PRB Carrier],
						
						CC.[PCC BLER Carrier],
						CC.[PCC CQI0 Carrier],
						CC.[PCC RSRP Carrier],
						CC.[PCC SINR Carrier],
						CC.[PCC RSSI Carrier]

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

	,ssd.ServerIpAddress

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
	,ssd.EndServiceStatus AS ServiceStatus
	,ssd.ErrorErrorCauseDetails AS ErrorCause
	,CASE 
		WHEN ssd.EndServiceStatus LIKE '%Stopped by User%'
			THEN 1
		ELSE 0
		END AS StoppedByUser

	,ssd.ServiceAccessStartNrPci AS NrPCI
	,ssd.DNSResolutionStartNrPci AS [NrPCI DNSResolution]
	,ssd.AverageThroughputNrKbps
	,ssd.KbyteCountNr
	,ssd.TimeSpentOnNr
	
	,sn.[NrRsrp]
    ,sn.[NrRsrq]
    ,sn.[NrSignalToNoiseRatio] AS NrSINR
    ,sn.[CellId28]
    ,sn.[MimoConfig]
    ,sn.[MimoConfigCount]

	,       ssd.IPServiceSetupTimeMethodADateTime AS '[Service Access  Duration [s]',
			ssd.ServiceAccessStartRnceNodeB AS [Service Access RNC/eNodeB Start],
			ssd.ServiceAccessStartSectorCell AS [Service Access Cell ID/Sector Start],
			ssd.ServiceAccessStartPciSC AS [Service Access SC/PCI Start],
			ssd.ServiceAccessStartRssi AS 'Service Access RSSI Start (dBm)',
			ssd.ServiceAccessStartRsrp AS 'Service Access RSRP Start (dBm)',
			ssd.ServiceAccessStartSinr AS 'Service Access SINR Start (dB)',
			ssd.ServiceAccessStartRscp AS 'Service Access RSCP Start (dBm)',
			ssd.ServiceAccessStartEcNo AS 'Service Access Ec/No Start (dBm)',
			ssd.ServiceAccessStartLacTac AS 'Service Access LAC/TAC Start',	
			
			ssd.SessionEndRnceNodeB AS [Session RNC/eNodeB End],
			ssd.SessionEndSectorCell AS [Session Cell ID/Sector End],
			ssd.SessionEndNrPci AS [Session SC/PCI End],

			sn.[LteRssi] AS [Session RSSI End (dBm)],
			sn.LteRsrp AS [Session RSRP End (dBm)],
			sn.LteRsrq		AS [Session RSRQ End (dBm)],
			sn.LteSignalToNoiseRatio AS [Session SINR End (dB)],
			
			ssd.SessionEndRscp AS [Session RSCP End (dBm)],
			ssd.SessionEndEcNo AS [Session Ec/No End (dBm)] ,
			ssd.SessionEndLacTac AS [Session LAC/TAC End]
	,vr.[Floors level]
	,vR.[WalkTest Area]
	,vR.Car
	,vR.NUCS
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



LEFT OUTER JOIN [dbo].[Component_Carrier] CC ON CC.DatasourceId = ssd.DatasourceId
	AND CC.SessionId = ssd.SessionId

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
WHERE 
		
	 (Logfile.LogfileProcessingStateId <> 3) and (ssd.EndServiceStatus not LIKE '%Invalid%')


and((CC.[PCC EUARFCN] = CC.[SCC1 EUARFCN])
OR ( CC.[PCC EUARFCN] = CC.[SCC2 EUARFCN])
OR ( CC.[PCC EUARFCN] = CC.[SCC3 EUARFCN])
OR ( CC.[PCC EUARFCN] = CC.[SCC4 EUARFCN])
OR ( CC.[SCC1 EUARFCN] =CC.[SCC2 EUARFCN])
OR ( CC.[SCC1 EUARFCN] =CC.[SCC3 EUARFCN])
OR ( CC.[SCC1 EUARFCN] =CC.[SCC4 EUARFCN])
OR ( CC.[SCC2 EUARFCN] =CC.[SCC3 EUARFCN])
OR ( CC.[SCC2 EUARFCN] =CC.[SCC4 EUARFCN])
OR ( CC.[SCC3 EUARFCN] =CC.[SCC4 EUARFCN]))	
 

 -- and vR.Area like '%Adhoc%'or vR.Area like '%SGS_Johannesburg%'