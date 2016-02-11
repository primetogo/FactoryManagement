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
    /// Interaction logic for RecipeEdit.xaml
    /// </summary>
    public partial class RecipeEdit : Window
    {
        MySqlConnection conn = new MySqlConnection();
        MySqlCommand cmd;
        MySqlDataReader reader;
        String recipeCoder = "";
        public RecipeEdit()
        {
            InitializeComponent();
            GetMaterial();
            setUpdateData();
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

        private void ReviewRecipeData(String recId)
        {
            ///Get recipe data from database by recipe id
            try
            {
                CheckStateDB();
                String sql_get = "SELECT recipe_name, detail, materialCode FROM recipe WHERE recipe_id = @recipe_id";
                cmd = new MySqlCommand(sql_get, conn);
                cmd.Parameters.AddWithValue("@recipe_id", recId);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    this.recipeName.Text = reader.GetString("recipe_name");
                    this.recipeDetail.Text = reader.GetString("detail");
                    this.recipeMaterial.Text = reader.GetString("materialCode");
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

        private void DeleteMaterial()
        {
            ///Delete latest material that just insert
            String matBox = this.recipeMaterial.Text;
            String[] key = matBox.Split(',');
            key = key.Take(key.Count() - 1).ToArray();
            this.recipeMaterial.Text = string.Join(", ", key);
        }

        private void CheckAmount(String amountIn)
        {
            ///Checking inserting amount data not greater than 100 or more than one decimal dot
            String amu = this.materialAmount.Text;
            int dotCount = amountIn.Split('.').Length - 1;
            if (amountIn.Trim() == "") return;
            for (int i = 0; i < amountIn.Length; i++)
            {
                if (!char.IsDigit(amountIn[i]) && amountIn[i] != '.' || dotCount > 1)
                {
                    MessageBox.Show("สามารถกรอกได้เฉพาะปริมาณที่มีจุดทศนิยมเพียงจุดเดียว", "ข้อผิดพลาด");
                    this.materialAmount.Text = "";
                    return;
                }
                else
                {
                    float amount = float.Parse(amu);
                    if (amount > 100)
                    {
                        MessageBox.Show("ใส่ได้ไม่เกิน 100%", "ข้อผิดพลาด");
                        this.materialAmount.Text = "";
                        return;
                    }
                }
            }
        }

        public void InsertMaterial(String matId)
        {
            ///Retreive material data and insert output in name box and material box
            try
            {
                CheckStateDB();
                String sql_get_data = "SELECT materialcode, materialname FROM materiallist WHERE idmaterial = @idmaterial";
                cmd = new MySqlCommand(sql_get_data, conn);
                cmd.Parameters.AddWithValue("@idmaterial", matId);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    this.materialName.Text = reader.GetString("materialcode") + "   " + reader.GetString("materialname");
                }
                //add = 1;
                reader.Close();
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void GetMaterial()
        {
            ///Get material list from database and add to combobox
            try
            {
                this.materialList.Items.Clear();
                CheckStateDB();
                String sql_get_mat = "SELECT idmaterial FROM materiallist";
                cmd = new MySqlCommand(sql_get_mat, conn);
                reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    this.materialList.IsEnabled = true;
                    while (reader.Read())
                    {
                        this.materialList.Items.Add(reader.GetString("idmaterial"));
                    }
                }
                else
                {
                    this.materialList.IsEnabled = false;
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


        private void ReceiveMaterial(String boundId, String amount)
        {
            ///Add selected material to materialbox
            try
            {
                String oldMat = this.recipeMaterial.Text;
                if (string.IsNullOrWhiteSpace(oldMat) == false)
                {
                    Decimal total = 0;
                    String[] amout = oldMat.Split(',');
                    for (int i = 0; i < amout.Length; i++)
                    {
                        String numb = amout[i];
                        String[] number = numb.Split(':');
                        Decimal catchy = Decimal.Parse(number[1]);
                        total += catchy;
                    }
                    if (total + Decimal.Parse(amount) > 100)
                    {
                        MessageBox.Show("วัตถุดิบมีปริมาณเกิน 100 %"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        oldMat += ", " + boundId + ":" + amount;
                        this.recipeMaterial.Text = oldMat;
                    }
                }
                else
                {
                    this.recipeMaterial.Text = boundId + ":" + amount;
                }
            }
            catch (Exception y)
            {
                ErrorLogCreate(y);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RecordRecipeData()
        {
            ///when user click record button it will record recipe data
            try
            {
                String id = recipeCoder;
                String name = this.recipeName.Text;
                String detail = this.recipeDetail.Text;
                String recipe = this.recipeMaterial.Text;
                Boolean inputCheck = false;
                inputCheck = string.IsNullOrWhiteSpace(name);
                inputCheck = string.IsNullOrWhiteSpace(recipe);
                inputCheck = string.IsNullOrWhiteSpace(detail);
                if (inputCheck == true)
                {
                    MessageBox.Show("กรอกข้อมูลไม่ครบหรือไม่ได้เลือกรหัสสูตร"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    CheckStateDB();
                    String sql_insert = "UPDATE recipe SET recipe_name = @recipe_name, detail = @detail, materialCode = @materialCode WHERE recipe_id=@recipe_id";
                    cmd = new MySqlCommand(sql_insert, conn);
                    cmd.Parameters.AddWithValue("@recipe_id", id);
                    cmd.Parameters.AddWithValue("@recipe_name", name);
                    cmd.Parameters.AddWithValue("@detail", detail);
                    cmd.Parameters.AddWithValue("@materialCode", recipe);
                    cmd.ExecuteNonQuery();
                    this.recipeDetail.Clear();
                    this.recipeMaterial.Clear();
                    this.recipeName.Clear();
                    MessageBox.Show("ทำการบันทึกข้อมูลสูตรเรียบร้อยแล้ว", "สถานะการบันทึก");
                    GetMaterial();
                }
            }
            catch (Exception x)
            {
                ErrorLogCreate(x);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);

            }


        }

        private void DeleteRecipe()
        {
            try
            {
                if (!(string.IsNullOrWhiteSpace(recipeCoder)))
                {
                    CheckStateDB();
                    Boolean recordFound = false;
                    String sqlTestProd = "SELECT product_name FROM product WHERE recipe_id = @rec";
                    cmd = new MySqlCommand(sqlTestProd, conn);
                    cmd.Parameters.AddWithValue("@rec", recipeCoder);
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows == true)
                    {
                        recordFound = true;
                    }
                    reader.Close();
                    if (recordFound == false)
                    {
                        String sql_del = "DELETE FROM recipe WHERE recipe_id = @recipe_id";
                        cmd = new MySqlCommand(sql_del, conn);
                        cmd.Parameters.AddWithValue("@recipe_id", recipeCoder);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("ลบข้อมูลสูตรแล้ว", "สถานะการบันทึก");
                    }
                    else
                    {
                        MessageBox.Show("ไม่สามารถลบสูตรการผลิตนี้ได้เนื่องจากมีผลิตภัณฑ์ที่ใช้สูตรนี้อยู่"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    
                }
                else
                {
                    MessageBox.Show("กรุณาเลือกรหัสสูตรก่อน", "สถานะการบันทึก");
                }
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
        }

        private void delBtn_Click(object sender, RoutedEventArgs e)
        {

            DeleteMaterial();

        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            this.recipeMaterial.Clear();
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.materialList.SelectedValue != null && string.IsNullOrEmpty(this.materialAmount.Text) == false && 
               string.IsNullOrWhiteSpace(this.materialAmount.Text) == false)
            {
                ReceiveMaterial(this.materialList.SelectedValue.ToString(), this.materialAmount.Text);
            }
            else
            {
                MessageBox.Show("กรุณาเลือกวัตถุดิบและใส่ปริมาณก่อน"
                                   , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }

        private void materialList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InsertMaterial(this.materialList.SelectedValue.ToString());
        }

        private void materialAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckAmount(this.materialAmount.Text);
        }

        private void DelRecBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(recipeCoder) == false || string.IsNullOrEmpty(recipeCoder) == false)
            {
                if (MessageBox.Show("ต้องการจะลบข้อมูลสูตรรหัส " + recipeCoder + " หรือไม่?", "Factory Manager 2015: ลบข้อมูลสูตรการผลิต", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    DeleteRecipe();
                }
            }
            else
            {
                MessageBox.Show("กรุณเลือกรหัสสูตรก่อน"
                                   , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }

        private void setUpdateData()
        {
            if (Application.Current.Properties["exportRecipe"] != null)
            {
                String code = (String)Application.Current.Properties["exportRecipe"];
                recipeCoder = code;
                ReviewRecipeData(code);
                Application.Current.Properties["exportRecipe"] = null;
            }
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(this.recipeCode.Text) == false || string.IsNullOrWhiteSpace(this.recipeCode.Text) == false)
                {
                    CheckStateDB();
                    String sqlGetRecipe = "SELECT recipe_name, detail, recipe_id FROM recipe WHERE recipe_id REGEXP @id";
                    cmd = new MySqlCommand(sqlGetRecipe, conn);
                    cmd.Parameters.AddWithValue("@id", this.recipeCode.Text);
                    reader = cmd.ExecuteReader();
                    RecipeList ses = new RecipeList();
                    if (reader.HasRows == true)
                    {
                        while (reader.Read())
                        {
                            ses.recipeLst.Items.Add(new
                            {
                                Col1 = reader.GetString("recipe_id"),
                                Col2 = reader.GetString("recipe_name"),
                                Col3 = reader.GetString("detail")
                            });
                        }
                        reader.Close();
                        ses.Show();
                        this.Close();
                    }
                    else
                    {
                        reader.Close();
                        MessageBox.Show("ไม่มีข้อมูลสูตรการผลิตบันทึกไว้"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }                  
                }
                else
                {
                    MessageBox.Show("กรุณาใส่คำค้นหา"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                ErrorLogCreate(ex);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
