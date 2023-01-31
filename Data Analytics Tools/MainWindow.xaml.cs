using Data_Analytics_Tools.Helpers;
using System;
using System.Collections.Generic;
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
        SQL sql;
        public MainWindow()
        {
            InitializeComponent();
            sql = new SQL();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;
        }

        private void RunScriptsToExcel_Click(object sender, RoutedEventArgs e)
        {
            progressSp.Visibility = Visibility.Visible;

            var scripts = FilesIO.ReadFileToCompletetion();

            int x = 0;
            foreach (var script in scripts)
            {
                if (x == 0)
                {
                    x = 1;
                    continue;
                }
                progressText.Text = $"Processing {script.Key}.sql ...";
                SQLToExcelHelper.SQLToCSV2(script.Value, script.Key);
            }
            
            var result = MessageBox.Show("Done!");
            if (result == MessageBoxResult.OK)
            {
                progressBar.Visibility = Visibility.Hidden;
            }
        }
    }
}
