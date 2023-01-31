using Data_Analytics_Tools.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

namespace Data_Analytics_Tools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker worker;
        private string sourceFolderTxt = "";
        private string destinationFolderTxt = "";

        public MainWindow()
        {
            InitializeComponent();
            
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;

            var savedSrcFolder = FilesIO.GetSavedSourceFolder();
            var savedDstFolder = FilesIO.GetSavedDestinationFolder();

            sourceFolder.Text = savedSrcFolder;
            sourceFolderFull.Text = savedSrcFolder; 
            destinationFolder.Text = savedDstFolder;
            destinationFolderFull.Text = savedDstFolder;    
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;
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

        private void SelectDestination_Click(object sender, RoutedEventArgs e)
        {
            OpenDialog(destinationFolder, destinationFolderFull);
        }

        private void RunScriptsToExcel_Click(object sender, RoutedEventArgs e)
        {
            sourceFolderTxt = sourceFolderFull.Text;
            destinationFolderTxt = destinationFolderFull.Text;

            progressBd.Visibility = Visibility.Visible;
            RunScriptsToExcel.IsEnabled = false;
            
            FilesIO.SaveDirectories(sourceFolderTxt, destinationFolderTxt);

            worker.RunWorkerAsync();
        }

        private void worker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            progressBarTxt.Text = e.ProgressPercentage + "%";
            progressText.Text = e.UserState?.ToString();
        }

        private void worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            try
            {
                var worker = sender as BackgroundWorker;

                var scripts = FilesIO.ReadFileToCompletetion(sourceFolderTxt);

                int processed = 0;
                int total = scripts.Count;
                int percent = 0;
                foreach (var script in scripts)
                {
                    worker.ReportProgress(percent, $"Processing {script.Key}...");

                    try
                    {
                        SQLToExcelHelper.SQLToCSV2(destinationFolderTxt, script.Value, script.Key);

                        processed++;
                        percent = (int)(((double)processed / (double)total) * 100);

                        if (processed == total)
                            worker.ReportProgress(percent, "Done");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error12", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch(Exception ex)
            {
                var result = MessageBox.Show(ex.Message, "Error1", MessageBoxButton.OK, MessageBoxImage.Error);
                worker.CancelAsync();
                if (result == MessageBoxResult.OK)
                {
                    progressBd.Visibility = Visibility.Hidden;
                    RunScriptsToExcel.IsEnabled = true;
                }
            }
        }

        private void worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            var result = MessageBox.Show(owner: this, "Done!");
            if (result == MessageBoxResult.OK)
            {
                progressBd.Visibility = Visibility.Hidden;
                RunScriptsToExcel.IsEnabled = true;
            }
        }

        private void CancelScriptRun_Click(object sender, RoutedEventArgs e)
        {
            worker.CancelAsync();
            var result = MessageBox.Show(owner: this, "Cancelled");
            if (result == MessageBoxResult.OK)
            {
                progressBd.Visibility = Visibility.Hidden;
                RunScriptsToExcel.IsEnabled = true;
            }
        }
    }
}
