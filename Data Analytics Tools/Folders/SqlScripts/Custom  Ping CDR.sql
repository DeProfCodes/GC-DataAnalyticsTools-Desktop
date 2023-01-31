SELECT ss.DatasourceId
	,ss.SessionIdOrCallIndex AS SessionId
	,ss.EndSampleId AS SampleId
	,ss.EndTime AS SessionEndTime
	,vR.Campaign
	,DATENAME(DD, ss.EndTime) AS 'Day' 
	,DATENAME(mm, ss.EndTime) AS 'Month' 

	,ss.SimOperator
	,ss.IMEI
	,ss.IMSI
	,sn.Mcc AS MCC
	,sn.Mnc AS MNC
	
	,ss.EndLongitude AS Longitude
	,ss.EndLatitude AS Latitude
	,ss.RadioTechnologySequence AS RadioTechnologySequence
	,ssp.PingEndDataRadioBearer AS DataRadioBearer

	,ssp.PingEndFailedCount AS PingFailure
	,ssp.PingEndSuccessCount AS PingSuccess
	,ssp.PingEndAverageDelay AS RoundTripTimeAvgMs
	 ,cast(datediff(Millisecond,'00:00:00.000',[PingEndMedianDelay]) as float)/1000 as [PingEndMedianDelay]
      --,[PingEndMedianDelay]
	  ,cast(datediff(Millisecond,'00:00:00.000',[PingEndAverageDelay]) as float)/1000 as [PingEndAverageDelay]
      --,[PingEndAverageDelay]
	  ,cast(datediff(Millisecond,'00:00:00.000',[PingEndMinDelay]) as float)/1000 as [PingEndMinDelay]
      --,[PingEndMinDelay]
	  ,cast(datediff(Millisecond,'00:00:00.000',[PingEndMaxDelay]) as float)/1000 as [PingEndMaxDelay]
	,ssp.PingRoundtripTimeList
	,ssp.PingSize AS PingSizeB
	,ssp.PingStartAddress AS TestNodeAddress
	,ss.IpAddress
	,ssp.PingStartIPAddress AS TestNodeIPAdress
	,ssp.PingErrorCauseDetails AS ErrorCause
	,ss.SessionEndStatus AS ServiceStatus
	
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
	,ss.LogfileName AS LogName

FROM cdr.SessionSummaryPing ssp
LEFT OUTER JOIN (
	SELECT ROW_NUMBER() OVER (
			PARTITION BY p.DatasourceId
			,p.SessionIdOrCallIndex ORDER BY p.DATETIME
			) AS rn
		,p.*
	FROM cdr.SessionNetwork p
	) sn ON ssp.DatasourceId = sn.DatasourceId
	AND ssp.SessionId = sn.SessionIdOrCallIndex
	AND rn = 1
LEFT OUTER JOIN cdr.SessionSummary ss ON ss.DatasourceId = ssp.DatasourceId
	AND ss.SessionIdOrCallIndex = ssp.SessionId
LEFT OUTER JOIN Logfile AS Logfile ON ss.LogfileId = Logfile.Id 
LEFT OUTER JOIN [cdr].[vRegion] vR ON vR.id = Logfile.Id 

WHERE (
		
		 (Logfile.LogfileProcessingStateId <> 3)and (ssp.PingEndServiceStatus not like '%Invalid%')--and ss.LogfileName like '%Adhoc%'or ss.LogfileName like '%SGS_Johannesburg%' --and ss.LogfileName not like '%Adhoc%'  --and ss.LogfileName like '%MPU_Metro_NLP%'
		--and ss.DatasourceId like '4__31%'
		 --AND (ssp.PingErrorCauseDetails like '%SERVICE_BUSY%')
		 --OR (ssp.PingErrorCauseDetails like '%Equipment Disconnecting%')


	
		)


--AND (ss.SessionEndStatus like '%Service Busy%' and ss.LogfileName like '%NUC3%')