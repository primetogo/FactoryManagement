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
    /// Interaction logic for customerCreate.xaml
    /// </summary>
    public partial class customerCreate : Window
    {
        MySqlConnection conn = new MySqlConnection();
        MySqlCommand cmd;
        //Boolean searchTypeStatus = true;

        public customerCreate()
        {
            InitializeComponent();
        }

        private byte[] EncoderUTF(String text)
        {
            ///translate normal input to utf8
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encoded = utf8.GetBytes(text);
            return encoded;
        }

        private void CheckStateDB()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
            }
        }

        private Boolean ValidateInputAction(String typeAction, int searchAction)
        {
            Boolean checkingFlag = false;
            if (typeAction.Equals("record"))
            {
                checkingFlag = !(string.IsNullOrEmpty(this.idCustomer.Text) || string.IsNullOrWhiteSpace(this.idCustomer.Text));
                checkingFlag = !(string.IsNullOrEmpty(this.customerName.Text) || string.IsNullOrWhiteSpace(this.customerName.Text));
            }
            else if (typeAction.Equals("search"))
            {
                if (searchAction == 0)
                {
                    checkingFlag = !(string.IsNullOrEmpty(this.idCustomer.Text) || string.IsNullOrWhiteSpace(this.idCustomer.Text));
                }
                else if (searchAction == 1)
                {
                    checkingFlag = !(string.IsNullOrEmpty(this.customerName.Text) || string.IsNullOrWhiteSpace(this.customerName.Text));
                }
                else if (searchAction == 2)
                {
                    checkingFlag = !(string.IsNullOrEmpty(this.searchQueryOrder1.Text) || string.IsNullOrWhiteSpace(this.searchQueryOrder1.Text));
                    checkingFlag = !(string.IsNullOrEmpty(this.searchQueryOrder2.Text) || string.IsNullOrWhiteSpace(this.searchQueryOrder2.Text));
                }
                else if (searchAction == 3)
                {
                    checkingFlag = !(string.IsNullOrEmpty(this.searchQueryNo1.Text) || string.IsNullOrWhiteSpace(this.searchQueryNo1.Text));
                    checkingFlag = !(string.IsNullOrEmpty(this.searchQueryNo2.Text) || string.IsNullOrWhiteSpace(this.searchQueryNo2.Text));
                }
            }
            return checkingFlag;
        }

        private void ErrorLogCreate(Exception text)
        {
            CultureInfo ci = new CultureInfo("en-US");
            String currentDate = DateTime.Now.ToString("dd-MM-yyyy", ci);
            String currentDateAndTime = DateTime.Now.ToString("dd-MM-yyyy H:mm:ss", ci);
            string path = @"C:\FMmanagement-Log\Log Date " + currentDate + ".txt";
            if (!File.Exists(path))
            {
                FileInfo file = new FileInfo(path);
                file.Directory.Create();
                TextWriter tw = new StreamWriter(path, true);
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

        private void SucceedLogCreate(String act)
        {
            CultureInfo ci = new CultureInfo("en-US");
            String currentDate = DateTime.Now.ToString("dd-MM-yyyy", ci);
            String currentDateAndTime = DateTime.Now.ToString("dd-MM-yyyy H:mm:ss", ci);
            string path = @"C:\FMmanagement-Log\Log Date " + currentDate + ".txt";
            if (!File.Exists(path))
            {
                FileInfo file = new FileInfo(path);
                file.Directory.Create();
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(currentDateAndTime + " => Operation " + act + " success!");
                tw.Close();
            }
            else if (File.Exists(path))
            {
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(currentDateAndTime + " => Operation " + act + " success!");
                tw.Close();
            }
        }

        private void recordingCustomer(String id, String name, String detail)
        {
            ///function for recording customer data
            try
            {
                CheckStateDB();
                String sqlCreate = "INSERT INTO customer (customer_id, customer_name, detail) VALUES(@id, @name, @detail)";
                cmd = new MySqlCommand(sqlCreate, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@detail", detail);
                cmd.ExecuteNonQuery();
                SucceedLogCreate("Recording new customer:customerCreate");
                MessageBox.Show("บันทึกข้อมูลลูกค้าสำเร็จแล้ว"
               , "สถานะการบันทึก", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                if (e.ToString().Contains("Duplicate entry"))
                {
                    MessageBox.Show("มีรหัสลูกค้านี้อยู่ในระบบแล้วกรุณาเปลี่ยนและกรอกรหัสลูกค้าใหม่"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                ErrorLogCreate(e);
            }
        }

        private void searchCustomer()
        {
            try
            {
                if (ValidateInputAction("search", this.typeSearch.SelectedIndex) == true)
                {
                    if (this.typeSearch.SelectedIndex == 0)
                    {
                        String cusId = this.idCustomer.Text;
                        ClearAllBox();
                        CheckStateDB();
                        String searchByCustomerId = "SELECT customer_name, customer_id FROM customer WHERE customer_id REGEXP @id";
                        cmd = new MySqlCommand(searchByCustomerId, conn);
                        cmd.Parameters.AddWithValue("@id", cusId);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows == false)
                        {
                            reader.Close();
                            MessageBox.Show("ไม่พบลูกค้าที่ค้นหา", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                customerResult.Items.Add(new
                                {
                                    CusIdCol = reader.GetString("customer_id"),
                                    CusNameCol = reader.GetString("customer_name")
                                });
                            }
                            reader.Close();
                        }
                        
                    }
                    else if (this.typeSearch.SelectedIndex == 1)
                    {
                        String cusName = this.customerName.Text;
                        ClearAllBox();
                        CheckStateDB();
                        String searchByCustomerName = "SELECT customer_name, customer_id FROM customer WHERE customer_name REGEXP @words";
                        cmd = new MySqlCommand(searchByCustomerName, conn);
                        cmd.Parameters.AddWithValue("@words", cusName);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows == false)
                        {
                            reader.Close();
                            MessageBox.Show("ไม่พบลูกค้าที่ค้นหา", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                customerResult.Items.Add(new
                                {
                                    CusIdCol = reader.GetString("customer_id"),
                                    CusNameCol = reader.GetString("customer_name")
                                });
                            }
                            reader.Close();
                        }
                       
                    }
                    else if (this.typeSearch.SelectedIndex == 2)
                    {
                        String yearNumber = this.searchQueryOrder1.Text, rowNumber = this.searchQueryOrder2.Text;
                        String customerId = "";
                        ClearAllBox();
                        CheckStateDB();
                    
                        String searchByOrderNumber = "SELECT customerId FROM command_card WHERE rowid = @rowid AND year = @year";
                        cmd = new MySqlCommand(searchByOrderNumber, conn);
                        cmd.Parameters.AddWithValue("@rowid", rowNumber.TrimStart('0'));
                        cmd.Parameters.AddWithValue("@year", yearNumber);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows == false)
                        {
                            reader.Close();
                            MessageBox.Show("ไม่พบลูกค้าที่ค้นหา", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                customerId = reader.GetString("customerId");
                            }
                            reader.Close();

                            String searchByCustomerId = "SELECT customer_id, customer_name FROM customer WHERE customer_id = @cusId";
                            CheckStateDB();
                            cmd = new MySqlCommand(searchByCustomerId, conn);
                            cmd.Parameters.AddWithValue("@cusId", customerId);
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                customerResult.Items.Add(new
                                {
                                    CusIdCol = reader.GetString("customer_id"),
                                    CusNameCol = reader.GetString("customer_name")
                                });
                            }
                            reader.Close();
                        }
                    }else if(this.typeSearch.SelectedIndex == 3){
                        String cardNumber = this.searchQueryNo1.Text + "-" + this.searchQueryNo2.Text;
                        String customerId = "";
                        ClearAllBox();
                        CheckStateDB();
    
                        String searchByCardNumber = "SELECT customerId FROM command_card WHERE cardCode REGEXP @word";
                        cmd = new MySqlCommand(searchByCardNumber, conn);
                        cmd.Parameters.AddWithValue("@word", cardNumber);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows == false)
                        {
                            reader.Close();
                            MessageBox.Show("ไม่พบลูกค้าที่ค้นหา", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                customerId = reader.GetString("customerId");
                            }
                            reader.Close();

                            String searchByCustomerId = "SELECT customer_id, customer_name FROM customer WHERE customer_id = @cusId";
                            CheckStateDB();
                            cmd = new MySqlCommand(searchByCustomerId, conn);
                            cmd.Parameters.AddWithValue("@cusId", customerId);
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                customerResult.Items.Add(new
                                {
                                    CusIdCol = reader.GetString("customer_id"),
                                    CusNameCol = reader.GetString("customer_name")
                                });
                            }
                            reader.Close();
                          
                        }
                    }
                }
                SucceedLogCreate("Retreive customer result:customerCreate");
            }
            catch (Exception e)
            {
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                ErrorLogCreate(e);
            }
        }



        private void ShowSearchQueryBox()
        {
            if (this.typeSearch.SelectedIndex == 2)
            {
                this.searchQueryOrderLabel.Visibility = Visibility.Visible;
                this.searchQueryOrder1.Visibility = Visibility.Visible;
                this.searchQueryOrder2.Visibility = Visibility.Visible;
                this.searchQueryOrderDat.Visibility = Visibility.Visible;

                this.searchQueryNo1.Visibility = Visibility.Hidden;
                this.searchQueryNo2.Visibility = Visibility.Hidden;
                this.searchQueryNoDat.Visibility = Visibility.Hidden;
                this.searchQueryNoLabel.Visibility = Visibility.Hidden;

            }
            else if (this.typeSearch.SelectedIndex == 3)
            {
                this.searchQueryNo1.Visibility = Visibility.Visible;
                this.searchQueryNo2.Visibility = Visibility.Visible;
                this.searchQueryNoDat.Visibility = Visibility.Visible;
                this.searchQueryNoLabel.Visibility = Visibility.Visible;

                this.searchQueryOrderLabel.Visibility = Visibility.Hidden;
                this.searchQueryOrder1.Visibility = Visibility.Hidden;
                this.searchQueryOrder2.Visibility = Visibility.Hidden;
                this.searchQueryOrderDat.Visibility = Visibility.Hidden;
            }
            else
            {
                this.searchQueryNo1.Visibility = Visibility.Hidden;
                this.searchQueryNo2.Visibility = Visibility.Hidden;
                this.searchQueryNoDat.Visibility = Visibility.Hidden;
                this.searchQueryNoLabel.Visibility = Visibility.Hidden;

                this.searchQueryOrderLabel.Visibility = Visibility.Hidden;
                this.searchQueryOrder1.Visibility = Visibility.Hidden;
                this.searchQueryOrder2.Visibility = Visibility.Hidden;
                this.searchQueryOrderDat.Visibility = Visibility.Hidden;
                
            }
        }
        private void ClearAllBox()
        {
            this.idCustomer.Text = "";
            this.idCustomer.IsEnabled = true;
            this.customerName.IsEnabled = true;
            this.recordBtn.IsEnabled = true;
            this.customerDetail.IsEnabled = true;
            this.customerName.Text = "";
            this.customerDetail.Text = "";
            this.typeSearch.SelectedIndex = -1;
            this.searchQueryOrder1.Text = "";
            this.searchQueryOrder2.Text = "";
            this.searchQueryNo1.Text = "";
            this.searchQueryNo2.Text = "";
            this.customerResult.Items.Clear();
            
        }
        private void typeSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.typeSearch.SelectedIndex == 0)
            {
                this.idCustomer.IsEnabled = true;
                this.idCustomer.Clear();
                this.customerName.Clear();
                this.customerDetail.Clear();
                this.customerName.IsEnabled = false;
                this.customerDetail.IsEnabled = false;
            }
            else if (this.typeSearch.SelectedIndex == 1)
            {
                this.customerName.IsEnabled = true;
                this.idCustomer.Clear();
                this.customerName.Clear();
                this.customerDetail.Clear();
                this.idCustomer.IsEnabled = false;
                this.customerDetail.IsEnabled = false;
            }
            else if (this.typeSearch.SelectedIndex > 1)
            {
                this.idCustomer.Clear();
                this.customerName.Clear();
                this.customerDetail.Clear();
                this.customerName.IsEnabled = false;
                this.idCustomer.IsEnabled = false;
                this.customerDetail.IsEnabled = false;
            }
            ShowSearchQueryBox();
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.typeSearch.SelectedIndex > 0)
            {
                this.customerResult.IsEnabled = true;
                searchCustomer();
            }
            else
            {
                MessageBox.Show("กรุณาเลือกประเภทการค้นหา"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }

        private void recordBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInputAction("record", this.typeSearch.SelectedIndex) == true)
            {
                String id = this.idCustomer.Text;
                String name = this.customerName.Text;
                String detail = "-";
                if (string.IsNullOrEmpty(this.customerDetail.Text) || string.IsNullOrWhiteSpace(this.customerDetail.Text) == true)
                {
                    detail = this.customerDetail.Text;
                }
                recordingCustomer(id, name, detail);
                ClearAllBox();

            }
            else
            {
                MessageBox.Show("ใส่ข้อมูลที่จำเป็นไม่ครบถ้วน", "ข้อผิดพลาด");
            }
        }
        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                 this.recordBtn.IsEnabled = false;
                 this.customerName.IsEnabled = true;
                 this.customerDetail.IsEnabled = true;

                 String customerNumber = "";
                 var item = sender as ListViewItem;
                 if (item != null && item.IsSelected)
                 {
                     dynamic selectedItem = customerResult.SelectedItems[0];
                     customerNumber = selectedItem.CusIdCol;
                 }
                String searchByCustomerId = "SELECT customer_id, customer_name, detail FROM customer WHERE customer_id = @cusId";
                CheckStateDB();
                cmd = new MySqlCommand(searchByCustomerId, conn);
                cmd.Parameters.AddWithValue("@cusId", customerNumber);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    this.customerName.Text = reader.GetString("customer_name");
                    this.idCustomer.Text = reader.GetString("customer_id");
                    this.customerDetail.Text = reader.GetString("detail");
                }
                reader.Close();
                
                this.updateBtn.IsEnabled = true;
                SucceedLogCreate("Selected customer data retreive:customerCreate");
            }catch(Exception ex){
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                ErrorLogCreate(ex);
            }
        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            this.idCustomer.IsEnabled = false;
            String customerNewName = this.customerName.Text;
            String customerDetail = this.customerDetail.Text;
            try
            {
                String updateCustomer = "UPDATE customer SET customer_name = @name, detail = @detail WHERE customer_id = @id";
                CheckStateDB();
                cmd = new MySqlCommand(updateCustomer, conn);
                cmd.Parameters.AddWithValue("@name", EncoderUTF(customerNewName));
                cmd.Parameters.AddWithValue("@detail", EncoderUTF(customerDetail));
                cmd.Parameters.AddWithValue("@id", this.idCustomer.Text);
                cmd.ExecuteNonQuery();
                
                SucceedLogCreate("Update customer data");
                MessageBox.Show("บันทึกข้อมูลลูกค้าสำเร็จแล้ว"
                , "สถานะการบันทึก", MessageBoxButton.OK, MessageBoxImage.Information);

                String searchByCustomerId = "SELECT customer_id, customer_name, detail FROM customer WHERE customer_id = @cusId";
                CheckStateDB();
                cmd = new MySqlCommand(searchByCustomerId, conn);
                cmd.Parameters.AddWithValue("@cusId", this.idCustomer.Text);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    this.customerName.Text = reader.GetString("customer_name");
                    this.idCustomer.Text = reader.GetString("customer_id");
                    this.customerDetail.Text = reader.GetString("detail");
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                ErrorLogCreate(ex);
            }
            ClearAllBox();
            this.customerResult.IsEnabled = false;
            this.updateBtn.IsEnabled = false;
            this.idCustomer.IsEnabled = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            conn.Close();
        }
    }
}
