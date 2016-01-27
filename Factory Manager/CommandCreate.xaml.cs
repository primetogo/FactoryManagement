using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Shapes;

namespace Factory_Manager
{
    /// <summary>
    /// Interaction logic for CommandCreate.xaml
    /// </summary>
    public partial class CommandCreate : Window
    {
        MySqlConnection conn = new MySqlConnection();
        MySqlCommand cmd;
        Boolean recPass1 = false, recPass2 = false, recPass3 = false;
        String[] currentNumber = { };
        public CommandCreate()
        {
            InitializeComponent();
            this.finishDate.DisplayDateStart = DateTime.Now;
            currentNumber = CountCardOn();
            RetreiveCustomer();
            RetreiveProduct();

        }

        private void RetreiveCustomer()
        {
            ///Retrieve customer data from database to add in combobox
            try
            {
                String connecter = (String)Application.Current.Properties["sqlCon"];
                conn.ConnectionString = connecter;
                String sql_get = "SELECT customer_id FROM customer";
                cmd = new MySqlCommand(sql_get, conn);
                conn.Open();
                MySqlDataReader road = cmd.ExecuteReader();
                if (road.HasRows == true)
                {
                    while (road.Read())
                    {
                        this.customerList.Items.Add(road.GetString("customer_id"));
                    }
                    conn.Close();
                    SucceedLogCreate("retreive customer data");
                }
                else
                {
                    throw new Exception("No row were found!");
                }
            }
            catch (Exception e)
            {
                conn.Close();
                ErrorLogCreate(e);
                LockScreen("lock");
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
                File.Create(path);
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
        private void RetreiveProduct()
        {
            ///Get product list from database
            try
            {
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
                String sqlGet = "SELECT product_id FROM product";
                cmd = new MySqlCommand(sqlGet, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        this.productList.Items.Add(reader.GetString("product_id"));
                    }
                    conn.Close();
                    SucceedLogCreate("retreive product data");
                }
                else
                {
                    throw new Exception("No row were found!");

                }
            }
            catch (Exception ex)
            {
                conn.Close();
                ErrorLogCreate(ex);
                LockScreen("lock");
            }
        }

        private String[] CountCardOn()
        {
            String[] code = new String[4];
            try
            {
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
                int number;

                String[] prefix = { "PR", "BL", "CU", "RW" };
                CultureInfo ci = new CultureInfo("en-US");
                String currentYear = DateTime.Now.ToString("yy", ci);
                String getNum = "SELECT COUNT(*) FROM command_card";
                cmd = new MySqlCommand(getNum, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                number = reader.GetInt32(0);
                for (int i = 0; i < 4; i++)
                {
                    code[i] = prefix[i] + currentYear + "-" + (number + 1).ToString().PadLeft(4, '0');
                }
                conn.Close();
                this.printingCode.Text = code[0];
                this.blowingCode.Text = code[1];
                this.cuttingCode.Text = code[2];
                this.matCode.Text = code[3];
                SucceedLogCreate("counting card");
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return code;
        }

        private void LockScreen(String action)
        {
            ///clear screen or lock screen
            if (action == "clear")
            {
                this.printingCode.Clear();
                this.printingCode.IsEnabled = true;
                this.blowingCode.Clear();
                this.blowingCode.IsEnabled = true;
                this.cuttingCode.Clear();
                this.cuttingCode.IsEnabled = true;
                this.matCode.Clear();
                this.matCode.IsEnabled = true;
                this.paperNumber.Clear();
                this.paperNumber.IsEnabled = true;
                this.paperRealAmount.Clear();
                this.paperRealAmount.IsEnabled = true;
                this.paperReqAmount.Clear();
                this.paperReqAmount.IsEnabled = true;
                this.finishDate.SelectedDate = null;
                this.finishDate.IsEnabled = true;
                this.finishDate.DisplayDate = DateTime.Today;
                this.customerList.SelectedIndex = -1;
                this.customerList.Items.Clear();
                this.customerList.IsEnabled = true;
                this.productList.SelectedIndex = -1;
                this.productList.Items.Clear();
                this.productList.IsEnabled = true;
                this.customerName.Clear();
                this.customerName.IsEnabled = true;
                this.productName.Clear();
                this.productName.IsEnabled = true;
                this.printingPaperDetail.Clear();
                this.printingPaperDetail.IsEnabled = true;
                this.blowingPaperDetail.Clear();
                this.blowingPaperDetail.IsEnabled = true;
                this.cuttingPaperDetail.Clear();
                this.cuttingPaperDetail.IsEnabled = true;
                this.recBtn.IsEnabled = true;
            }
            else
            {
                this.printingCode.Clear();
                this.printingCode.IsEnabled = false;
                this.blowingCode.Clear();
                this.blowingCode.IsEnabled = false; ;
                this.cuttingCode.Clear();
                this.cuttingCode.IsEnabled = false;
                this.matCode.Clear();
                this.matCode.IsEnabled = false;
                this.paperNumber.Clear();
                this.paperNumber.IsEnabled = false;
                this.paperRealAmount.Clear();
                this.paperRealAmount.IsEnabled = false;
                this.paperReqAmount.Clear();
                this.paperReqAmount.IsEnabled = false;
                this.finishDate.SelectedDate = null;
                this.finishDate.IsEnabled = false;
                this.finishDate.DisplayDate = DateTime.Today;
                this.customerList.Items.Clear();
                this.customerList.Items.Add("ไม่มีข้อมูลลูกค้าหรือผลิตภัณฑ์ในระบบ");
                this.customerList.SelectedIndex = 0;
                this.customerList.IsEnabled = false;
                this.productList.SelectedIndex = -1;
                this.productList.Items.Clear();
                this.productList.IsEnabled = false;
                this.customerName.Clear();
                this.customerName.IsEnabled = false;
                this.productName.Clear();
                this.productName.IsEnabled = false;
                this.printingPaperDetail.Clear();
                this.printingPaperDetail.IsEnabled = false;
                this.blowingPaperDetail.Clear();
                this.blowingPaperDetail.IsEnabled = false;
                this.cuttingPaperDetail.Clear();
                this.cuttingPaperDetail.IsEnabled = false;
                this.recBtn.IsEnabled = false;

            }
        }

        private void GetCustomerName(String id)
        {
            ///get customername from custoermer id
            try
            {
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
                String sqlGet = "SELECT customer_name FROM customer WHERE customer_id = @customerId";
                cmd = new MySqlCommand(sqlGet, conn);
                cmd.Parameters.AddWithValue("@customerId", id);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    this.customerName.Text = reader.GetString("customer_name");
                }
                conn.Close();
                SucceedLogCreate("retreive customer name");
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void GetProductName(String id)
        {
            ///get product name from id
            try
            {
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
                String sqlGet = "SELECT product_name FROM product WHERE product_id = @cusid";
                cmd = new MySqlCommand(sqlGet, conn);
                cmd.Parameters.AddWithValue("@cusid", id);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    this.productName.Text = reader.GetString("product_name");
                }
                conn.Close();
                SucceedLogCreate("retreive product name");
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }


        private Boolean ValidateInput(String text)
        {
            ///validate all input for not null or whitespace
            Boolean cflag = true;
            if (string.IsNullOrWhiteSpace(text) == true)
            {
                cflag = false;
            }
            return cflag;
        }

        private String getLatestRow()
        {
            ///get latest row number
            String rowCount = "";
            try
            {
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
                String sql_count = "SELECT COUNT(*) FROM command_card";
                cmd = new MySqlCommand(sql_count, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                rowCount = reader.GetInt64(0).ToString();
                conn.Close();
                SucceedLogCreate("retreive last row");
            }
            catch (Exception e)
            {
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                ErrorLogCreate(e);
            }
            return rowCount;

        }

        private byte[] EncoderUTF(String text)
        {
            ///translate normal input to utf8
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encoded = utf8.GetBytes(text);
            return encoded;

        }

        public void RecordCommandCard()
        {
            ///insert new card
            try
            {
                CultureInfo ci = new CultureInfo("en-US");
                String cardCode = this.printingCode.Text + "," + this.blowingCode.Text + "," + this.cuttingCode.Text + "," + this.matCode.Text;
                String customerId = this.customerList.SelectedValue.ToString();
                String productId = this.productList.SelectedValue.ToString();
                String receiveCode = this.paperNumber.Text;
                String produceAmount = this.paperReqAmount.Text;
                String realAmount = this.paperRealAmount.Text;

                DateTime finishDateTime = (DateTime)this.finishDate.SelectedDate;
                String finishDate = finishDateTime.ToString("dd/MM/yyyy", ci);
                String customerCode = this.customerList.SelectedValue.ToString();
                String productCode = this.productList.SelectedValue.ToString();
                String paperDetail = this.printingPaperDetail.Text + "," + this.blowingPaperDetail.Text + "," + this.cuttingPaperDetail.Text;
                String currentYear = DateTime.Now.ToString("yy", ci);
                String currentDate = DateTime.Now.ToString("dd/MM/yyyy", ci);
                Boolean checkAll = ValidateInput(cardCode) && ValidateInput(customerId) && ValidateInput(productId) &&
                                    ValidateInput(this.printingCode.Text) && ValidateInput(this.blowingCode.Text) && ValidateInput(this.cuttingCode.Text) &&
                                    ValidateInput(this.matCode.Text) && ValidateInput(receiveCode) && ValidateInput(produceAmount) && ValidateInput(realAmount) &&
                                   ValidateInput(finishDate) && ValidateInput(customerCode) && ValidateInput(productCode);


                if (checkAll == true)
                {
                    int nextRow = int.Parse(getLatestRow()) + 1;
                    conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                    conn.Open();
                    String sql_rec = "INSERT INTO command_card (rowid, cardCode, customerId, " +
                                     "productId, year, receiveNumber, recordDate, finishDate, produceAmount," +
                                     "requiredAmount, cardDetail) VALUES(@rowid, @cardCode," +
                                     "@customerId, @productId, @year, @receiveNumber, @recordDate, " +
                                     "@finishDate, @produceAmount, @requiredAmount, @cardDetail)";

                    cmd = new MySqlCommand(sql_rec, conn);
                    cmd.Parameters.AddWithValue("@rowid", nextRow);
                    cmd.Parameters.AddWithValue("@cardCode", EncoderUTF(cardCode));
                    cmd.Parameters.AddWithValue("@customerId", EncoderUTF(customerCode));
                    cmd.Parameters.AddWithValue("@productId", EncoderUTF(productCode));
                    cmd.Parameters.AddWithValue("@year", EncoderUTF(currentYear));
                    cmd.Parameters.AddWithValue("@receiveNumber", EncoderUTF(receiveCode));
                    cmd.Parameters.AddWithValue("@recordDate", EncoderUTF(currentDate));
                    cmd.Parameters.AddWithValue("@finishDate", EncoderUTF(finishDate));
                    cmd.Parameters.AddWithValue("@produceAmount", produceAmount);
                    cmd.Parameters.AddWithValue("@requiredAmount", realAmount);
                    cmd.Parameters.AddWithValue("@cardDetail", EncoderUTF(paperDetail));
                    cmd.ExecuteNonQuery();
                    recPass1 = true;
                    recPass2 = true;
                    recPass3 = true;
                    MessageBox.Show("บันทึกข้อมูลใบคำสั่งแล้ว", "สถานะการบันทึก");
                    conn.Close();
                    SucceedLogCreate("record command card data");
                    LockScreen("clear");
                }
                else
                {
                    throw new Exception("Form is not complete!");
                }
            }
            catch (Exception xr)
            {
                conn.Close();
                ErrorLogCreate(xr);
                MessageBox.Show("ใส่ข้อมูลที่จำเป็นไม่ครบ,ไม่ถูกต้องหรือมีรหัสนี้ในระบบแล้ว", "สถานะการบันทึก");
            }
        }

        private void recBtn_Click(object sender, RoutedEventArgs e)
        {
            RecordCommandCard();
            LockScreen("clear");

        }

        private void customerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (recPass1 == false)
            {
                GetCustomerName(this.customerList.SelectedValue.ToString());
            }
            else
            {
                recPass1 = false;
            }
        }

        private void productList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (recPass2 == false)
            {
                GetProductName(this.productList.SelectedValue.ToString());
            }
            else
            {
                recPass2 = false;
            }
        }
    }
}
