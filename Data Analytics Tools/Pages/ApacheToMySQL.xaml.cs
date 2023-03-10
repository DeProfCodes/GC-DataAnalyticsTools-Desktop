﻿using Data_Analytics_Tools.BusinessLogic;
using Data_Analytics_Tools.Helpers;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Paket.Profile.EventBoundary;

namespace Data_Analytics_Tools.Pages
{
    /// <summary>
    /// Interaction logic for ApacheToMySQL.xaml
    /// </summary>
    public partial class ApacheToMySQL : Page
    {
        private ApacheLogFilesHelper apacheHelper;
        private BackgroundWorker worker;
        private DateTime StartDate;
        private DateTime EndDate;

        private int totalLogHashes;
        private string terminationMessage;

        public ApacheToMySQL()
        {
            InitializeComponent();

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;

            progressBd.Visibility = Visibility.Hidden;
        }

        private async void ApacheLogDownloadAndUpload_Click(object sender, RoutedEventArgs e)
        {
            DateTime.TryParse(startDate.Text, out StartDate);
            DateTime.TryParse(endDate.Text, out EndDate);
            progressBd.Visibility = Visibility.Visible;

            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            try
            {
                apacheHelper = new ApacheLogFilesHelper();
                apacheHelper.CreateTablesSchema();
                apacheHelper.CreateLogFileListForDownload(StartDate, EndDate);

                apacheHelper.isRunning = true;

                apacheHelper.DownloadAndImportApacheFilesToMySQL(StartDate, EndDate, worker);
                while (apacheHelper.isRunning)
                {
                    Thread.Sleep(10000);
                }
            }
            catch (Exception ex)
            {
                worker.CancelAsync();
            }
        }

        private void worker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            try
            {
                var message = e.UserState?.ToString() ?? "";
                var data = message.Split("#");

                if (data.Length > 1)
                {
                    if (data[0] == "TotalLogHashes")
                    {
                        LogHashNumberCount.Text = "0";
                        LogHashNumberTotal.Text = data[1];
                        totalLogHashes = Convert.ToInt32(data[1]);
                    }
                    else if (data[0] == "logProcessingName")
                    {
                        logProcessingName.Text = data[1];
                    }
                    else if (data[0] == "logTableProcessed" && data[1] != "0")
                    {
                        var tableCount = Convert.ToDouble(data[1]);
                        int tableProgress = (int)((tableCount / 171.0) * 100);

                        tableProcessedCount.Text = data[1];
                        tableProgressBar.Value = tableProgress;
                        tableProgressBarTxt.Text = $"{tableProgress}%";
                    }
                    else if (data[0] == "LogHashNumberCount")
                    {
                        var logHashCount = Convert.ToDouble(data[1]);
                        double logHashesProgress = Math.Round(((logHashCount / totalLogHashes) * 100), 1);

                        LogHashNumberCount.Text = data[1];
                        logHashProgressBar.Value = logHashesProgress;
                        logHashesProgressBarTxt.Text = $"{logHashesProgress}%";
                    }
                    else if (data[0] == "downloadsCount")
                    {
                        downloadsCountTxt.Text = data[1];
                    }
                    else if (data[0] == "importsCount")
                    {
                        importsCountTxt.Text = data[1];
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            var result = MessageBox.Show($"{terminationMessage}");
            if (result == MessageBoxResult.OK)
            {
                progressBd.Visibility = Visibility.Hidden;
                
                CancelScriptRun.Content = "Abort";
                CancelScriptRun.Width = 70;

                logProcessingName.Text = "loading...";

                tableProcessedCount.Text = "0";
                tableProgressBar.Value = 0;
                tableProgressBarTxt.Text = "0%";

                LogHashNumberCount.Text = "-";
                logHashProgressBar.Value = 0;
                logHashesProgressBarTxt.Text = "0%";
                LogHashNumberTotal.Text = "-";

                downloadsCountTxt.Text = "0";
            }
        }

        private void CancelScriptRun_Click(object sender, RoutedEventArgs e)
        {
            //Application.Current.Shutdown();
            if (worker.IsBusy)
            {
                worker.CancelAsync();
                //sql.CancelWork();
                CancelScriptRun.Content = "canceling please wait...";
                CancelScriptRun.Width = 150;
                //CancelScriptRun.IsEnabled = false;
                //CancelScriptRun.Background = Brushes.Tomato;
            }
        }

        private void CancelScriptRun_Click_1(object sender, RoutedEventArgs e)
        {
            apacheHelper.isRunning = false;
            apacheHelper.TerminationMessage = "Operation Cancelled!";
        }
    }
}
