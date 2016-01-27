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
using System.Windows.Shapes;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Globalization;
using System.IO;
using System.Collections;

namespace Factory_Manager
{
    /// <summary>
    /// Interaction logic for TotalData.xaml
    /// </summary>
    public partial class TotalData : Window
    {
        MySqlConnection conn = new MySqlConnection();
        MySqlCommand cmd;

        public TotalData()
        {
            InitializeComponent();
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

        private void QuerybyCondition()
        {
            try
            {
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
                CultureInfo ci = new CultureInfo("en-US");
                String start = "", end = "";
                String[] dataSet = new String[4];
                String runYear = this.runNumberYear.Text, runNo = this.runNumberNo.Text;
                if (this.startDate.SelectedDate != null)
                {
                    DateTime startTime = (DateTime)this.startDate.SelectedDate;
                    start = startTime.ToString("dd/MM/yyyy", ci);
                    dataSet[0] = start;
                }
                if (this.finishDate.SelectedDate != null)
                {
                    DateTime finishTime = (DateTime)this.finishDate.SelectedDate;
                    end = finishTime.ToString("dd/MM/yyyy", ci);
                    dataSet[1] = end;
                }
                if ((string.IsNullOrWhiteSpace(runYear) && string.IsNullOrWhiteSpace(runNo)) == false)
                {
                    dataSet[2] = runYear;
                    dataSet[3] = runNo.TrimStart('0');
                }
                String sqlGet = "SELECT rowid, year, finishDate, recordDate, customerId, productId FROM command_card WHERE ";
                String[] searchSet = { "recordDate=@recordDate", "finishDate=@finishDate", "year=@year", "rowid=@rowid" };
                String[] paramList = { "@recordDate", "@finishDate", "@year", "@rowid" };
                String[] paramIn = new String[4];
                String part = "";

                if (dataSet.Length > 0)
                {
                    for (int i = 0; i < dataSet.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(dataSet[i]) == false)
                        {
                            part += searchSet[i];
                            paramIn[i] = paramList[i];
                            part += " ";
                        }
                    }
                    part = part.TrimEnd(' ');
                    part = part.Replace(" ", " AND ");
                    sqlGet += part;
                    cmd = new MySqlCommand(sqlGet, conn);
                    for (int j = 0; j < paramList.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(paramIn[j]) == false)
                        {
                            cmd.Parameters.AddWithValue(paramIn[j], dataSet[j]);
                        }
                    }

                    MySqlDataReader reader = cmd.ExecuteReader();
                    String currentDate = DateTime.Now.ToString("dd/MM/yyyy", ci);
                    String jobNo = "";
                    String jobProduct = "";
                    ArrayList product = new ArrayList();
                    String jobCustomer = "";
                    ArrayList customer = new ArrayList();
                    String jobStart = "";
                    String JobEnd = "";
                    while (reader.Read())
                    {
                        jobNo += reader.GetString("year") +"-" +reader.GetString("rowid").PadLeft(5, '0') + " \n ";
                        product.Add(reader.GetString("productId"));
                        customer.Add(reader.GetString("customerId"));
                        jobStart += reader.GetString("recordDate") + " \n ";
                        JobEnd += reader.GetString("finishDate") + " \n ";
                    }
                    conn.Close();
                    for (int i = 0; i < product.Count; i++)
                    {
                        jobProduct += GetProductName((String)product[i]) + "\n";
                        jobCustomer += GetCustomerName((String)customer[i]) + "\n";
                    }
                    String[] jobData = { jobNo, jobProduct, jobCustomer, jobStart, JobEnd};
                    Application.Current.Properties["dataTable"] = jobData;
                    TotalDataPreview ses = new TotalDataPreview();
                    ses.Show();

                }
                else
                {
                    conn.Close();
                    throw new Exception("No choice was selected!");
                }
            }
            catch (Exception ee)
            {
               
                conn.Close();
                ErrorLogCreate(ee);
                MessageBox.Show("ไม่มีการเลือกคำค้นหา", "ข้อผิดพลาด");
            }

        }

        private String GetCustomerName(String id)
        {
            ///get customername from custoermer id
            String Cusname = "";
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
                    Cusname = reader.GetString("customer_name");
                }
                conn.Close();
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return Cusname;
        }

        private String GetProductName(String id)
        {
            ///get product name from id
            String prodName = "";
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
                    prodName = reader.GetString("product_name");
                }
                conn.Close();
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return prodName;
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            QuerybyCondition();
        }
    }
}
