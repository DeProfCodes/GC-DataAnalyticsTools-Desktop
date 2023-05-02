using Data_Analytics_Tools.BusinessLogic;
using Data_Analytics_Tools.Constants;
using Data_Analytics_Tools.Helpers;
using Data_Analytics_Tools.Models;
using FontAwesome.WPF;
using Microsoft.Win32;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
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
using static Paket.NetUtils.Auth;
using static Paket.Profile.EventBoundary;

namespace Data_Analytics_Tools.Pages
{
    /// <summary>
    /// Interaction logic for ApacheToMySQL.xaml
    /// </summary>
    public partial class ApacheToMySQL : Page
    {
        private ApacheLogFilesHelper apacheHelper;
        private WebHelper azenqosServer;
        private SQL sql;
        private AppCredentials credentials;

        private AppCredentials tempCreds;


        private BackgroundWorker worker;
        private IBusinessLogicData dataIO;

        private DateTime StartDate;
        private DateTime EndDate;

        private int totalLogHashes;
        private string baseFolder;

        public ApacheToMySQL()
        {
            InitializeComponent();
            

            azenqosServer = new WebHelper();
            
            tempCreds = new AppCredentials();

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;

            progressBd.Visibility = Visibility.Hidden;

            FilesIO.CreateSchemaTextAndOtherTexts();

            ApacheConstants.SqlServer = FilesIO.GetServerName();
            sql = new SQL();

            OpenSavedCredentials();
            SetBaseFolder();
        }

