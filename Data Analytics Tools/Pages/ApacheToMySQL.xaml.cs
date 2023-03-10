using Data_Analytics_Tools.BusinessLogic;
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

            apacheHelper = new ApacheLogFilesHelper();

            await apacheHelper.CreateTablesSchema();
            await apacheHelper.CreateLogFileListForDownload(StartDate, EndDate);

            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            try
            {
                apacheHelper = new ApacheLogFilesHelper();
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
            var message = e.UserState?.ToString() ?? "";
            var data = message.Split("#");

            if (data.Length > 1)
            {
                if (data[0] == "TotalLogHashes")
                {
                    LogHashNumberTotal.Text = data[1];
                }
                if (data[0] == "logProcessingName")
                {
                    logProcessingName.Text = data[1];
                }
                if (data[0] == "logTableProcessed" && data[1] !="0")
                {
                    var tableCount = Convert.ToDouble(data[1]);
                    int tableProgress = (int)((tableCount / 171.0) * 100); 

                    tableProcessedCount.Text = data[1];
                    tableProgressBar.Value = tableProgress;
                    tableProgressBarTxt.Text = $"{tableProgress}%";
                }
            }
            
            //progressBar.Value = e.ProgressPercentage;
            //progressBarTxt.Text = e.ProgressPercentage + "%";
            //progressText.Text = e.UserState?.ToString();
        }

        private void worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            var result = MessageBox.Show("Done!");
            if (result == MessageBoxResult.OK)
            {
                progressBd.Visibility = Visibility.Hidden;
                
                CancelScriptRun.Content = "Abort";
                CancelScriptRun.Width = 50;
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
    }
}
