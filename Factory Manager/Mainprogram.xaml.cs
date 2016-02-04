using CrystalDecisions.CrystalReports.Engine;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace Factory_Manager
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    
    public partial class Mainprogram : Window
    {
        MySqlConnection conn = new MySqlConnection();
        public Mainprogram()
        {
            InitializeComponent();
            String sis = (string)Application.Current.Properties["role"];
            if (string.Equals(sis, "admin") == false)
            {
                this.system_main.Visibility = Visibility.Hidden;
            }
            else
            {
                this.system_main.Visibility = Visibility.Visible;
            }
            this.username.Text = (String)Application.Current.Properties["user"];
        }

       

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("ต้องการจะออกจากโปรแกรมหรือไม่?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                    conn.Close();
                }
                Application.Current.Shutdown();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            RecipeCreate ses = new RecipeCreate();
            ses.Show();
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            RecipeEdit ses = new RecipeEdit();
            ses.Show();
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            ProductCreate ses = new ProductCreate();
            ses.Show();
        }

        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {
            ProductEdit ses = new ProductEdit();
            ses.Show();
        }

        private void MenuItem_Click_8(object sender, RoutedEventArgs e)
        {
            CommandCreate ses = new CommandCreate();
            ses.Show();
        }

        private void MenuItem_Click_9(object sender, RoutedEventArgs e)
        {
            CommandEdit ses = new CommandEdit();
            ses.Show();
        }

        private void MenuItem_Click_10(object sender, RoutedEventArgs e)
        {
            ReportPrint ses = new ReportPrint();
            ses.Show();
        }

        private void MenuItem_Click_11(object sender, RoutedEventArgs e)
        {
            TotalData ses = new TotalData();
            ses.Show();
        }

        private void MenuItem_Click_12(object sender, RoutedEventArgs e)
        {
            CreateMember ses = new CreateMember();
            ses.Show();
        }

        private void MenuItem_Click_13(object sender, RoutedEventArgs e)
        {
            EditMember ses = new EditMember();
            ses.Show();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            customerCreate ses = new customerCreate();
            ses.Show();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            materialCreate ses = new materialCreate();
            ses.Show();
        }


    }
}
