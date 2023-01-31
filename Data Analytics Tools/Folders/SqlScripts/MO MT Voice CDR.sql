SELECT   
	
mdv. [MoDatasourceId]
,mdv.[MtDatasourceId]
,mdv.[MoCallIndex]
,mdv.[MtCallIndex]
,mdv.[StartDateTime]
,mdv.[MoSimOperator] AS SimOperator


	,vR.Campaign
	,DATENAME(DD, ss.EndTime) AS 'Day' 
	,DATENAME(mm, ss.EndTime) AS 'Month' 
	,vR.Region
	,vR.Geographic
	,vR.RouteRegion
	,vR.Area
	,vR.Route
	,vR.NUCS


,mdv.[MoAqmSessionEndAqmCallQuality] AS MoAqmCallQuality
,mdv.[MtAqmSessionEndAqmCallQuality]  AS MtAqmCallQuality

,ssv.AqmSessionEndAqmCallQuality AS AQMCallQuality
,mdv.[AqmScore]
,mdv.[MoAqmScore]
,mdv.[MtAqmScore]
,mdv.[MoAqmDownlinkScores]
,mdv.[MtAqmDownlinkScores]
,mdv.[CallNonSustainability]
,mdv.[CallNonSustainabilityNb]
,mdv.[CallNonSustainabilityWb]
,mdv.[CallNonSustainabilitySamples]
,SessionStatisticsEvents.EpsPuschBlockErrorRateSessionStatisticsAverage AS [BLER]
,mdv.[AqmScoreNb]
,mdv.[NumberOfNbSamples]
,mdv.[NumberOfSamples]
,mdv.[LowAqmScores]
,mdv.[MoNumberOfSamples]
,mdv.[MoAqmScoreCountUnder1p3]
,mdv.[MoAqmScoreCountUnder2]
,mdv.[MtNumberOfSamples]
,mdv.[MtAqmScoreCountUnder1p3]
,mdv.[MtAqmScoreCountUnder2]
,mdv.[AvgMoAudioPacketLossRate]
,mdv.[AvgMtAudioPacketLossRate]
,mdv.[MoRtpLostPacketsRateAudioCallAverage]
,mdv.[MtRtpLostPacketsRateAudioCallAverage]
,mdv.[MoCallAttempt]
,mdv.[MoCallInitiation]
,mdv.[MoCallBlocked]
,mdv.[MtCallAttempt]
,mdv.[MtCallInitiation]
,mdv.[MtCallBlocked]   
,mdv.[CallDropped]
,mdv.[MoRadioTechnologySequence]
,mdv.[MtRadioTechnologySequence]
,mdv.[MoLogfileName]
,mdv.[MtLogfileName]
,mdv.[MoEUArfcn]
,mdv.[MtEUArfcn]
,mdv.[MoLacTac]
,mdv.[MtLacTac]
,mdv.[MoRnceNbCellId]
,mdv.[MtRnceNbCellId]
,mdv.[MoCellIdentity]
,mdv.[MoRnceNbCellId28Bits]
,mdv.[MoFrequencyBand]
,mdv.[MoBandwidth]
,mdv.[MtCellIdentity]
,mdv.[MtRnceNbCellId28Bits]
,mdv.[MtFrequencyBand]
,mdv.[MtBandwidth]
,mdv.[MoLteRsrp]
,mdv.[MoLteRsrq]
,mdv.[MoLteRssi]
,mdv.[MoLteSignalToNoiseRatio]
,mdv.[MtLteRsrp]
,mdv.[MtLteRsrq]
,mdv.[MtLteRssi]
,mdv.[MtLteSignalToNoiseRatio]
,mdv.[TestStatusMO]
,mdv.[TestStatusMT]
,mdv.[ErrorCause]
,mdv.[ErrorCauseDetails]   
,mdv.[MoIMSI]
,mdv.[MtIMSI]
,mdv.[MoIpAddress]
,mdv.[MtIpAddress]
,mdv.[MoAPN]
,mdv.[MtAPN]
,mdv.[MoLatitude]
,mdv.[MoLongitude]
,mdv.[MtLatitude]
,mdv.[MtLongitude]
,mdv.[MoCallSetupTime]
,mdv.[MtCallSetupTime]
,mdv.[MoCallSetupTimeConnect]
,mdv.[MtCallSetupTimeConnect]
,mdv.[MoToMtCallSetupTimeConnect]
,mdv.[MoAudioAverageJitter]
,mdv.[MtAudioAverageJitter]
,mdv.[Test] AS [Test sequence] 
,mdv.[UsedPcscfAddress]
,mdv.[MoDialStartPhoneNumber]
,mdv.[MoCallSetupLatestRNC]
,mdv.[MoCallSetupLatestCarrier]
,mdv.[MoCallSetupLatestCell]
,mdv.[MoCallSetupLatestLAC]
,mdv.[MoCallSetupLatestRsrp]
,mdv.[MoCallSetupLatestRssi]
,mdv.[MoCallSetupLatestSinr]
,mdv.[MoCallSetupLatestSC]
,mdv.[MoCallSetupLatestRSCP]
,mdv.[MoCallSetupLatestEcNo]
,mdv.[MoCallEndLatestRNC]
,mdv.[MoCallEndLatestCarrier]
,mdv.[MoCallEndLatestCell]
,mdv.[MoCallEndLatestLAC]
,mdv.[MoCallEndLatestRsrp]
,mdv.[MoCallEndLatestRssi]
,mdv.[MoCallEndLatestSinr]
,mdv.[MoCallEndLatestSC]
,mdv.[MoCallEndLatestRSCP]
,mdv.[MoCallEndLatestEcNo]
,mdv.[MtCallSetupLatestRNC]
,mdv.[MtCallSetupLatestCarrier]
,mdv.[MtCallSetupLatestCell]
,mdv.[MtCallSetupLatestLAC]
,mdv.[MtCallSetupLatestRsrp]
,mdv.[MtCallSetupLatestRssi]
,mdv.[MtCallSetupLatestSinr]
,mdv.[MtCallSetupLatestSC]
,mdv.[MtCallSetupLatestRSCP]
,mdv.[MtCallSetupLatestEcNo]
,mdv.[MtCallEndLatestRNC]
,mdv.[MtCallEndLatestCarrier]
,mdv.[MtCallEndLatestCell]
,mdv.[MtCallEndLatestLAC]
,mdv.[MtCallEndLatestRsrp]
,mdv.[MtCallEndLatestRssi]
,mdv.[MtCallEndLatestSinr]
,mdv.[MtCallEndLatestSC]
,mdv.[MtCallEndLatestRSCP]
,mdv.[MtCallEndLatestEcNo]
,mdv.[MoSpeechCodecs]
,mdv.[MtSpeechCodecs]
,mdv.[MoVoiceCallBearer]
,mdv.[MtVoiceCallBearer]
,mdv.[MoCallSetupLatestNrCarrier]
,mdv.[MoCallSetupLatestNrPci]
,mdv.[MoCallSetupLatestSsRsrp]
,mdv.[MoCallSetupLatestSsRsrq]
,mdv.[MoCallSetupLatestSsSinr]
,mdv.[MtCallSetupLatestNrCarrier]
,mdv.[MtCallSetupLatestNrPci]
,mdv.[MtCallSetupLatestSsRsrp]
,mdv.[MtCallSetupLatestSsRsrq]
,mdv.[MtCallSetupLatestSsSinr]
,mdv.[MoCallEndLatestNrCarrier]
,mdv.[MoCallEndLatestNrPci]
,mdv.[MoCallEndLatestSsRsrp]
,mdv.[MoCallEndLatestSsRsrq]
,mdv.[MoCallEndLatestSsSinr]
,mdv.[MtCallEndLatestNrCarrier]
,mdv.[MtCallEndLatestNrPci]
,mdv.[MtCallEndLatestSsRsrp]
,mdv.[MtCallEndLatestSsRsrq]
,mdv.[MtCallEndLatestSsSinr]


