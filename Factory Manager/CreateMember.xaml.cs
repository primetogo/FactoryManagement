using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for CreateMember.xaml
    /// </summary>
    public partial class CreateMember : Window
    {
        MySqlConnection conn = new MySqlConnection();
        MySqlCommand cmd, cmd2;
        public CreateMember()
        {
            InitializeComponent();
        }

        private void ClearScreen()
        {
            this.username.Clear();
            this.password.Clear();
            this.admin.IsChecked = true;
        }
        private void ErrorLogCreate(Exception text)
        {
            CultureInfo ci = new CultureInfo("en-US");
            String currentDate = DateTime.Now.ToString("dd-MM-yyyy", ci);
            String currentDateAndTime = DateTime.Now.ToString("dd-MM-yyyy H:mm:ss", ci);
            string path = @"C:\FMmanagement-Log\Log Date " + currentDate + ".txt";
            if (!File.Exists(path))
            {
                File.Create(path);
                TextWriter tw = new StreamWriter(path);
                tw.WriteLine(currentDateAndTime + " => " + text.ToString());
                tw.Close();
            }
            else if (File.Exists(path))
            {
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(currentDateAndTime + " => " + text.ToString());
                tw.Close();
            }
        }

        private void RecordUserData()
        {
            try
            {
                String userData = this.username.Text;
                String pass = this.password.Password;
                String role = "";
                if (this.user.IsChecked == true)
                {
                    role = "user";
                }
                else
                {
                    role = "admin";
                }
                String sqlInsert = "INSERT user (username, password, role) VALUES(@username, @password, @role)";
                String sqlCheck = "SELECT COUNT(*) FROM user WHERE username=@username";
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
                System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(pass);
                byte[] hash = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                pass = sb.ToString().ToLower();
                cmd2 = new MySqlCommand(sqlCheck, conn);
                cmd2.Parameters.AddWithValue("@username", EncoderUTF(userData));
                MySqlDataReader read = cmd2.ExecuteReader();
                if (read.GetInt64(0) > 0)
                {
                    throw new Exception("exists");
                }
                else
                {
                    cmd = new MySqlCommand(sqlInsert, conn);
                    cmd.Parameters.AddWithValue("@username", EncoderUTF(userData));
                    cmd.Parameters.AddWithValue("@password", pass);
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("บันทึกข้อมูลสมาชิกแล้ว", "สถานะการบันทึก");
                    conn.Close();
                    ClearScreen();
                }
                conn.Close();
            }
            catch (Exception er)
            {
                conn.Close();
                ErrorLogCreate(er);
                MessageBox.Show("มี username นี้แล้วหรือลืมใส่รหัสผ่าน", "สถานะการบันทึก");
            }



        }

        private byte[] EncoderUTF(String text)
        {
            ///translate normal input to utf8
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encoded = utf8.GetBytes(text);
            return encoded;

        }



        private void btnrec_Click(object sender, RoutedEventArgs e)
        {
            RecordUserData();
        }
    }
}
