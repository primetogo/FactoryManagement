using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Threading;

namespace Factory_Manager
{
    /// <summary>
    /// Interaction logic for CommandList.xaml
    /// </summary>
    public partial class CommandList : Window
    {
        MySqlConnection conn = new MySqlConnection();
        MySqlCommand cmd;
        MySqlDataReader reader;
        public CommandList()
        {
            InitializeComponent();
        }

        private void CheckStateDB()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
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

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = (dynamic)commandLst.SelectedItems[0];



                var orderNumber = selected.Col1;
                var receiveNumber = selected.Col2;
                var recordDate = selected.Col3;
                var finishDate = selected.Col4;
                var customerName = selected.Col5;
                var productName = selected.Col6;
                List<String> cardData = new List<String>();
                cardData.Add(orderNumber);
                cardData.Add(receiveNumber);
                cardData.Add(recordDate);
                cardData.Add(finishDate);
                cardData.Add(customerName);
                cardData.Add(productName);
                CultureInfo ci = new CultureInfo("en-US");

                CheckStateDB();
                String[] order = cardData[0].Split('-');
                String getCardData = "SELECT cardCode, customerId, cardDetail, productId, produceAmount, requiredAmount FROM command_card" +
                " WHERE rowid = @id AND year = @year";
                cmd = new MySqlCommand(getCardData, conn);
                cmd.Parameters.AddWithValue("@id", order[1].TrimStart('0'));
                cmd.Parameters.AddWithValue("@year", order[0]);
                reader = cmd.ExecuteReader();
                reader.Read();

                String[] allCode = reader.GetString("cardCode").Split(',');
                String[] allDetail = reader.GetString("cardDetail").Split(',');

                List<Object> allData = new List<Object>();
                allData.Add(cardData[0]);
                allData.Add(allCode[0]);
                allData.Add(allCode[1]);
                allData.Add(allCode[2]);
                allData.Add(allCode[3]);
                allData.Add(reader.GetString("customerId"));
                allData.Add(reader.GetString("productId"));
                allData.Add(cardData[4]);
                allData.Add(cardData[5]);
                allData.Add(cardData[3]);
                allData.Add(cardData[1]);
                allData.Add(reader.GetString("requiredAmount"));
                allData.Add(reader.GetString("produceAmount"));
                allData.Add(allDetail[0]);
                allData.Add(allDetail[1]);
                allData.Add(allDetail[2]);

                Application.Current.Properties["cardExportData"] = allData;


                reader.Close();



                CommandEdit ses = new CommandEdit();
                ses.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                ErrorLogCreate(ex);
            }
        }
    }
}
