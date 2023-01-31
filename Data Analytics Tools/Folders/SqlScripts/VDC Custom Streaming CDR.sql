 select 
 ss.DatasourceId  
,ss.SessionIdOrCallIndex as SessionId     
,ss.EndSampleId as SampleId     
,vR.Campaign
,DATENAME(DD, ss.EndTime) AS 'Day' 
,DATENAME(mm, ss.EndTime) AS 'Month'  
,ss.SimOperator
,ss.IMSI     
,ss.IMEI     
,sn.Mcc as MCC     
,sn.Mnc as MNC     
,ss.StartTime as SessionStartTime     
,ss.EndTime as SessionEndTime   

,ss.RadioTechnologySequence AS RadioTechnologySequence
,sss.EndDataRadioBearer

	,CC.[Total Bandwidth Usage (MHz)]
	,sn.FrequencyBand
	,CC.[Carrier Configured] 
		
	,sn.NrArfcn
	
	,CC.[CA Combination]
    ,CC.[PCC EUARFCN],
	CC.[SCC1 EUARFCN], 
	CC.[SCC2 EUARFCN],
	CC.[SCC3 EUARFCN], 
    CC.[SCC4 EUARFCN],
	
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
,ss.EndLatitude as Latitude 
,ss.EndLongitude as Longitude         
,CASE When sss.StreamingVideoPlayStartFailure is not null then 1 else 0 end as VideoPlayStartFailure     
,sss.VideoPlayStartTime as VideoPlayStartTime     
,CASE When sss.StreamingVideoSessionFailure is not null then 1 else 0 end as VideoSessionFailure    
,sss.VideoSessionTime as VideoSessionTime     
,CASE When sss.ServiceNotAccessible  is not null then 1 else 0 end as VideoServiceAccessFailure     
,sss.ServiceAccessTime as VideoServiceAccessTime     
,CASE When sss.ReproductionStartFailure  is not null then 1 else 0 end as ReproductionStartFailure     
,sss.ReproductionStartDelay as ReproductionStartDelay     
,CASE When sss.EndVideoSessionInterruptionNumberOf > 0 then 'No' else 'Yes' end as VideoSessionImpairmentFree    
,sss.EndVideoSessionInterruptionNumberOf as StreamingEndVideoSessionInterruptionNumberOf     
,sss.EndVideoSessionInterruptionDurationSum as StreamingEndVideoSessionInterruptionDurationSum    
,sss.EndVideoSessionInterruptionDurationMax as StreamingEndVideoSessionInterruptionDurationMax     
,dc.StreamingEndAverageSessionResolution as StreamingEndAverageSessionResolution    

,case when [StreamingEndAverageSessionResolution]  >= 719 then 'Yes'
 else 'No' end as StreamingEndHdResolution   	

,sss.StreamingEndAverageThroughputKbps as StreamingEndAverageThroughputKbps     
,dc.StreamingEndVideoSize as StreamingEndVideoSize     
,dc.StreamingEndVideoBufferSize as StreamingEndVideoBufferSize     
,dc.StreamingEndClipLength as StreamingEndClipLength    
,dci.StreamingEndClientIpAddressesIPAddress as StreamingEndClientIp     
,lkp.Value as StreamingEndClientType   

,CASE WHEN (dc.StreamingEndQualifiedImpairmentFree=1) THEN 'Yes' ELSE 'No' END as StreamingEndQualifiedImpairmentFree     
,CASE When ss.SessionEndStatus like '%Stopped by User%' then 'Yes' else 'No' end as StoppedByUser    
,sss.Url as TestNode    
,case
when sss.Url like '%https://www.youtube.com/watch?v=9Auq9mYxFEE%' THEN 'LiveHD'
when sss.Url like '%https://www.youtube.c/watch?v=-upyPouRrB8%' THEN  'LiveHD'
when sss.Url like '%https://www.youtube.com/watch?v=-upyPouRrB8%' THEN  'LiveHD'

WHEN sss.Url like '%https://www.youtube.com/watch?v=uyPPAZUq66I%'  then 'Static'
WHEN sss.Url like '%https://www.youtube.com/watch?v=QoHDdunfcQ8%'  then 'Static'

WHEN sss.Url like '%https://www.youtube.com/watch?v=oDruklAJFmA%'  then '4K'
WHEN sss.Url like '%https://youtu.be/LXb3EKWsInQ%'  then '4K'

else 'No URL'
end AS [YouTube Type] 

