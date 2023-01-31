select distinct top 600000
				vtm.DatasourceId,
				vtm.SampleId, 
				vtm.CurrentCallIndex,
				ssvM2M.MoDataSourceId,
				ssvM2M.MtDataSourceId,
				vR.Campaign, 
                vR.Region, 
				vR.Geographic, 
				vR.Area, 
				vR.Route 
				,[SS].[SimOperator] AS Operator
				,vtm.DateTime 

                ,vtm.PolqaSwbScoreDownlinkAny	
				,ssvM2M.[MoAqmScore] as 'Mo POLQA Recorded'
				,ssvM2M.[MtAqmScore] as 'Mt POLQA Recorded'
				,ssvM2M.[AqmScore] as 'POLQA Value Overall'
				,ssv.AqmSessionEndAqmCallQualityDownlink as AqmCallQuality
				
				,[ssv].[CallDirection]
				,[ssv].[SpeechCodecs] AS [SpeechCodecs]

				,[ssv].[CallDroppedDropType] as 'CallDropped Type'
				,[ssv].[CallEndType] as 'CallEndType'
				,CASE 
					WHEN [ssv].CallDroppedSampleId  IS NOT NULL
					THEN 1
					ELSE 0
					END AS CallDropped
				,CASE 
					WHEN [ssv].CallAttemptSampleId IS NOT NULL
					THEN 1
					ELSE 0
					END AS CallAttempt
				,[ssvM2M].[MtCallSetupTime] as 'CallSetupTime'
				,CASE 
					WHEN [ssv].[CallBlockedSampleId] IS NOT NULL
					THEN 1
					ELSE 0
					END AS CallBlocked
				,CASE 
					WHEN [ssv].[CallSetupSampleId] IS NOT NULL
					THEN 1
					ELSE 0
					END AS CallSetup
				,[ssvM2M].[CallNonSustainabilitySamples]
				,[ssvM2M].[CallNonSustainability]
				
				,[ssvM2M].[MoCallSetupLatestCarrier] as 'Mo Start carrier'
				,[ssvM2M].[MtCallSetupLatestCarrier] as 'Mt Start carrier'

				,[ss].StartRadioTechnology AS 'Radio Technology'
				,[ss].RadioTechnologySequence
				,[ssvM2M].[MoStartRadioAccessTechnology] as 'Mo Start RadioTechnology'
				,[ssvM2M].[MoEndRadioAccessTechnology] as 'Mo End RadioTechnology'
				,[ssvM2M].[MtStartRadioAccessTechnology] as 'Mt Start RadioTechnology'
				,[ssvM2M].[MtEndRadioAccessTechnology] as 'Mt End RadioTechnology'

				,[ssvM2M].[MoCallSetupLatestRNC]	as 'Mo Start eNodeB/RNC'
				,[ssvM2M].[MoCallSetupLatestCell]	as 'Mo Start Sector/Cell Id'
				,[ssvM2M].[MoCallSetupLatestRSCP]	as 'Mo Start RSCP'
				,[ssvM2M].[MoCallSetupLatestEcNo]	as 'Mo Start Ec/No'
				,[ssvM2M].[MoCallSetupLatestRsrp]	as 'Mo Start RSRP'
				,[ssvM2M].[MoCallSetupLatestSinr]	as 'Mo Start SINR'
				,[ssvM2M].[MoCallSetupLatestLAC]	as 'Mo Start TAC/LAC'
				,[ssvM2M].[MoCallEndLatestRsrp] as 'Mo CallEndLatestRsrp'
				,[ssvM2M].[MoCallEndLatestSinr] as 'Mo CallEndLatestSinr'
				,[ssvM2M].[MoCallEndLatestRSCP] as 'Mo CallEndLatestRSCP'
				,[ssvM2M].[MoCallEndLatestEcNo] as 'Mo CallEndLatestEcNo'
				,[ssvM2M].[MoCallEndLatestRNC]  as 'Mo CallEndLatestRNC'
				,[ssvM2M].[MoCallEndLatestCell] as 'Mo CallEndLatestCell'

				,[ssvM2M].[MtCallSetupLatestRNC]	as 'Mt Start eNodeB/RNC'
				,[ssvM2M].[MtCallSetupLatestCell]	as 'Mt Start Sector/Cell Id'
				,[ssvM2M].[MtCallSetupLatestRSCP]	as 'Mt Start RSCP'
				,[ssvM2M].[MtCallSetupLatestEcNo]	as 'Mt Start Ec/No'
				,[ssvM2M].[MtCallSetupLatestRsrp]	as 'Mt Start RSRP'
				,[ssvM2M].[MtCallSetupLatestSinr]	as 'Mt Start SINR'
				,[ssvM2M].[MtCallSetupLatestLAC]	as 'Mt Start TAC/LAC'
				,[ssvM2M].[MtCallEndLatestRsrp] as 'Mt CallEndLatestRsrp'
				,[ssvM2M].[MtCallEndLatestSinr] as 'Mt CallEndLatestSinr'
				,[ssvM2M].[MtCallEndLatestRSCP] as 'Mt CallEndLatestRSCP'
				,[ssvM2M].[MtCallEndLatestEcNo] as 'Mt CallEndLatestEcNo'
				,[ssvM2M].[MtCallEndLatestRNC]  as 'Mt CallEndLatestRNC'
				,[ssvM2M].[MtCallEndLatestCell] as 'Mt CallEndLatestCell'

				,vtm.SpeechPathDelayOneWay, 
				
				vtm.Latitude, 
				vtm.Longitude, 
				Logfile.Name, 
						
				vR.NUCS, 
				vR.Car, 
				vR.Log_Network


FROM [Services].[vVoiceSession] ssv
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

INNER JOIN     Datasource AS Datasource ON ssv.DatasourceId = Datasource.Id

LEFT OUTER JOIN VoiceTestingMetrics vtm ON ss.DatasourceId = vtm.DatasourceId

LEFT OUTER JOIN Logfile AS Logfile ON ss.LogfileId = Logfile.Id 		
LEFT OUTER JOIN [cdr].[vRegion] vR ON vR.id = Logfile.Id 
INNER JOIN LookupSIMOperator AS LookupSIMOperator ON Datasource.SimMcc = LookupSIMOperator.MCC AND Datasource.SimMnc = LookupSIMOperator.MNC 
LEFT OUTER JOIN cdr.SessionSummaryVoiceM2M ssvM2M ON (
		(ssvM2M.MoDatasourceId = ss.DatasourceId)
		AND (ssvM2M.MoCallIndex = ss.SessionIdOrCallIndex)
		)
	OR (
		(ssvM2M.MtDataSourceId = ss.DatasourceId)
		AND (ssvM2M.MtCallIndex = ss.SessionIdOrCallIndex)
		)

WHERE 
		
		(Logfile.LogfileProcessingStateId <> 3) and (vtm.PolqaSwbScoreDownlinkAny IS NOT NULL) --and vtm.DatasourceId IN (21045)
		-- order by ssvM2M.[AqmScore] desc
		--and Logfile.Name like '%SGs%'