SELECT			NrMetrics.DatasourceId, NrMetrics.SampleId, 
				NrMetrics.SessionId, NrMetrics.CurrentCallIndex, 
				 NrMetrics.DateTime, 
				cdr.vRegion.Campaign,
				cdr.vRegion.Region, 
						 cdr.vRegion.Geographic,
                         NrMetrics.MultiRatConnectivityMode, 
						 NrMetricsNrServingCellIndex.NrServingCellType,
						 Datasource.IMSI, 
						 left(Datasource.IMSI,5) as Operator,
						
						 NrMetrics.RadioTechnology,
						 NrMetrics.Latitude, NrMetrics.Longitude, NrMetrics.Altitude, 
						 NrMetricsNrServingCellIndex.NrServingCellIndex, 
						 NrMetricsNrServingCellIndex.NrServingCellBandwidthDownlink, 
						  
                         NrMetricsNrServingCellIndex.NrServingCellNrArfcnDownlink, 
						 NrMetricsNrServingCellIndex.NrServingCellNrArfcnUplink, 
						 NrMetricsNrServingCellIndex.NrServingCellGscn, 
						 NrMetricsNrServingCellIndex.NrServingCellGroup, 
                         NrMetricsNrServingCellIndex.NrServingCellPhysicalCellIdentity, 
						 
						 NrMetricsNrServingCellIndex.NrServingSsbBeamIndex, 
						 NrMetricsNrServingCellIndex.NrServingCellSsRsrp, 
                         NrMetricsNrServingCellIndex.NrServingCellSsRsrq, 
						 NrMetricsNrServingCellIndex.NrServingCellSsSinr, 
						 NrMetricsNrServingCellIndex.NrDownlinkPathloss, 
						 NrMetricsNrServingCellIndex.NrPuschTransmitPower, 
                         NrMetricsNrServingCellIndex.NrServingCellRrcCellIdentityComplete, 
						 Logfile.Name,  
						 cdr.vRegion.Area, 
						 cdr.vRegion.Route, 
						 cdr.vRegion.NUCS, 
						 cdr.vRegion.Car, 
                         cdr.vRegion.Log_Network 
						 
FROM            NrMetrics 
				INNER JOIN NrMetricsNrServingCellIndex ON NrMetrics.DatasourceId = NrMetricsNrServingCellIndex.DatasourceId 
				AND NrMetrics.SampleId = NrMetricsNrServingCellIndex.SampleId 
				INNER JOIN Datasource ON NrMetrics.DatasourceId = Datasource.Id 
				INNER JOIN Logfile ON Datasource.LogfileId = Logfile.Id 
				INNER JOIN cdr.vRegion ON Datasource.LogfileId = cdr.vRegion.id