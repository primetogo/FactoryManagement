using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for EditMember.xaml
    /// </summary>
    public partial class EditMember : Window
    {
        MySqlConnection conn = new MySqlConnection();
        MySqlCommand cmd;
        MySqlDataReader reader;
        public EditMember()
        {
            InitializeComponent();
            ClearScreen();
            LoadMemberList();
        }

        private void CheckStateDB()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
            }
        }

        private void LoadMemberList()
        {
            try
            {
                CheckStateDB();
                this.memberList.Items.Clear();
                String member = "SELECT iduser FROM user";
                cmd = new MySqlCommand(member, conn);
                reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        this.memberList.Items.Add(reader.GetString("iduser"));
                    }
                    reader.Close();
                }
                else
                {
                    LockScreen();
                }
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
            
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

        private void DisplayData(String id)
        {
            try
            {
                CheckStateDB();
                String getData = "SELECT username, role FROM user WHERE iduser = @iduser";
                cmd = new MySqlCommand(getData, conn);
                cmd.Parameters.AddWithValue("@iduser", id);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    this.username.Text = reader.GetString("username");
                    if (reader.GetString("role") == "admin")
                    {
                        this.admin.IsChecked = true;
                    }
                    else
                    {
                        this.user.IsChecked = true;
                    }
                }
                reader.Close();
            }catch(Exception e){
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RecordData()
        {
            try
            {
                String user = this.username.Text.ToString();
                String pass = this.password.Password.ToString();
                if (string.IsNullOrWhiteSpace(user) == false && string.IsNullOrWhiteSpace(pass) == false)
                {
                    CheckStateDB();
                    String updating = "UPDATE user SET username=@username, password=@password, role=@role WHERE iduser=@iduser";
                    String role = "";
                    String id = this.memberList.SelectedValue.ToString();
                    System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(pass);
                    byte[] hash = md5.ComputeHash(inputBytes);

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hash.Length; i++)
                    {
                        sb.Append(hash[i].ToString("X2"));
                    }
                    pass = sb.ToString().ToLower();
                    if (this.admin.IsChecked == true)
                    {
                        role = "admin";
                    }
                    else
                    {
                        role = "user";
                    }
                    cmd = new MySqlCommand(updating, conn);
                    cmd.Parameters.AddWithValue("@username", user);
                    cmd.Parameters.AddWithValue("@password", pass);
                    cmd.Parameters.AddWithValue("@iduser", id);
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("เปลี่ยนรหัสผ่านเรียบร้อย", "สถานะบันทึก");
                }
                else
                {
                    MessageBox.Show("กรุณาใส่ชื่อผู้ใช้และรหัสผ่าน", "สถานะบันทึก");
                }
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        public void LockScreen()
        {
            this.memberList.Items.Clear();
            this.memberList.IsEnabled = false;
            this.user.IsEnabled = false;
            this.password.IsEnabled = false;
            this.admin.IsEnabled = false;
            this.user.IsEnabled = false;
        }

        public void ClearScreen()
        {
            this.memberList.Items.Clear();
            this.memberList.IsEnabled = true;
            this.user.IsEnabled = true;
            this.username.Clear();
            this.password.Clear();
            this.password.IsEnabled = true;
            this.admin.IsEnabled = true;
            this.user.IsEnabled = true;
        }

        private void memberList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this.deleteBtn.IsEnabled = true;
                this.btnrec.IsEnabled = true;
                if (this.memberList.SelectedValue != null)
                {
                    String idd = this.memberList.SelectedValue.ToString();
                    DisplayData(idd);

                }else if(this.memberList.SelectedValue == null){
                   //does nothing
                 
                }
                else
                {
                    MessageBox.Show("กรุณาเลือกรหัสสมาชิก", "ข้อผิดพลาด");
                }
            }
            catch (Exception ex)
            {
                ErrorLogCreate(ex);
            }
        }



        private void btnrec_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.memberList.SelectedValue.ToString()) == false)
            {
                RecordData();
                MessageBox.Show("บันทึกข้อมูลสมาชิกแล้ว", "สถานะการบันทึก");
            }
            else
            {
                MessageBox.Show("กรุณาเลือกรหัสสมาชิก", "ข้อผิดพลาด");
            }
            this.btnrec.IsEnabled = false;
            this.deleteBtn.IsEnabled = false;
            ClearScreen();
            LoadMemberList();
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckStateDB();
                String id = this.memberList.SelectedValue.ToString();
                String deleteMember = "DELETE FROM user WHERE iduser = @iduser";
                cmd = new MySqlCommand(deleteMember, conn);
                cmd.Parameters.AddWithValue("@iduser", id);
                cmd.ExecuteNonQuery();
                MessageBox.Show("ลบข้อมูลสมาชิกเรียบร้อยแล้ว", "สถานะการบันทึก");
       
            }
            catch (Exception ex)
            {
                ErrorLogCreate(ex);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                   , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            this.deleteBtn.IsEnabled = false;
            this.btnrec.IsEnabled = false;
            ClearScreen();
            LoadMemberList();
        }
    }
}