,sss.EndStatus as ServiceStatus      
,sss.ErrorCauseDetails as ErrorCause  
,sss.ErrorCauseIdentity
,[Logfile].[SerialNumber] as SerialNumber  

    ,sn.[MimoConfig]
    ,sn.[MimoConfigCount]
  
	,sn.RnceNbCellId
	,sn.CellIdentity
	,sn.PCi
	,sn.LacTac
	
	,sn.[NrRsrp]
    ,sn.[NrRsrq]
    ,sn.[NrSignalToNoiseRatio] AS NrSINR
    ,sn.[CellId28]
    

	
	,sn.LteRsrp AS RSRP
	,sn.LteRsrq AS RSRQ
	,sn.LteRssi AS RSSI
	,sn.LteSignalToNoiseRatio SINR
	,sn.WcdmaCpichEcNo AS EcNo
	,sn.WcdmaCpichRscp AS RSCP
	,sn.WcdmaScramblingCode AS ScramblingCode

    ,vR.Region
	,vR.Geographic
	,vR.RouteRegion
	,vR.Area
	,vR.Route
	,vR.Car
	,vR.NUCS
	,left(ss.IMSI,5) as Operator
	,ss.LogfileName as LogName 
	,vR.Log_Network
 



From cdr.SessionSummaryStreaming sss     
left outer join (SELECT ROW_NUMBER() OVER (PARTITION BY p.DatasourceId, p.SessionIdOrCallIndex ORDER BY p.DateTime) as rn, p.* 
FROM cdr.SessionNetwork p) 
sn on sss.DatasourceId = sn.DatasourceId and sss.SessionId = sn.SessionIdOrCallIndex and rn = 1     
left outer join cdr.SessionSummary ss on ss.DatasourceId = sss.DatasourceId and ss.SessionIdOrCallIndex = sss.SessionId    
left outer join dbo.DataCompletion dc on dc.DatasourceId = sss.DatasourceId and dc.SessionId = sss.SessionId and dc.StreamingEnd is not null     
left outer join dbo.DataCompletionIndex dci on dci.DatasourceId = dc.DatasourceId and dci.sampleId = dc.SampleId and [dci].[Index] = 0 

LEFT OUTER JOIN [dbo].[Component_Carrier] CC ON CC.DatasourceId = sss.DatasourceId
	AND CC.SessionId = sss.SessionId

LEFT OUTER JOIN Logfile AS Logfile ON ss.LogfileId = Logfile.Id 
LEFT OUTER JOIN [cdr].[vRegion] vR ON vR.id = Logfile.Id 
INNER JOIN [dbo].[LookupStreamingClientType] lkp on lkp.Id=isnull (dc.StreamingEndClientType, 0)     

WHERE 		
		 (Logfile.LogfileProcessingStateId <> 3)


except

 select 
 ss.DatasourceId  
,ss.SessionIdOrCallIndex as SessionId     
,ss.EndSampleId as SampleId     
,vR.Campaign
,DATENAME(DD, ss.EndTime) AS 'Day' 
,DATENAME(mm, ss.EndTime) AS 'Month'  
,ss.SimOperator
,ss.IMSI     
,ss.IMEI     
,sn.Mcc as MCC     
,sn.Mnc as MNC     
,ss.StartTime as SessionStartTime     
,ss.EndTime as SessionEndTime   

,ss.RadioTechnologySequence AS RadioTechnologySequence
,sss.EndDataRadioBearer

	,CC.[Total Bandwidth Usage (MHz)]
	,sn.FrequencyBand
	,CC.[Carrier Configured] 
		
	,sn.NrArfcn
	
	,CC.[CA Combination]
    ,CC.[PCC EUARFCN],
	CC.[SCC1 EUARFCN], 
	CC.[SCC2 EUARFCN],
	CC.[SCC3 EUARFCN], 
    CC.[SCC4 EUARFCN],
	
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
,ss.EndLatitude as Latitude 
,ss.EndLongitude as Longitude         
,CASE When sss.StreamingVideoPlayStartFailure is not null then 1 else 0 end as VideoPlayStartFailure     
,sss.VideoPlayStartTime as VideoPlayStartTime     
,CASE When sss.StreamingVideoSessionFailure is not null then 1 else 0 end as VideoSessionFailure    
,sss.VideoSessionTime as VideoSessionTime     
,CASE When sss.ServiceNotAccessible  is not null then 1 else 0 end as VideoServiceAccessFailure     
,sss.ServiceAccessTime as VideoServiceAccessTime     
,CASE When sss.ReproductionStartFailure  is not null then 1 else 0 end as ReproductionStartFailure     
,sss.ReproductionStartDelay as ReproductionStartDelay     
,CASE When sss.EndVideoSessionInterruptionNumberOf > 0 then 'No' else 'Yes' end as VideoSessionImpairmentFree    
,sss.EndVideoSessionInterruptionNumberOf as StreamingEndVideoSessionInterruptionNumberOf     
,sss.EndVideoSessionInterruptionDurationSum as StreamingEndVideoSessionInterruptionDurationSum    
,sss.EndVideoSessionInterruptionDurationMax as StreamingEndVideoSessionInterruptionDurationMax     
,dc.StreamingEndAverageSessionResolution as StreamingEndAverageSessionResolution    

