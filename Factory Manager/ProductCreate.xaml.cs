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
    /// Interaction logic for ProductCreate.xaml
    /// </summary>
    public partial class ProductCreate : Window
    {
        MySqlConnection conn = new MySqlConnection();
        MySqlCommand cmd;
        MySqlDataReader reader;
        public ProductCreate()
        {
            InitializeComponent();
            RetreiveRecipe();
        }

        private void CheckStateDB()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
            }
        }

        private void RetreiveRecipe()
        {
            ///Retreive recipe list from database
            try
            {
                CheckStateDB();
                String sqlGetRecipe = "SELECT recipe_id FROM recipe";
                cmd = new MySqlCommand(sqlGetRecipe, conn);
                reader = cmd.ExecuteReader();
                if (reader.HasRows == false)
                {
                    throw new Exception("No row were found!");
                }
                else
                {
                    this.recipeList.Items.Clear();
                    this.productCode.IsEnabled = true;
                    this.productName.IsEnabled = true;
                    this.productType.IsEnabled = true;
                    this.productUnit.IsEnabled = true;
                    this.productWeight.IsEnabled = true;
                    this.productDetail.IsEnabled = true;
                    this.recipeList.IsEnabled = true;
                }
                while (reader.Read())
                {
                    this.recipeList.Items.Add(reader.GetString("recipe_id"));
                }
                reader.Close();
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                this.recipeList.Items.Clear();
                this.productCode.IsEnabled = false;
                this.productName.IsEnabled = false;
                this.productType.IsEnabled = false;
                this.productUnit.IsEnabled = false;
                this.productWeight.IsEnabled = false;
                this.productDetail.IsEnabled = false;
                this.recipeList.Items.Add("ไม่มีข้อมูลสูตรการผลิต");
                this.recipeList.SelectedIndex = 0;
                this.recipeList.IsEnabled = false;
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

        private String ValidateInput(String rawInput)
        {
            ///check if input is blank or something
            if (string.IsNullOrEmpty(rawInput))
            {
                rawInput = null;
            }
            return rawInput;

        }

        private byte[] EncoderUTF(String text)
        {
            ///translate normal input to utf8
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encoded = utf8.GetBytes(text);
            return encoded;

        }

        private void RecordProductData()
        {
            ///Record new product data
            String prodCode = this.productCode.Text;
            String prodType = this.productType.Text;
            String prodName = this.productName.Text;
            String prodUnit = this.productUnit.Text;
            String prodWeight = this.productWeight.Text;
            String recipe = (String)this.recipeList.SelectedValue;
            String spec = this.productDetail.Text;
            String cutSizeValue = this.cutSize.Text;
            String blowSizeValue = this.blowSize.Text;
            String printSizeValue = this.printSize.Text;
            String frontColorValue = this.frontColor.Text;
            String backColorValue = this.backColor.Text;
            String frontAmountValue = this.frontAmount.Text;
            String backAmountValue = this.backAmount.Text;


            prodName = ValidateInput(prodName);
            prodCode = ValidateInput(prodCode);
            prodType = ValidateInput(prodType);
            prodWeight = ValidateInput(prodWeight);
            prodUnit = ValidateInput(prodUnit);
            recipe = ValidateInput(recipe);
            spec = ValidateInput(spec);
            cutSizeValue = ValidateInput(cutSizeValue);
            blowSizeValue = ValidateInput(blowSizeValue);
            printSizeValue = ValidateInput(printSizeValue);
            frontColorValue = ValidateInput(frontColorValue);
            backColorValue = ValidateInput(backColorValue);
            frontAmountValue = ValidateInput(frontAmountValue);
            backAmountValue = ValidateInput(backAmountValue);
            try
            {
                CheckStateDB();
                String sql_insert = "INSERT INTO product (product_id, product_type, product_name, unit, product_weight," +
                                    "recipe_id, productRecord, cutSize, printSize, blowSize, printFrontColor, "+
                                    "printBackColor, printFrontAmount, printBackAmount) VALUES(@product_id, @product_type, @product_name, @unit," +
                                    "@product_weight, @recipe_id, @spec_size, @cutsize, @printsize, @blowsize, @printfrontcolor, @printbackcolor, "+
                                    "@printfrontamount, @printbackamount)";
                cmd = new MySqlCommand(sql_insert, conn);
                cmd.Parameters.AddWithValue("@product_id", EncoderUTF(prodCode));
                cmd.Parameters.AddWithValue("@product_type", EncoderUTF(prodType));
                cmd.Parameters.AddWithValue("@product_name", EncoderUTF(prodName));
                cmd.Parameters.AddWithValue("@unit", EncoderUTF(prodUnit));
                cmd.Parameters.AddWithValue("@product_weight", EncoderUTF(prodWeight));
                cmd.Parameters.AddWithValue("@recipe_id", EncoderUTF(recipe));
                cmd.Parameters.AddWithValue("@spec_size", EncoderUTF(spec));
                cmd.Parameters.AddWithValue("@cutsize", EncoderUTF(cutSizeValue));
                cmd.Parameters.AddWithValue("@printsize", EncoderUTF(printSizeValue));
                cmd.Parameters.AddWithValue("@blowsize", EncoderUTF(blowSizeValue));
                cmd.Parameters.AddWithValue("@printfrontcolor", EncoderUTF(frontColorValue));
                cmd.Parameters.AddWithValue("@printbackcolor", EncoderUTF(backColorValue));
                cmd.Parameters.AddWithValue("@printfrontamount", EncoderUTF(frontAmountValue));
                cmd.Parameters.AddWithValue("@printbackamount", EncoderUTF(backAmountValue));
                cmd.ExecuteNonQuery();
                
                MessageBox.Show("ทำการบันทึกข้อมูลผลิตภัณฑ์เรียบร้อยแล้ว", "สถานะการบันทึก");
            }
            catch (Exception tron)
            {
                
                ErrorLogCreate(tron);
                MessageBox.Show("กรอกข้อมูลที่จำเป็นไม่ครบหรือมีรหัสผลิตภัณฑ์นี้แล้ว", "สถานะการบันทึก");
                
            }
        }

        private void ClearScreen()
        {
            ///Clear every input thingy out
            this.productCode.Clear();
            this.productName.Clear();
            this.productType.Clear();
            this.productDetail.Clear();
            this.productUnit.Clear();
            this.productWeight.Clear();
            this.recipeList.SelectedIndex = -1;
            this.printSize.Clear();
            this.blowSize.Clear();
            this.cutSize.Clear();
            this.frontAmount.Clear();
            this.backAmount.Clear();
            this.frontColor.Clear();
            this.backColor.Clear();
        }

        private void recBtn_Click(object sender, RoutedEventArgs e)
        {
            RecordProductData();
            ClearScreen();
        }     
    }
}