FROM cdr.SessionSummaryVoice ssv
LEFT OUTER JOIN (
	SELECT ROW_NUMBER() OVER (
			PARTITION BY p.DatasourceId
			,p.SessionIdOrCallIndex ORDER BY p.DATETIME
			) AS rn
		,p.*
	FROM cdr.SessionNetwork p
	) sn ON ssv.DatasourceId = sn.DatasourceId


	AND ssv.CallIndex = sn.SessionIdOrCallIndex
	AND rn = 1
LEFT OUTER JOIN cdr.SessionSummary ss ON ss.DatasourceId = ssv.DatasourceId
	AND ss.SessionIdOrCallIndex = ssv.CallIndex
LEFT OUTER JOIN dbo.handover ho ON ho.DatasourceId = ss.DatasourceId
	AND ho.CurrentCallIndex = ss.SessionIdOrCallIndex
	AND (
		(ho.InterSystemHandoverToWcdma IS NOT NULL)
		OR (ho.InterSystemHandoverToWcdmaFailure IS NOT NULL)
		)
LEFT OUTER JOIN dbo.VoiceEvents ve ON ve.DatasourceId = ss.DatasourceId
	AND ve.CallEndCallIndex = ss.SessionIdOrCallIndex

LEFT OUTER JOIN SessionStatisticsEvents  ON SessionStatisticsEvents.DatasourceId = ss.DatasourceId 
	AND SessionStatisticsEvents.CurrentCallIndex = ss.SessionIdOrCallIndex

LEFT OUTER JOIN Logfile AS Logfile ON ss.LogfileId = Logfile.Id 		
LEFT OUTER JOIN [cdr].[vRegion] vR ON vR.id = Logfile.Id
 
LEFT OUTER JOIN [md].[Voice] mdv ON (
		(mdv.MoDatasourceId = ss.DatasourceId)
		AND (mdv.MoCallIndex = ss.SessionIdOrCallIndex)
		)
	OR (
		(mdv.MtDataSourceId = ss.DatasourceId)
		AND (mdv.MtCallIndex = ss.SessionIdOrCallIndex)
		)

WHERE 
		
		(Logfile.LogfileProcessingStateId <> 3) and  mdv.[MoDatasourceId] is not null --and ss.LogfileName like '%Adhoc%'or ss.LogfileName like '%SGS_Johannesburg%'
			--and	ss.LogfileName like '%gOTT%'
		--and mdv. [MoDatasourceId] in (14959)
		--and vR.Region IN ('SGS','SGC')