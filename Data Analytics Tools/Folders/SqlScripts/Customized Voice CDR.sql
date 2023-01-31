SELECT  ss.DatasourceId
	,ss.SessionIdOrCallIndex AS CallIndex
	,ss.EndTime AS SessionEndTime
		,vR.Campaign
	,vR.Region
	,vR.Geographic
	,vR.RouteRegion
	,vR.Area
	,vR.Route

	
	,CASE 
		WHEN ssv.DialStartPhoneNumber IS NOT NULL
			THEN 1
		ELSE 0
		END AS DialEvent
	,ssv.DialStartPhoneNumber
	,CASE 
		WHEN ssv.AnswerStartSampleId IS NOT NULL
			THEN 1
		ELSE 0
		END AS AnswerEvent
	,CASE 
		WHEN ssv.CallAttemptSampleId IS NOT NULL
			THEN 1
		ELSE 0
		END AS CallAttempt
	,CASE 
		WHEN ssv.CallAttemptCsfbSampleId IS NOT NULL
			THEN 1
		ELSE 0
		END AS CallAttemptCsfb
	,CASE 
		WHEN ssv.CallSetupSampleId IS NOT NULL
			THEN 1
		ELSE 0
		END AS CallSetup
	,CASE 
		WHEN ssv.CallBlockedSampleId IS NOT NULL
			THEN 1
		ELSE 0
		END AS CallBlocked
	,ssv.CallSetupUserPerceivedTime AS CallSetupTimeUEs
	,ssv.MtCallSetupTime AS CallSetupTimeCGs
	,CASE 
		WHEN ssv.CallEndSampleId IS NOT NULL
			THEN 1
		ELSE 0
		END AS CallEnd
	,CASE 
		WHEN ssv.CallDroppedSampleId IS NOT NULL
			THEN 1
		ELSE 0
		END AS CallDropped
	,ssv.CallEndCallDuration AS CallDuration
	,coalesce(ssv.AqmAlgorithmDownlink, ssv.AqmAlgorithmUplink) AS AQMAlgorithm
	,ssv.AqmSessionEndAqmCallQuality AS AQMCallQuality
	,ssv.AqmSessionEndAqmCallQualityDownlink AS AQMCallQualityDL
	,ssv.AqmSessionEndAqmCallQualityUplink AS AQMCallQualityUL
	,ssv.AqmCallType AS AQMCallType
	,ssv.CallDirection
	,CASE 
		WHEN ssv.CallEndCause = 'User busy'
			THEN 'Yes'
		ELSE 'No'
		END AS UserBusy
	,CASE 
		WHEN ve.CallEndCallEstablished = 0
			THEN 'True'
		ELSE 'False'
		END AS CallNotEstablished
	,ssv.IncompleteCall AS IncompleteCall
	,ssv.CallBlockedType AS BlockedCallCause
	,ssv.CallDroppedDropType AS DroppedCallCause
	,ssvM2M.CallNonSustainability AS CallNonSustain
	,ssv.RtpJitterAudioAverageJitter AS AverageRtpJitter
	,CASE 
		WHEN (ho.InterSystemHandoverToWcdma IS NOT NULL)
			AND (ho.InterSystemHandoverToWcdmaHandoverType = 2)
			THEN 1
		ELSE NULL
		END AS SRVCCSuccess
	,CASE 
		WHEN (ho.InterSystemHandoverToWcdmaFailure IS NOT NULL)
			AND (ho.InterSystemHandoverToWcdmaFailureHandoverType = 2)
			THEN 1
		ELSE NULL
		END AS SRVCCFailure
	,CASE 
		WHEN ho.InterSystemHandoverToWcdmaHandoverType = 2
			THEN ho.InterSystemHandoverToWcdmaDuration
		ELSE NULL
		END AS SRVCCDuration
	,ssv.SpeechCodecs AS SpeechCodec
	,ss.LogfileName AS LogName
	,ss.EndLongitude AS Longitude
	,ss.EndLatitude AS Latitude
	,ss.RadioTechnologySequence AS ServiceBearer
	,ss.IMSI
	,ss.IMEI
	,sn.Mcc AS ServingMCC
	,sn.Mnc AS ServingMNC
	,ssvM2M.MoCallEndLatestCarrier
	,ssvM2M.MoCallEndLatestRNC
	,ssvM2M.MoCallEndLatestCell AS MoPCi
	,ssvM2M.MoCallEndLatestLAC
	,ssvM2M.MoCallEndLatestSC
	,ssvM2M.MoCallEndLatestRsrp
	,ssvM2M.MoCallEndLatestRssi
	,ssvM2M.MoCallEndLatestSinr
	,ssvM2M.MoCallEndLatestEcNo
	,ssvM2M.MoCallEndLatestRSCP
	,ssvM2M.MtCallEndLatestCarrier
	,ssvM2M.MtCallEndLatestRNC
	,ssvM2M.MtCallEndLatestCell AS MtPCi
	,ssvM2M.MtCallEndLatestLAC
	,ssvM2M.MtCallEndLatestSC
	,ssvM2M.MtCallEndLatestRsrp
	,ssvM2M.MtCallEndLatestRssi
	,ssvM2M.MtCallEndLatestSinr
	,ssvM2M.MtCallEndLatestEcNo
	,ssvM2M.MtCallEndLatestRSCP

	
	,sn.Bandwidth
	,sn.FrequencyBand
	,sn.LteRsrp
	,sn.LteRsrq
	,sn.LteRssi

	,sn.EUArfcn
	,sn.NrArfcn
	,sn.[NrRsrp]
    ,sn.[NrRsrq]
    ,sn.[NrSignalToNoiseRatio] AS NrSINR

	,sn.RnceNbCellId
	,sn.Pci
	,sn.WcdmaCpichEcNo
	,sn.WcdmaCpichRscp
	,sn.WcdmaScramblingCode

	,vR.Car
	,vR.NUCS
	,left(ss.IMSI,5) as Opto
	,vR.Log_Network
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

LEFT OUTER JOIN Logfile AS Logfile ON ss.LogfileId = Logfile.Id 		
LEFT OUTER JOIN [cdr].[vRegion] vR ON vR.id = Logfile.Id 
LEFT OUTER JOIN cdr.SessionSummaryVoiceM2M ssvM2M ON (
		(ssvM2M.MoDatasourceId = ss.DatasourceId)
		AND (ssvM2M.MoCallIndex = ss.SessionIdOrCallIndex)
		)
	OR (
		(ssvM2M.MtDataSourceId = ss.DatasourceId)
		AND (ssvM2M.MtCallIndex = ss.SessionIdOrCallIndex)
		)

WHERE (
		
	 (Logfile.LogfileProcessingStateId <> 3)--and ss.LogfileName like '%Adhoc%'or ss.LogfileName like '%SGS_Johannesburg%'
		)
