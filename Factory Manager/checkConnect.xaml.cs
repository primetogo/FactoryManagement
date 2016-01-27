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
using System.Windows.Shapes;

namespace Factory_Manager
{
    /// <summary>
    /// Interaction logic for checkConnect.xaml
    /// </summary>
    public partial class checkConnect : Window
    {
        public checkConnect()
        {
            InitializeComponent();
        }

        private void db_record_Click(object sender, RoutedEventArgs e)
        {
            String dbLink = this.dbUrl.Text, dbUsername = this.dbName.Text, dbPassword = this.dbPass.Text;
            String dbNameFull = this.dbName.Text;
            Application.Current.Properties["sqlCon"] = "SERVER=" + dbLink + "; uid=" + dbUsername + "; pwd=" + dbPassword + "; database=" + dbNameFull + "charset=utf8";
            this.Close();
        }
    }
}
