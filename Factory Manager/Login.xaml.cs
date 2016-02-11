using MySql.Data.MySqlClient;
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

namespace Factory_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MySqlConnection conn = new MySqlConnection();
        MySqlCommand cmd;
        Boolean loginFlag = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        public Boolean ConnectionTester(String e)
        {
            //function for testing database connection
            Boolean flagCheck = false;
            conn.ConnectionString = e;
            try { conn.Open(); flagCheck = true; conn.Close(); }
            catch(Exception ex) {
                MessageBox.Show(ex.ToString());
                flagCheck = false; }
            return flagCheck;
        }

        public String[] getPass(String user, String Connector)
        {
            //function for get password and role from database
            String[] data = new String[3];
            conn.ConnectionString = Connector;
            String sql_pass = String.Format("SELECT password, role, iduser FROM user WHERE username='{0}'", user);
            cmd = new MySqlCommand(sql_pass, conn);
            conn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data[0] = reader.GetString("password");
                data[1] = reader.GetString("role");
                data[2] = reader.GetString("iduser");
            }
            conn.Close();
            return data;
        }

        public String MD5Convert(String passy)
        {
            //function for convert normal password string to MD5
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(passy);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String connLine = "";
            if (Application.Current.Properties["sqlCon"] == null)
            {
                Application.Current.Properties["sqlCon"] = "SERVER=localhost; uid=root; pwd='itkmitl2014'; database=factory management; charset=utf8;";
                connLine = (String)Application.Current.Properties["sqlCon"];
            }
            else
            {
                connLine = (String)Application.Current.Properties["sqlCon"];
            }
            Boolean flag = ConnectionTester(connLine);
            if (flag == true)
            {
                //MessageBox.Show("Database connection is ok!", "สถานะ Database");
                String[] dataComing = getPass(this.username.Text, connLine);
                String pass_out = MD5Convert(this.pass.Password);
                if (pass_out.ToLower() == dataComing[0])
                {
                    loginFlag = true;
                    Application.Current.Properties["role"] = dataComing[1];
                    Application.Current.Properties["iduser"] = dataComing[2];

                    MessageBox.Show(this.username.Text);
                    Mainprogram sesy = new Mainprogram();
                    sesy.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("รหัสผ่านหรือ username ผิด", "Factory Manager: Failed to login");
                }
                Application.Current.Properties["userFlag"] = loginFlag;
            }
            else
            {
                if (MessageBox.Show("การเชื่อมต่อ database มีปัญหากรุณาใส่ข้อมูลการเชื่อมต่อใหม่", "สถานะ Database", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    checkConnect sesy = new checkConnect();
                    sesy.Show();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.username.Clear();
            this.pass.Clear();
        }

        private void pass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsDown && e.Key == Key.Enter)
            {
                String connLine = "";
                if (Application.Current.Properties["sqlCon"] == null)
                {
                    Application.Current.Properties["sqlCon"] = "SERVER=localhost; uid=root; pwd=itkmitl2014; database=factory management";
                    connLine = (String)Application.Current.Properties["sqlCon"];
                }
                else
                {
                    connLine = (String)Application.Current.Properties["sqlCon"];
                }
                Boolean flag = ConnectionTester(connLine);
                if (flag == true)
                {
                    //MessageBox.Show("Database connection is ok!", "สถานะ Database");
                    String[] dataComing = getPass(this.username.Text, connLine);
                    String pass_out = MD5Convert(this.pass.Password);
                    if (pass_out.ToLower() == dataComing[0])
                    {
                        loginFlag = true;
                        Application.Current.Properties["role"] = dataComing[1];
                        Application.Current.Properties["iduser"] = dataComing[2];
                        Application.Current.Properties["user"] = this.username.Text;
                        Mainprogram sesy = new Mainprogram();
                        sesy.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("รหัสผ่านหรือ username ผิด", "Factory Manager: Failed to login");
                    }
                    Application.Current.Properties["userFlag"] = loginFlag;
                }
                else
                {
                    if (MessageBox.Show("การเชื่อมต่อ database มีปัญหากรุณาใส่ข้อมูลการเชื่อมต่อใหม่", "สถานะ Database", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        checkConnect sesy = new checkConnect();
                        sesy.Show();
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }
                }
            }
        }

        private void username_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsDown && e.Key == Key.Enter)
            {
                String connLine = "";
                if (Application.Current.Properties["sqlCon"] == null)
                {
                    Application.Current.Properties["sqlCon"] = "SERVER=localhost; uid=root; pwd=itkmitl2014; database=factory management";
                    connLine = (String)Application.Current.Properties["sqlCon"];
                }
                else
                {
                    connLine = (String)Application.Current.Properties["sqlCon"];
                }
                Boolean flag = ConnectionTester(connLine);
                if (flag == true)
                {
                    //MessageBox.Show("Database connection is ok!", "สถานะ Database");
                    String[] dataComing = getPass(this.username.Text, connLine);
                    String pass_out = MD5Convert(this.pass.Password);
                    if (pass_out.ToLower() == dataComing[0])
                    {
                        loginFlag = true;
                        Application.Current.Properties["role"] = dataComing[1];
                        Application.Current.Properties["iduser"] = dataComing[2];
                        Mainprogram sesy = new Mainprogram();
                        sesy.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("รหัสผ่านหรือ username ผิด", "Factory Manager: Failed to login");
                    }
                    Application.Current.Properties["userFlag"] = loginFlag;
                }
                else
                {
                    if (MessageBox.Show("การเชื่อมต่อ database มีปัญหากรุณาใส่ข้อมูลการเชื่อมต่อใหม่", "สถานะ Database", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        checkConnect sesy = new checkConnect();
                        sesy.Show();
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }
                }
            }
        }


    }
}
