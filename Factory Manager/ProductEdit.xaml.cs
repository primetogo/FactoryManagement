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
    /// Interaction logic for ProductEdit.xaml
    /// </summary>
    public partial class ProductEdit : Window
    {
        MySqlConnection conn = new MySqlConnection();
        MySqlCommand cmd;
        MySqlDataReader reader;
        public ProductEdit()
        {
            InitializeComponent();
            LockWindow(false);
            RetreiveProduct();
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
        private void RetreiveProduct()
        {
            ///Get product list from database
            try
            {
                CheckStateDB();
                String sqlGet = "SELECT product_id FROM product";
                cmd = new MySqlCommand(sqlGet, conn);
                reader = cmd.ExecuteReader();
                if (reader.HasRows == false)
                {
                    throw new Exception("No row were found!");
                }
                else
                {
                    while (reader.Read())
                    {
                        this.prodList.Items.Add(reader.GetString("product_id"));
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                ErrorLogCreate(ex);
                LockWindow(true);
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

        private void RetreiveProductData()
        {
            ///Get product data from productID
            try
            {
                String iDD = this.prodList.SelectedValue.ToString();
                CheckStateDB();
                String sql_ret = "SELECT product_name, product_type, unit, product_weight, recipe_id, productRecord,"+
                                 "cutSize, printSize, blowSize, printFrontColor, printBackColor, printFrontAmount, "+
                                 "printBackAmount FROM product WHERE product_id = @prodID";
                cmd = new MySqlCommand(sql_ret, conn);
                cmd.Parameters.AddWithValue("@prodID", iDD);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    this.productName.Text = reader.GetString("product_name");
                    this.productType.Text = reader.GetString("product_type");
                    this.productUnit.Text = reader.GetString("unit");
                    this.productWeight.Text = reader.GetString("product_weight");
                    this.productDetail.Text = reader.GetString("productRecord");
                    this.recipeList.SelectedValue = reader.GetString("recipe_id");
                    this.cutSize.Text = reader.GetString("cutSize");
                    this.blowSize.Text = reader.GetString("blowSize");
                    this.printSize.Text = reader.GetString("printSize");
                    this.frontColor.Text = reader.GetString("printFrontColor");
                    this.backColor.Text = reader.GetString("printBackColor");
                    this.frontAmount.Text = reader.GetString("printFrontAmount");
                    this.backAmount.Text = reader.GetString("printBackAmount");
                }
                reader.Close();
            }
            catch (Exception xa)
            {
                ErrorLogCreate(xa);
                LockWindow(true);
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
                    while (reader.Read())
                    {
                        this.recipeList.Items.Add(reader.GetString("recipe_id"));
                    }
                    
                }
                reader.Close();
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                LockWindow(true);
            }
        }

        private void LockWindow(Boolean locking)
        {
            ///use for lock whole windows
            if (locking == true)
            {
                this.prodList.Items.Clear();
                this.prodList.Items.Add("ไม่มีข้อมูลผลิตภัณฑ์หรือสูตรในระบบ");
                this.prodList.SelectedIndex = 0;
                this.prodList.IsEnabled = false;
                this.productName.IsEnabled = false;
                this.productType.IsEnabled = false;
                this.productDetail.IsEnabled = false;
                this.productUnit.IsEnabled = false;
                this.productWeight.IsEnabled = false;
                this.recipeList.Items.Clear();
                this.recipeList.SelectedIndex = -1;
                this.recipeList.IsEnabled = false;
                this.cutSize.IsEnabled = false;
                this.cutSize.Clear();
                this.blowSize.IsEnabled = false;
                this.blowSize.Clear();
                this.printSize.IsEnabled = false;
                this.printSize.Clear();
                this.frontAmount.IsEnabled = false;
                this.frontAmount.Clear();
                this.frontColor.IsEnabled = false;
                this.frontColor.Clear();
                this.backAmount.IsEnabled = false;
                this.backAmount.Clear();
                this.backColor.IsEnabled = false;
                this.backColor.Clear();
            }
            else
            {
                this.prodList.Items.Clear();
                this.prodList.IsEnabled = true;
                this.prodList.SelectedIndex = -1;
                this.productName.Clear();
                this.productName.IsEnabled = true;
                this.productType.Clear();
                this.productType.IsEnabled = true;
                this.productDetail.Clear();
                this.productDetail.IsEnabled = true;
                this.productDetail.Clear();
                this.productUnit.Clear();
                this.productUnit.IsEnabled = true;
                this.productWeight.Clear();
                this.productWeight.IsEnabled = true;
                this.recipeList.Items.Clear();
                this.recipeList.IsEnabled = true;
                this.cutSize.IsEnabled = true;
                this.cutSize.Clear();
                this.blowSize.IsEnabled = true;
                this.blowSize.Clear();
                this.printSize.IsEnabled = true;
                this.printSize.Clear();
                this.frontAmount.IsEnabled = true;
                this.frontAmount.Clear();
                this.frontColor.IsEnabled = true;
                this.frontColor.Clear();
                this.backAmount.IsEnabled = true;
                this.backAmount.Clear();
                this.backColor.IsEnabled = true;
                this.backColor.Clear();
            }
        }

        public Boolean ValidateInput(String inputy)
        {
            Boolean cflag = true;
            ///validate all input not to be null or whitespace
            if (string.IsNullOrWhiteSpace(inputy) == true)
            {
                cflag = false;
            }
            return cflag;
        }

        private byte[] EncoderUTF(String text)
        {
            ///translate normal input to utf8
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encoded = utf8.GetBytes(text);
            return encoded;

        }

        private void RecordRecipeData()
        {
            ///Recording all recipe data to database
            try
            {
                String prodCode = this.prodList.SelectedValue.ToString();
                String prodName = this.productName.Text;
                String prodType = this.productType.Text;
                String prodUnit = this.productUnit.Text;
                String prodWeight = this.productWeight.Text;
                String prodDetail = this.productDetail.Text;
                String recipeId = this.recipeList.SelectedValue.ToString();
                String cutSizeValue = this.cutSize.Text;
                String blowSizeValue = this.blowSize.Text;
                String printSizeValue = this.printSize.Text;
                String frontColorValue = this.frontColor.Text;
                String backColorValue = this.backColor.Text;
                String frontAmountValue = this.frontAmount.Text;
                String backAmountValue = this.backAmount.Text;

                Boolean checkerProdCode = ValidateInput(prodCode), checkerProdName = ValidateInput(prodName),
                        checkerProdType = ValidateInput(prodType), checkerProdUnit = ValidateInput(prodUnit),
                        checkerProdWeight = ValidateInput(prodWeight), checkerRecipeId = ValidateInput(recipeId);
                Boolean allCheck = checkerProdCode && checkerProdName && checkerProdType && checkerProdUnit &&
                                   checkerProdWeight && checkerRecipeId && ValidateInput(cutSizeValue) &&
                                   ValidateInput(blowSizeValue) && ValidateInput(printSizeValue) && ValidateInput(frontColorValue) &&
                                   ValidateInput(backColorValue) && ValidateInput(frontAmountValue) && ValidateInput(backAmountValue);
                if (allCheck == false)
                {
                    throw new Exception("Form is not complete!");
                }

                CheckStateDB();
                String sqlUpdate = "UPDATE product SET product_type=@product_type, product_name=@product_name, unit=@unit, product_weight=@product_weight, recipe_id=@recipe_id, productRecord=@spec_size, "+
                    "cutSize=@cutsize, printSize=@printsize, blowSize=@blowsize, printFrontColor=@printfrontcolor, "+
                    "printBackColor=@printbackcolor, printFrontAmount=@printfrontamount, printBackAmount=@printbackamount WHERE product_id=@product_id";
                cmd = new MySqlCommand(sqlUpdate, conn);
                cmd.Parameters.AddWithValue("@product_type", EncoderUTF(prodType));
                cmd.Parameters.AddWithValue("@product_name", EncoderUTF(prodName));
                cmd.Parameters.AddWithValue("@unit", EncoderUTF(prodUnit));
                cmd.Parameters.AddWithValue("@product_weight", EncoderUTF(prodWeight));
                cmd.Parameters.AddWithValue("@recipe_id", EncoderUTF(recipeId));
                cmd.Parameters.AddWithValue("@product_id", EncoderUTF(prodCode));
                cmd.Parameters.AddWithValue("@spec_size", EncoderUTF(prodDetail));
                cmd.Parameters.AddWithValue("@cutsize", EncoderUTF(cutSizeValue));
                cmd.Parameters.AddWithValue("@blowsize", EncoderUTF(blowSizeValue));
                cmd.Parameters.AddWithValue("@printsize", EncoderUTF(printSizeValue));
                cmd.Parameters.AddWithValue("@printfrontcolor", EncoderUTF(frontColorValue));
                cmd.Parameters.AddWithValue("@printbackcolor", EncoderUTF(backColorValue));
                cmd.Parameters.AddWithValue("@printbackamount", EncoderUTF(backAmountValue));
                cmd.Parameters.AddWithValue("@printfrontamount", EncoderUTF(frontAmountValue));
                cmd.ExecuteNonQuery();
                LockWindow(false);
                RetreiveProduct();
                RetreiveRecipe();
                MessageBox.Show("ทำการบันทึกข้อมูลผลิตภัณฑ์เรียบร้อยแล้ว", "สถานะการบันทึก");

            }
            catch (Exception rr)
            {
                ErrorLogCreate(rr);
                MessageBox.Show("กรอกข้อมูลที่จำเป็นไม่ครบ ", "สถานะการบันทึก");
            }
        }

        private object EncoderUTF(TextBox productUnit)
        {
            throw new NotImplementedException();
        }

        private void DeleteProduct()
        {
            ///Delete selected product
            try
            {
                CheckStateDB();
                String del = "DELETE FROM product WHERE product_id = @prodid";
                cmd = new MySqlCommand(del, conn);
                cmd.Parameters.AddWithValue("@prodid", this.prodList.SelectedValue.ToString());
                cmd.ExecuteNonQuery();
                LockWindow(false);
                MessageBox.Show("ลบข้อมูลผลิตภัณฑ์เรียบร้อยแล้ว", "สถานะการบันทึก");
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
        private void recBtn_Click(object sender, RoutedEventArgs e)
        {
            RecordRecipeData();
            LockWindow(false);
            RetreiveProduct();
        }

        private void delBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("ต้องการจะลบข้อมูลผลิตภัณฑ์รหัส " + this.prodList.SelectedValue.ToString() + " หรือไม่?", "Factory Manager 2015: ลบข้อมูลผลิตภัณฑ์", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                DeleteProduct();
                RetreiveProduct();
            }
        }

        private void prodList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.prodList.SelectedIndex != -1 && ((String)this.prodList.SelectedValue) != "ไม่มีข้อมูลผลิตภัณฑ์ในระบบ")
            {
                RetreiveProductData();
            }

        }
    }
}