        private void SetBaseFolder()
        {
            dataIO = new BusinessLogicData();

            sourceFolder.Text = FilesIO.GetBaseFolder();
            sourceFolderFull.Text = FilesIO.GetBaseFolder();
            return;
            try
            {
                

                var baseFolder = dataIO.GetBaseFolder();
                sourceFolder.Text = baseFolder;
                sourceFolderFull.Text = baseFolder;
            }
            catch
            {
                sourceFolder.Text = FilesIO.GetBaseFolder();
                sourceFolderFull.Text = FilesIO.GetBaseFolder();
            }
        }
        private void OpenSavedCredentials()
        {
            try
            {
                credentials = sql.GetSavedCredentials() ?? new AppCredentials();
                if (string.IsNullOrEmpty(credentials.AzenqosUsername))
                {
                    MessageBox.Show("Please configure credentials before", "Error Loading Credentials", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else 
                {
                    azenqosServerName.Text = credentials?.AzenqosUsername ?? "n/a";
                    sqlServerName.Text = credentials?.SqlServer ?? "n/a";
                    dbName.Text = credentials?.SqlDatabase ?? "n/a";

                    azenqosUsr.Text = credentials?.AzenqosUsername ?? "";
                    azenqosPwd.Password = credentials?.AzenqosPassword ?? "";
                    sqlUsr.Text = credentials?.SqlUsername ?? "";
                    sqlPwd.Password = credentials?.SqlPassword ?? "";
                    dbNameTxt.Text = credentials?.SqlDatabase ?? "";
                    sqlServerTxt.Text = credentials?.SqlServer ?? "";
                }
                ApacheConstants.SqlServer = credentials?.SqlServer ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong, Please try setting credentials to remove unknown errors", "Error Loading Credentials", MessageBoxButton.OK, MessageBoxImage.Error);
                configureCredentials.Visibility = Visibility.Visible;
            }
        }

        private void OpenDialog(TextBox textBox, TextBlock textBlock)
        {
            var dialog = new SaveFileDialog();
            var initDirec = Directory.Exists(textBox.Text) ? textBox.Text : "C:\\";
            dialog.InitialDirectory = initDirec; // Use current value for initial dir
            dialog.Title = "Select a Directory"; // instead of default "Save As"
            dialog.Filter = "Directory|*.this.directory"; // Prevents displaying files
            dialog.FileName = "select"; // Filename will then be "select.this.directory"

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                // Remove fake filename from resulting path
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                // If user has changed the filename, create the new directory
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                // Our final value is in path
                textBox.Text = path;
                textBlock.Text = path;
            }
        }

        private void SelectSource_Click(object sender, RoutedEventArgs e)
        {
            OpenDialog(sourceFolder, sourceFolderFull);
        }

        private async void ApacheLogDownloadAndUpload_Click(object sender, RoutedEventArgs e)
        {
            //force configure
            if (string.IsNullOrEmpty(credentials.SqlUsername) || string.IsNullOrEmpty(credentials.SqlPassword) || string.IsNullOrEmpty(credentials.AzenqosUsername) ||
                string.IsNullOrEmpty(credentials.AzenqosPassword) || string.IsNullOrEmpty(credentials.SqlServer) || string.IsNullOrEmpty(credentials.SqlDatabase))
            {
                MessageBox.Show("Please configure Azenqos and SQL Credentials to begin the process", "Configure Credentials", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                ApacheConstants.ConnectionString = $"Server={credentials.SqlServer};Database={credentials.SqlDatabase};User Id={credentials.SqlUsername};Password={credentials.SqlPassword};Integrated Security=false;Encrypt=False;";
            }

            if (string.IsNullOrEmpty(ApacheConstants.AzenqosToken))
            {
                var token = await azenqosServer.GetAuthToken(credentials.AzenqosUsername, credentials.AzenqosPassword);
                ApacheConstants.AzenqosToken = token;
            }

            DateTime.TryParse(startDate.Text, out StartDate);
            DateTime.TryParse(endDate.Text, out EndDate);
            progressBd.Visibility = Visibility.Visible;

            dataIO.AddOrUpdateBaseFolderDirectory(sourceFolderFull.Text);
            FilesIO.UpdateBaseFolder(sourceFolderFull.Text);
            FilesIO.UpdateServerName(credentials.SqlServer);

            baseFolder = sourceFolderFull.Text;

            apacheHelper = new ApacheLogFilesHelper();
            apacheHelper.SetApacheLogsDirectory(baseFolder);
            try
            {
                await apacheHelper.CreateLogFileListForDownload(StartDate, EndDate);
                worker.RunWorkerAsync();
            }
            catch(Exception ex)
            {
                if (ex.Message == "Something went wrong with Azenqos Servers, please try again")
                {
                    MessageBox.Show(ex.Message, "Azenqos Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Something went wrong, please try again, if the error persist please contact support", "Azenqos Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                progressBd.Visibility = Visibility.Hidden;
            }
        }

        private void worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            try
            {
                apacheHelper = new ApacheLogFilesHelper();
                apacheHelper.SetApacheLogsDirectory(baseFolder);
                apacheHelper.CreateTablesSchema();
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
                if (ex.Message == "Connection string not set")
                {
                    MessageBox.Show("Please ensure check your SQL connection in configure page", "SQL Error",MessageBoxButton.OK, MessageBoxImage.Error);
                }
                if (ex.Message == "Something went wrong with Azenqos Servers, please try again")
                {
                    MessageBox.Show(ex.Message, "Azenqos Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                MessageBox.Show("An unknown error has occured, please contact support");
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
            var result = MessageBox.Show($"{apacheHelper.TerminationMessage}");
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

        private void ConfigureCredentials_Click(object sender, RoutedEventArgs e)
        {
            configureCredentials.Visibility = Visibility.Visible;

            azenqosProgress.Visibility = Visibility.Hidden;
            azenqosStatus.Visibility = Visibility.Hidden;
            sqlProgress.Visibility = Visibility.Hidden;
            sqlStatus.Visibility = Visibility.Hidden;
            credsProgress.Visibility = Visibility.Hidden;

            try
            {
                credentials = sql.GetSavedCredentials();
            }
            catch
            {
                
            }

            azenqosUsr.Text = credentials?.AzenqosUsername ?? "";
            azenqosPwd.Password = credentials?.AzenqosPassword ?? "";
            sqlUsr.Text = credentials?.SqlUsername ?? "";
            sqlPwd.Password = credentials?.SqlPassword ?? "";
            dbNameTxt.Text = credentials?.SqlDatabase ?? "";
            sqlServerName.Text = credentials?.SqlServer ?? "";
        }

        private bool AllCredentialsChecked()
        {
            bool azenqos = !string.IsNullOrEmpty(tempCreds.AzenqosUsername) && !string.IsNullOrEmpty(tempCreds.AzenqosPassword);
            bool sqlCreds = !string.IsNullOrEmpty(tempCreds.SqlServer) && !string.IsNullOrEmpty(tempCreds.SqlDatabase) &&
                            !string.IsNullOrEmpty(tempCreds.SqlUsername) && !string.IsNullOrEmpty(tempCreds.SqlPassword);

            return azenqos && sqlCreds;
        }

        private async void ConnectAzenqos_Click(object sender, RoutedEventArgs e)
        {
            azenqosProgress.Visibility = Visibility.Visible;
            azenqosProgress.Icon = FontAwesomeIcon.CircleOutlineNotch;
            azenqosProgress.Foreground = Brushes.DarkOrange;
            azenqosProgress.Spin = true;

            string status = "Failed to connected to Azenqos server using above credentials";
            Brush statusColor = Brushes.Red;
            FontAwesomeIcon progrssIcon = FontAwesomeIcon.Times;
            var error = "";

            if (string.IsNullOrEmpty(azenqosUsr.Text) || string.IsNullOrEmpty(azenqosPwd.Password))
            {
                error = "Username and Password cannot be empty";
                string validationErrors = "";

                if (string.IsNullOrEmpty(azenqosUsr.Text)) validationErrors = "Azenqos Username cannot be empty.\n";
                if (string.IsNullOrEmpty(azenqosPwd.Password)) validationErrors += "Azenqos Password cannot be empty.\n";

                MessageBox.Show(validationErrors, "Input Errors", MessageBoxButton.OK, MessageBoxImage.Error);
                azenqosStatus.Foreground = statusColor;
                azenqosStatus.Text = error;
                azenqosStatus.Visibility = Visibility.Visible;

                azenqosProgress.Icon = progrssIcon;
                azenqosProgress.Foreground = statusColor;
                azenqosProgress.Spin = false;

                return;
            }

            try
            {
                var token = await azenqosServer.GetAuthToken(azenqosUsr.Text.Trim(), azenqosPwd.Password);

                if (!string.IsNullOrEmpty(token))
                {
                    status = "Successfully connected to Azenqos server using above credentials";
                    statusColor = Brushes.Green;
                    progrssIcon = FontAwesomeIcon.Check;
                    ApacheConstants.AzenqosToken = token;

                    tempCreds.AzenqosUsername = azenqosUsr.Text;
                    tempCreds.AzenqosPassword = azenqosPwd.Password;
                }
            }
            catch(Exception ex) 
            {
                var azenqosError = ErrorHandling.GetAzenqosConnectionError(ex.Message);
                error = !string.IsNullOrEmpty(azenqosError) ? azenqosError : ex.Message; 
            }
            
            azenqosProgress.Icon = progrssIcon;
            azenqosProgress.Foreground = statusColor;
            azenqosProgress.Spin = false;

            azenqosStatus.Visibility = Visibility.Visible;
            azenqosStatus.Text = status;
            azenqosStatus.Foreground = statusColor;

            if(!string.IsNullOrEmpty(error))
                MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private async void ConnectSQL_Click(object sender, RoutedEventArgs e)
        {
            sqlProgress.Visibility = Visibility.Visible;
            sqlProgress.Icon = FontAwesomeIcon.CircleOutlineNotch;
            sqlProgress.Foreground = Brushes.DarkOrange;
            sqlProgress.Spin = true;

            string status = "Failed to connected to SQL server using above credentials";
            Brush statusColor = Brushes.Red;
            FontAwesomeIcon progrssIcon = FontAwesomeIcon.Times;
            var error = "";

            if (string.IsNullOrEmpty(sqlServerTxt.Text) || string.IsNullOrEmpty(dbNameTxt.Text) || string.IsNullOrEmpty(sqlUsr.Text) || string.IsNullOrEmpty(sqlPwd.Password))
            {
                error = "1 or more textboxes is empty";
                string validationErrors = "";

                if (string.IsNullOrEmpty(sqlServerTxt.Text)) validationErrors = "SQL Server Name cannot be empty.\n";
                if (string.IsNullOrEmpty(dbNameTxt.Text)) validationErrors += "SQL Database Name cannot be empty.\n";
                if (string.IsNullOrEmpty(sqlUsr.Text)) validationErrors += "SQL Username cannot be empty.\n";
                if (string.IsNullOrEmpty(sqlPwd.Password)) validationErrors += "SQL Password cannot be empty.\n";

                MessageBox.Show(validationErrors, "Input Errors", MessageBoxButton.OK, MessageBoxImage.Error);
                sqlStatus.Foreground = statusColor;  
                sqlStatus.Text = error;
                sqlStatus.Visibility = Visibility.Visible;

                sqlProgress.Icon = progrssIcon;
                sqlProgress.Foreground = statusColor;
                sqlProgress.Spin = false;

                return;
            }

            try
            {
                var sqlConnStatus = await sql.CheckSQLConnection(sqlServerTxt.Text, dbNameTxt.Text, sqlUsr.Text, sqlPwd.Password);

                if (sqlConnStatus == SqlConnectionStatus.SUCCESS)
                {
                    status = "Successfully connected to SQL server using above credentials";
                    statusColor = Brushes.Green;
                    progrssIcon = FontAwesomeIcon.Check;

                    tempCreds.SqlServer = sqlServerTxt.Text;
                    tempCreds.SqlUsername = sqlUsr.Text;
                    tempCreds.SqlPassword = sqlPwd.Password;
                    tempCreds.SqlDatabase = dbNameTxt.Text;

                    ApacheConstants.SqlServer = tempCreds.SqlServer;
                }
                else if (sqlConnStatus == SqlConnectionStatus.DATABASE_NOT_FOUND)
                {
                    MessageBoxResult createDb = MessageBox.Show(
                                                                $"The database '{dbNameTxt.Text}' entered does not exist, would you like to crate it?",
                                                                "Database Not Found",
                                                                MessageBoxButton.YesNo,
                                                                MessageBoxImage.Question
                                                               );
                    if (createDb == MessageBoxResult.Yes)
                    {
                        string connectionString = $"Server={sqlServerTxt.Text};User Id={sqlUsr.Text};Password={sqlPwd.Password};Integrated Security=false;Encrypt=False;";
                        string query = $"CREATE DATABASE {dbNameTxt.Text}";
                        sql.SetConnectionString(connectionString);
                        sql.RunQueryOLD(query);

                        MessageBox.Show($"New database with name '{dbNameTxt.Text}' has been created!");

                        status = "Successfully connected to SQL server using above credentials";
                        statusColor = Brushes.Green;
                        progrssIcon = FontAwesomeIcon.Check;

                        tempCreds.SqlServer = sqlServerTxt.Text;
                        tempCreds.SqlUsername = sqlUsr.Text;
                        tempCreds.SqlPassword = sqlPwd.Password;
                        tempCreds.SqlDatabase = dbNameTxt.Text;

                        ApacheConstants.SqlServer = tempCreds.SqlServer;
                    }
                    else
                    {
                        error = $"The database name '{dbNameTxt.Text}' does not exist, please correct this";
                    }
                }
                else if (sqlConnStatus == SqlConnectionStatus.INCORRECT_SERVER_NAME)
                {
                    error = $"The server name '{sqlServerTxt.Text}' is Incorrect, please correct this";
                }
                else if (sqlConnStatus == SqlConnectionStatus.INCORRECT_CREDENTIALS)
                {
                    error = $"The login details for the provided server are incorrect, please enter correct username and password";
                }
                else if (sqlConnStatus == SqlConnectionStatus.OTHER)
                {
                    error = "Something went wrong, unknown error, please contact support";
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            sqlProgress.Icon = progrssIcon;
            sqlProgress.Foreground = statusColor;
            sqlProgress.Spin = false;

            sqlStatus.Visibility = Visibility.Visible;
            sqlStatus.Text = status;
            sqlStatus.Foreground = statusColor;

            if (!string.IsNullOrEmpty(error))
                MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void CancelCredentials_Click(object sender, RoutedEventArgs e)
        {
            configureCredentials.Visibility = Visibility.Hidden;  
        }

        private async void SaveCredentials_Click(object sender, RoutedEventArgs e)
        {
            credsProgress.Visibility = Visibility.Visible;
            if (AllCredentialsChecked())
            {
                await sql.UpdateSavedCredentials(tempCreds);
                MessageBoxResult ans = MessageBox.Show("All Credentials have been saved successfully", "Credentials Saved", MessageBoxButton.OK, MessageBoxImage.Information);

                azenqosServerName.Text = tempCreds.AzenqosUsername;
                sqlServerName.Text = tempCreds.SqlServer;
                dbName.Text = tempCreds.SqlDatabase;

                azenqosProgress.Visibility = Visibility.Hidden;
                azenqosStatus.Visibility = Visibility.Hidden;
                sqlProgress.Visibility = Visibility.Hidden;
                sqlStatus.Visibility = Visibility.Hidden;
                credsProgress.Visibility = Visibility.Hidden;

                configureCredentials.Visibility = Visibility.Hidden;

                credentials = tempCreds;
                FilesIO.UpdateServerName(credentials.SqlServer);
                //credentials.AzenqosUsername = tempCreds.AzenqosUsername;
                //credentials.AzenqosPassword = tempCreds.AzenqosPassword;
                //credentials.SqlUsername = tempCreds.SqlUsername;
                //credentials.SqlPassword = tempCreds.SqlPassword;
                //credentials.SqlDatabase = tempCreds.SqlDatabase;
                //credentials.SqlServer = tempCreds.SqlServer;
            }
            else
            {
                MessageBox.Show("Please Check both Azenqos and SQL Credentials before clicking Save button", "Cannot Save", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }
    }
}