,case when [StreamingEndAverageSessionResolution]  >= 719 then 'Yes'
 else 'No' end as StreamingEndHdResolution   	

,sss.StreamingEndAverageThroughputKbps as StreamingEndAverageThroughputKbps     
,dc.StreamingEndVideoSize as StreamingEndVideoSize     
,dc.StreamingEndVideoBufferSize as StreamingEndVideoBufferSize     
,dc.StreamingEndClipLength as StreamingEndClipLength    
,dci.StreamingEndClientIpAddressesIPAddress as StreamingEndClientIp     
,lkp.Value as StreamingEndClientType   

,CASE WHEN (dc.StreamingEndQualifiedImpairmentFree=1) THEN 'Yes' ELSE 'No' END as StreamingEndQualifiedImpairmentFree     
,CASE When ss.SessionEndStatus like '%Stopped by User%' then 'Yes' else 'No' end as StoppedByUser    
,sss.Url as TestNode   
,case
when sss.Url like '%https://www.youtube.com/watch?v=9Auq9mYxFEE%' THEN 'LiveHD'
when sss.Url like '%https://www.youtube.c/watch?v=-upyPouRrB8%' THEN  'LiveHD'
when sss.Url like '%https://www.youtube.com/watch?v=-upyPouRrB8%' THEN  'LiveHD'

WHEN sss.Url like '%https://www.youtube.com/watch?v=uyPPAZUq66I%'  then 'Static'
WHEN sss.Url like '%https://www.youtube.com/watch?v=QoHDdunfcQ8%'  then 'Static'

WHEN sss.Url like '%https://www.youtube.com/watch?v=oDruklAJFmA%'  then '4K'
WHEN sss.Url like '%https://youtu.be/LXb3EKWsInQ%'  then '4K'

else 'No URL'
end AS [YouTube Type] 

,sss.EndStatus as ServiceStatus      
,sss.ErrorCauseDetails as ErrorCause  
,sss.ErrorCauseIdentity
,[Logfile].[SerialNumber] as SerialNumber  

    ,sn.[MimoConfig]
    ,sn.[MimoConfigCount]
  
	,sn.RnceNbCellId
	,sn.CellIdentity
	,sn.PCi
	,sn.LacTac
	
	,sn.[NrRsrp]
    ,sn.[NrRsrq]
    ,sn.[NrSignalToNoiseRatio] AS NrSINR
    ,sn.[CellId28]
    

	
	,sn.LteRsrp AS RSRP
	,sn.LteRsrq AS RSRQ
	,sn.LteRssi AS RSSI
	,sn.LteSignalToNoiseRatio SINR
	,sn.WcdmaCpichEcNo AS EcNo
	,sn.WcdmaCpichRscp AS RSCP
	,sn.WcdmaScramblingCode AS ScramblingCode

    ,vR.Region
	,vR.Geographic
	,vR.RouteRegion
	,vR.Area
	,vR.Route
	,vR.Car
	,vR.NUCS
	,left(ss.IMSI,5) as Operator
	,ss.LogfileName as LogName 
	,vR.Log_Network
 



From cdr.SessionSummaryStreaming sss     
left outer join (SELECT ROW_NUMBER() OVER (PARTITION BY p.DatasourceId, p.SessionIdOrCallIndex ORDER BY p.DateTime) as rn, p.* 
FROM cdr.SessionNetwork p) 
sn on sss.DatasourceId = sn.DatasourceId and sss.SessionId = sn.SessionIdOrCallIndex and rn = 1     
left outer join cdr.SessionSummary ss on ss.DatasourceId = sss.DatasourceId and ss.SessionIdOrCallIndex = sss.SessionId    
left outer join dbo.DataCompletion dc on dc.DatasourceId = sss.DatasourceId and dc.SessionId = sss.SessionId and dc.StreamingEnd is not null     
left outer join dbo.DataCompletionIndex dci on dci.DatasourceId = dc.DatasourceId and dci.sampleId = dc.SampleId and [dci].[Index] = 0 

LEFT OUTER JOIN [dbo].[Component_Carrier] CC ON CC.DatasourceId = sss.DatasourceId
	AND CC.SessionId = sss.SessionId

LEFT OUTER JOIN Logfile AS Logfile ON ss.LogfileId = Logfile.Id 
LEFT OUTER JOIN [cdr].[vRegion] vR ON vR.id = Logfile.Id 
INNER JOIN [dbo].[LookupStreamingClientType] lkp on lkp.Id=isnull (dc.StreamingEndClientType, 0)     

WHERE 		
		 (Logfile.LogfileProcessingStateId <> 3)


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
and sss.EndStatus not like '%Invalid%'
--and vR.Area like '%Adhoc%'or vR.Area like '%SGS_Johannesburg%'