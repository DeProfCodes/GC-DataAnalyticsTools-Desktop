using Data_Analytics_Tools.BusinessLogic;
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

namespace Data_Analytics_Tools.Pages
{
    /// <summary>
    /// Interaction logic for HomeScreen.xaml
    /// </summary>
    public partial class HomeScreen : Page
    {
        Frame ParentFrame;

        public HomeScreen(Frame frame)
        {
            InitializeComponent();
            ParentFrame = frame;
        }

        private void RunScriptsToExcel_Click(object sender, RoutedEventArgs e)
        {
            ParentFrame.Content = new SqlDataToExcel();
        }

        private void ApacheFilesProcessor_Click(object sender, RoutedEventArgs e)
        {
            ParentFrame.Content = new ApacheToMySQL();
        }
    }
}
