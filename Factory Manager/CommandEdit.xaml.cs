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
    /// Interaction logic for CommandEdit.xaml
    /// </summary>
    public partial class CommandEdit : Window
    {
        MySqlConnection conn = new MySqlConnection();
        MySqlCommand cmd;
        MySqlDataReader reader;
        Boolean recPass1 = false, recPass2 = false, terminater = false;
        String[] currentNumber = new String[4];
        public CommandEdit()
        {
            InitializeComponent();

            this.finishDate.DisplayDateStart = DateTime.Now;
            RetreiveCustomer();
            RetreiveProduct();
            SetUpdateData();

        }

        private void CheckStateDB()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
            }
        }


        public void RetreiveCustomer()
        {
            ///Retrieve customer data from database to add in combobox
            try
            {
                CheckStateDB();
                String sql_get = "SELECT customer_id FROM customer";
                cmd = new MySqlCommand(sql_get, conn);
                reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        this.customerList.Items.Add(reader.GetString("customer_id"));
                    }
                    SucceedLogCreate("retreive customer data");
                }
                else
                {
                    throw new Exception("No row were found!");
                }
                reader.Close();
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                LockScreen("lock");
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

        private void SucceedLogCreate(String act)
        {
            CultureInfo ci = new CultureInfo("en-US");
            String currentDate = DateTime.Now.ToString("dd-MM-yyyy", ci);
            String currentDateAndTime = DateTime.Now.ToString("dd-MM-yyyy H:mm:ss", ci);
            string path = @"C:\FMmanagement-Log\Log Date " + currentDate + ".txt";
            if (!File.Exists(path))
            {
                File.Create(path);
                TextWriter tw = new StreamWriter(path);
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

        private void SetUpdateData()
        {

            if (Application.Current.Properties["cardExportData"] != null)
            {

                List<Object> data = (List<Object>)Application.Current.Properties["cardExportData"];
                String orderNumber = data[0].ToString(),
                    printCode = data[1].ToString(),
                    blowCode = data[2].ToString(),
                    cutCode = data[3].ToString(),
                    matCoder = data[4].ToString(),
                    customerId = data[5].ToString(),
                    productId = data[6].ToString(),
                    customer = data[7].ToString(),
                    product = data[8].ToString(),
                    receiveNumber = data[10].ToString(),
                    requiredAmount = data[11].ToString(),
                    produceAmount = data[12].ToString(),
                    printDetail = data[13].ToString(),
                    codeDetail = data[14].ToString(),
                    blowDetail = data[15].ToString();

                String finish = (String)data[9];
                String[] finishDate1 = finish.Split('/');
                this.finishDate.SelectedDate = new DateTime(int.Parse(finishDate1[2]), int.Parse(finishDate1[1]), int.Parse(finishDate1[0]));
                this.orderNumber.Text = orderNumber;
                this.printingCode.Text = printCode;
                this.blowingCode.Text = blowCode;
                this.cuttingCode.Text = cutCode;
                this.matCode.Text = matCoder;
                this.customerList.SelectedIndex = this.customerList.Items.IndexOf(customerId);
                this.productList.SelectedIndex = this.productList.Items.IndexOf(productId);
                this.paperNumber.Text = receiveNumber;
                this.paperReqAmount.Text = requiredAmount;
                this.paperRealAmount.Text = produceAmount;
                this.printingPaperDetail.Text = printDetail;
                this.blowingPaperDetail.Text = blowDetail;
                this.cuttingPaperDetail.Text = codeDetail;
                this.deleteBtn.IsEnabled = true;
                this.recBtn.IsEnabled = true;
                Application.Current.Properties["cardExportData"] = null;
            }
        }

        private void DeleteCommand()
        {
            try
            {
                ///use this to delete card
                CheckStateDB();
                String delete = "DELETE FROM command_card WHERE year=@year AND rowid=@id";
                String[] cardCode = this.orderNumber.Text.Split('-');
                cmd = new MySqlCommand(delete, conn);
                cmd.Parameters.AddWithValue("@year", cardCode[0]);
                cmd.Parameters.AddWithValue("@id", int.Parse(cardCode[1]));
                cmd.ExecuteNonQuery();
                recPass1 = true;
                recPass2 = true;
                LockScreen("clear");
                RetreiveCustomer();
                RetreiveProduct();
                SucceedLogCreate("delete command card");
                MessageBox.Show("ลบใบคำสั่งเรียบร้อยแล้ว", "สถานะการบันทึก");

            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
        private void RetreiveProduct()
        {
            ///Get product list from database
            try
            {
                CheckStateDB();
                String sqlGet = "SELECT product_id FROM product";
                cmd = new MySqlCommand(sqlGet, conn);
                reader = cmd.ExecuteReader();
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
                reader.Close();
            }
            catch (Exception ex)
            {
                ErrorLogCreate(ex);
                LockScreen("lock");
            }
        }

        private void GetCustomerName(String id)
        {
            ///get customername from custoermer id
            try
            {
                CheckStateDB();
                String sqlGet = "SELECT customer_name FROM customer WHERE customer_id = @customerId";
                cmd = new MySqlCommand(sqlGet, conn);
                cmd.Parameters.AddWithValue("@customerId", id);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    this.customerName.Text = reader.GetString("customer_name");
                }
                reader.Close();
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
                CheckStateDB();
                String sqlGet = "SELECT product_name FROM product WHERE product_id = @cusid";
                cmd = new MySqlCommand(sqlGet, conn);
                cmd.Parameters.AddWithValue("@cusid", id);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    this.productName.Text = reader.GetString("product_name");
                }
                reader.Close();

            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



        private void GetCommandData(String id)
        {
            ///get command data from database
            try
            {
                String[] codeIn = id.Split('-');
                CheckStateDB();
                String cusId = "";
                String prodId = "";
                String getData = "SELECT customerId, productId, receiveNumber, finishDate, produceAmount, cardCode, " +
                    "requiredAmount, cardDetail FROM command_card WHERE year=@year AND rowid=@rowid";
                cmd = new MySqlCommand(getData, conn);
                cmd.Parameters.AddWithValue("@year", codeIn[0]);
                cmd.Parameters.AddWithValue("@rowid", codeIn[1].TrimStart('0'));
                reader = cmd.ExecuteReader();
                recPass1 = true;
                recPass2 = true;

                while (reader.Read())
                {
                    this.paperNumber.Text = reader.GetString("receiveNumber");
                    this.paperRealAmount.Text = reader.GetString("produceAmount");
                    this.paperReqAmount.Text = reader.GetString("requiredAmount");
                    this.finishDate.SelectedDate = DateTime.Parse(reader.GetString("finishDate")).AddYears(543);
                    this.customerList.SelectedValue = reader.GetString("customerId");
                    cusId = reader.GetString("customerId");
                    prodId = reader.GetString("productId");
                    this.productList.SelectedValue = reader.GetString("productId");
                    String[] detail = reader.GetString("cardDetail").Split(',');
                    String[] code = reader.GetString("cardCode").Split(',');
                    this.printingCode.Text = code[0];
                    this.blowingCode.Text = code[1];
                    this.cuttingCode.Text = code[2];
                    this.matCode.Text = code[3];
                    this.printingPaperDetail.Text = detail[0];
                    this.blowingPaperDetail.Text = detail[1];
                    this.cuttingPaperDetail.Text = detail[2];
                }
                reader.Close();

                GetCustomerName(cusId);
                GetProductName(prodId);
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private byte[] EncoderUTF(String text)
        {
            ///translate normal input to utf8
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encoded = utf8.GetBytes(text);
            return encoded;

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

        private String[] CountCardOn()
        {
            String[] code = new String[4];
            try
            {
                CheckStateDB();
                int number;
                String[] prefix = { "PR", "BL", "CU", "RW" };
                CultureInfo ci = new CultureInfo("en-US");
                String currentYear = DateTime.Now.ToString("yy", ci);

                number = int.Parse(getLatestRow());
                for (int i = 0; i < 4; i++)
                {
                    code[i] = prefix[i] + currentYear + "-" + (number + 1).ToString().PadLeft(5, '0');
                }
                reader.Close();
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return code;


        }

        private String getLatestRow()
        {
            ///get latest row number
            String rowCount = "";
            try
            {
                CheckStateDB();
                String sql_count = "SELECT rowid FROM command_card ORDER BY rowid DESC LIMIT 1";
                cmd = new MySqlCommand(sql_count, conn);
                reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    reader.Read();
                    rowCount = reader.GetString("rowid");
                }
                else
                {
                    rowCount = "0";
                }
                reader.Close();
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

        private void RecordCommandData(Boolean create)
        {
            CultureInfo ci = new CultureInfo("en-US");
            String cardCode = this.printingCode.Text + "," + this.blowingCode.Text + "," + this.cuttingCode.Text + "," + this.matCode.Text;
            String customerId = this.customerList.SelectedValue.ToString();
            String productId = this.productList.SelectedValue.ToString();
            String[] runNumber = this.orderNumber.Text.Split('-');
            String receiveCode = this.paperNumber.Text;
            String produceAmount = this.paperReqAmount.Text;
            String realAmount = this.paperRealAmount.Text;
            DateTime finishDateTime = (DateTime)this.finishDate.SelectedDate;
            String finishDate = finishDateTime.ToString("dd/MM/yyyy", ci);
            String customerCode = this.customerList.SelectedValue.ToString();
            String productCode = this.productList.SelectedValue.ToString();
            String paperDetail = this.printingPaperDetail.Text + "," + this.blowingPaperDetail.Text + "," + this.cuttingPaperDetail.Text;
            Boolean checkAll = ValidateInput(cardCode) && ValidateInput(customerId) && ValidateInput(productId) &&
                                ValidateInput(this.printingCode.Text) && ValidateInput(this.blowingCode.Text) && ValidateInput(this.cuttingCode.Text) &&
                                ValidateInput(this.matCode.Text) && ValidateInput(receiveCode) && ValidateInput(produceAmount) && ValidateInput(realAmount) &&
                               ValidateInput(finishDate) && ValidateInput(customerCode) && ValidateInput(productCode) &&
                               ValidateInput(paperDetail) && ValidateInput(runNumber[0]) && ValidateInput(runNumber[1]);
            try
            {
                if (checkAll == true)
                {
                    if (create == false)
                    {

                        ///Update all command data
                        CheckStateDB();
                        String user = (String)Application.Current.Properties["user"];
                        String getData = "UPDATE command_card SET customerId=@customerId, productId=@productId, receiveNumber=@receiveNumber," +
                                         "finishDate=@finishDate, produceAmount=@produceAmount, requiredAmount=@requiredAmount, cardDetail=@cardDetail, " +
                                         "editUser = @edit WHERE year=@year AND rowid=@rowid";
                        cmd = new MySqlCommand(getData, conn);
                        cmd.Parameters.AddWithValue("@cardCode", EncoderUTF(cardCode));
                        cmd.Parameters.AddWithValue("@customerId", EncoderUTF(customerCode));
                        cmd.Parameters.AddWithValue("@productId", EncoderUTF(productCode));
                        cmd.Parameters.AddWithValue("@receiveNumber", EncoderUTF(receiveCode));
                        cmd.Parameters.AddWithValue("@finishDate", EncoderUTF(finishDate));
                        cmd.Parameters.AddWithValue("@produceAmount", realAmount);
                        cmd.Parameters.AddWithValue("@requiredAmount", produceAmount);
                        cmd.Parameters.AddWithValue("@cardDetail", EncoderUTF(paperDetail));
                        cmd.Parameters.AddWithValue("@edit", user);
                        cmd.Parameters.AddWithValue("@year", runNumber[0]);
                        cmd.Parameters.AddWithValue("@rowid", runNumber[1]);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("บันทึกข้อมูลใบคำสั่งแล้ว", "สถานะการบันทึก");

                    }
                    else
                    {
                        String user = (String)Application.Current.Properties["user"];
                        currentNumber = CountCardOn();
                        String cardNewCode = currentNumber[0] + "," + currentNumber[1] + "," + currentNumber[2] + "," + currentNumber[3];
                        String currentYear = DateTime.Now.ToString("yy", ci);
                        String currentDate = DateTime.Now.ToString("dd/MM/yyyy", ci);
                        int nextRow = int.Parse(getLatestRow()) + 1;
                        CheckStateDB();
                        String sql_rec = "INSERT INTO command_card (rowid, cardCode, customerId, " +
                                    "productId, year, receiveNumber, recordDate, finishDate, produceAmount," +
                                    "requiredAmount, cardDetail, createUser, editUser) VALUES(@rowid, @cardCode," +
                                    "@customerId, @productId, @year, @receiveNumber, @recordDate, " +
                                    "@finishDate, @produceAmount, @requiredAmount, @cardDetail, @create, @edit)";
                        cmd = new MySqlCommand(sql_rec, conn);
                        cmd.Parameters.AddWithValue("@rowid", nextRow);
                        cmd.Parameters.AddWithValue("@cardCode", EncoderUTF(cardNewCode));
                        cmd.Parameters.AddWithValue("@customerId", EncoderUTF(customerCode));
                        cmd.Parameters.AddWithValue("@productId", EncoderUTF(productCode));
                        cmd.Parameters.AddWithValue("@year", EncoderUTF(currentYear));
                        cmd.Parameters.AddWithValue("@receiveNumber", EncoderUTF(receiveCode));
                        cmd.Parameters.AddWithValue("@recordDate", EncoderUTF(currentDate));
                        cmd.Parameters.AddWithValue("@finishDate", EncoderUTF(finishDate));
                        cmd.Parameters.AddWithValue("@produceAmount", produceAmount);
                        cmd.Parameters.AddWithValue("@requiredAmount", realAmount);
                        cmd.Parameters.AddWithValue("@cardDetail", EncoderUTF(paperDetail));
                        cmd.Parameters.AddWithValue("@create", user);
                        cmd.Parameters.AddWithValue("@edit", "-");
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("สร้างข้อมูลใบคำสั่งแล้ว", "สถานะการบันทึก");
                        recPass1 = true;
                        recPass2 = true;
                        LockScreen("clear");
                        RetreiveCustomer();
                        RetreiveProduct();

                    }

                }
                else
                {
                    MessageBox.Show("กรอกข้อมูลไม่ครบถ้วน"
                                     , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception xx)
            {
                ErrorLogCreate(xx);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


        }

        private void LockScreen(String action)
        {
            ///clear screen or lock screen
            if (action == "clear")
            {
                this.orderNumber.Clear();
                this.paperNumber.Clear();
                this.paperNumber.IsEnabled = true;
                this.paperRealAmount.Clear();
                this.paperRealAmount.IsEnabled = true;
                this.paperReqAmount.Clear();
                this.paperReqAmount.IsEnabled = true;
                this.printingCode.Clear();
                this.printingCode.IsEnabled = true;
                this.blowingCode.Clear();
                this.blowingCode.IsEnabled = true;
                this.cuttingCode.Clear();
                this.cuttingCode.IsEnabled = true;
                this.matCode.Clear();
                this.matCode.IsEnabled = true;
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
                this.blowingCode.IsEnabled = false;
                this.cuttingCode.Clear();
                this.cuttingCode.IsEnabled = false;
                this.matCode.Clear();
                this.matCode.IsEnabled = false;
                this.orderNumber.Clear();
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
                this.customerList.Items.Add("ไม่มีข้อมูลใบคำสั่ง,ผลิตภัณฑ์หรือลูกค้าในระบบ");
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

        private void recBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("ต้องการแก้ไขข้อมูล job หรือไม่? หากตอบไม่จะเป็นการสร้าง job ใหม่", "Update?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                RecordCommandData(false);
            }
            else
            {
                RecordCommandData(true);
            }

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


        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("ต้องการจะลบข้อมูลใบคำสั่งรหัส " + this.orderNumber.Text + " หรือไม่?", "Factory Manager 2015: ลบข้อมูลลูกค้า", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                DeleteCommand();

            }

        }

        private String ResolveProductName(String code)
        {
            String name = "";
            try
            {
                CheckStateDB();
                String sql_name2 = "SELECT product_name FROM product WHERE product_id = @prodId";
                cmd = new MySqlCommand(sql_name2, conn);
                cmd.Parameters.AddWithValue("@prodId", code);
                reader = cmd.ExecuteReader();
                reader.Read();
                name = reader.GetString("product_name");
                reader.Close();
                SucceedLogCreate("cardList : resolve product name");
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return name;
        }

        private String ResolveCustomerName(String code)
        {
            String name = "None";
            try
            {
                CheckStateDB();
                String sql_name = "SELECT customer_name FROM customer WHERE customer_id = @id";
                cmd = new MySqlCommand(sql_name, conn);
                cmd.Parameters.AddWithValue("@id", code);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                name = reader.GetString("customer_name");
                reader.Close();
                SucceedLogCreate("cardList : resolve customer name");
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return name;
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckStateDB();
                if (string.IsNullOrEmpty(this.searchQuery.Text) || string.IsNullOrWhiteSpace(this.searchQuery.Text) == false)
                {
                    if (this.searchQuery.Text.Contains('-'))
                    {
                        String[] order = this.searchQuery.Text.Split('-');
                        String sql_data = "SELECT rowid, customerId, finishDate, recordDate, productId, year, receiveNumber FROM command_card WHERE rowid REGEXP @id AND year REGEXP @year";
                        cmd = new MySqlCommand(sql_data, conn);
                        cmd.Parameters.AddWithValue("@id", order[1].TrimStart('0'));
                        cmd.Parameters.AddWithValue("@year", order[0]);
                        reader = cmd.ExecuteReader();
                        CommandList ses = new CommandList();
                        if (reader.HasRows == false)
                        {
                            MessageBox.Show("ไม่พบข้อมูล Job ที่ค้นหา", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                            terminater = true;
                        }
                        List<String> orderNumber = new List<String>();
                        List<String> receiveNumber = new List<String>();
                        List<String> recordDate = new List<String>();
                        List<String> finishDate = new List<String>();
                        List<String> customerId = new List<String>();
                        List<String> productId = new List<String>();
                        while (reader.Read())
                        {
                            orderNumber.Add(reader.GetString("year") + "-" + reader.GetString("rowid").PadLeft(4, '0'));
                            receiveNumber.Add(reader.GetString("receiveNumber"));
                            recordDate.Add(reader.GetString("recordDate"));
                            finishDate.Add(reader.GetString("finishDate"));
                            customerId.Add(reader.GetString("customerId"));
                            productId.Add(reader.GetString("productId"));
                        }
                        reader.Close();
                        for (int i = 0; i < orderNumber.Count(); i++)
                        {
                            ses.commandLst.Items.Add(new
                            {
                                Col1 = orderNumber[i],
                                Col2 = receiveNumber[i],
                                Col3 = recordDate[i],
                                Col4 = finishDate[i],
                                Col5 = ResolveCustomerName(customerId[i]),
                                Col6 = ResolveProductName(productId[i])
                            });
                        }
                        if (terminater == false)
                        {
                            this.Close();
                            ses.Show();

                        }
                    }
                    else
                    {
                        MessageBox.Show("ไม่พบข้อมูล Job ที่ค้นหา", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("กรุณากรอกลำดับสั่งในช่องค้นหา", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }
            catch (Exception ex)
            {
                ErrorLogCreate(ex);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void paperReqAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            int test;
            if (string.IsNullOrEmpty(this.paperReqAmount.Text) == false || string.IsNullOrWhiteSpace(this.paperReqAmount.Text) == false)
            {
                if (int.TryParse(this.paperReqAmount.Text, out test) == false)
                {
                    MessageBox.Show("กรุณาใส่ข้อมูลที่เป็นตัวเลข", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                    this.paperReqAmount.Clear();
                }
            }   
        }

        private void paperRealAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            int test;
            if (string.IsNullOrEmpty(this.paperReqAmount.Text) == false || string.IsNullOrWhiteSpace(this.paperReqAmount.Text) == false)
            {
                if (int.TryParse(this.paperRealAmount.Text, out test) == false)
                {
                    MessageBox.Show("กรุณาใส่ข้อมูลที่เป็นตัวเลข", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                    this.paperRealAmount.Clear();
                }
            }
            
        }



    }
}
