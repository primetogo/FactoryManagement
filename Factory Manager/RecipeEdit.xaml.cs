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
    /// Interaction logic for RecipeEdit.xaml
    /// </summary>
    public partial class RecipeEdit : Window
    {
        MySqlConnection conn = new MySqlConnection();
        MySqlCommand cmd, count;
        int matCount = 0, recipeCount = 0, review = 0, add = 0, del = 0;
        public RecipeEdit()
        {
            InitializeComponent();
            RetreiveRecipeList();
            GetMaterial();
            CheckMaterialPerm();
        }

        private void RetreiveRecipeList()
        {
            ///Get recipe list from database
            try
            {
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
                String sql_get = "SELECT recipe_id FROM recipe";
                String sql_count = "SELECT COUNT(*) FROM recipe";
                count = new MySqlCommand(sql_count, conn);
                MySqlDataReader red = count.ExecuteReader();
                red.Read();
                recipeCount = (int)red.GetInt64(0);
                red.Close();
                cmd = new MySqlCommand(sql_get, conn);
                MySqlDataReader cake = cmd.ExecuteReader();
                while (cake.Read())
                {
                    this.recipeList.Items.Add(cake.GetString("recipe_id"));
                }
                conn.Close();
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

        private void ReviewRecipeData(String recId)
        {
            ///Get recipe data from database by recipe id
            try
            {
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
                String sql_get = "SELECT recipe_name, detail, materialCode FROM recipe WHERE recipe_id = @recipe_id";
                cmd = new MySqlCommand(sql_get, conn);
                cmd.Parameters.AddWithValue("@recipe_id", recId);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    this.recipeName.Text = reader.GetString("recipe_name");
                    this.recipeDetail.Text = reader.GetString("detail");
                    this.recipeMaterial.Text = reader.GetString("materialCode");
                }
                conn.Close();
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
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
                String sql_get_data = "SELECT materialcode, materialname FROM materiallist WHERE idmaterial = @idmaterial";
                cmd = new MySqlCommand(sql_get_data, conn);
                cmd.Parameters.AddWithValue("@idmaterial", matId);
                MySqlDataReader read = cmd.ExecuteReader();
                while (read.Read())
                {
                    this.materialName.Text = read.GetString("materialcode") + "   " + read.GetString("materialname");
                }
                add = 1;
                conn.Close();
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
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
                String sql_get_mat = "SELECT idmaterial FROM materiallist";
                String sql_count = "SELECT COUNT(*) FROM materiallist";
                cmd = new MySqlCommand(sql_get_mat, conn);
                count = new MySqlCommand(sql_count, conn);
                MySqlDataReader cat = count.ExecuteReader();
                cat.Read();
                matCount = (int)cat.GetInt64(0);
                cat.Close();
                MySqlDataReader read = cmd.ExecuteReader();
                while (read.Read())
                {
                    this.materialList.Items.Add(read.GetString("idmaterial"));
                }
                if (matCount == 0)
                {
                    this.materialList.IsEnabled = false;
                }
                else
                {
                    this.materialList.IsEnabled = true;
                }
                conn.Close();
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void CheckMaterialPerm()
        {
            ///Prevent user from enter data with no material added
            if (recipeCount == 0)
            {
                this.materialList.Items.Clear();
                this.recipeList.Items.Clear();
                this.recipeList.Items.Add("ไม่มีข้อมูลสูตรบันทึกเอาไว้");
                this.recipeList.SelectedIndex = 0;
                this.recipeList.IsEnabled = false;
                this.materialList.IsEnabled = false;
                this.recipeName.IsEnabled = false;
                this.recipeName.Clear();
                this.recipeDetail.IsEnabled = false;
                this.recipeDetail.Clear();
                this.recipeMaterial.IsEnabled = false;
                this.recipeMaterial.Clear();
                this.materialList.IsEnabled = false;
                this.materialName.IsEnabled = false;
                this.materialAmount.IsEnabled = false;
                this.recBtn.IsEnabled = false;
                this.delBtn.IsEnabled = false;
                this.addBtn.IsEnabled = false;
                this.clearBtn.IsEnabled = false;
                this.DelRecBtn.IsEnabled = false;
            }
            else
            {
                this.materialList.Items.Clear();
                this.recipeList.Items.Clear();
                this.recipeList.IsEnabled = true;
                this.materialList.IsEnabled = true;
                this.recipeName.IsEnabled = true;
                this.recipeName.Clear();
                this.recipeDetail.IsEnabled = true;
                this.recipeDetail.Clear();
                this.recipeMaterial.IsEnabled = true;
                this.recipeMaterial.Clear();
                this.materialList.IsEnabled = true;
                this.materialName.IsEnabled = true;
                this.materialAmount.IsEnabled = true;
                this.recBtn.IsEnabled = true;
                this.delBtn.IsEnabled = true;
                this.addBtn.IsEnabled = true;
                this.clearBtn.IsEnabled = true;
                this.DelRecBtn.IsEnabled = true;
                GetMaterial();
                RetreiveRecipeList();
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
                        throw new Exception("Too much ingrediant amount!");
                    }
                    oldMat += ", " + boundId + ":" + amount;
                    this.recipeMaterial.Text = oldMat;
                }
                else
                {
                    this.recipeMaterial.Text = boundId + ":" + amount;
                }
                this.materialName.Clear();
                this.materialAmount.Clear();
                this.materialList.SelectedIndex = -1;
            }
            catch (Exception y)
            {
                ErrorLogCreate(y);
                MessageBox.Show("ปริมาณวัตถุดิบเกิน 100%", "ข้อผิดพลาด");
            }
        }

        private void RecordRecipeData()
        {
            ///when user click record button it will record recipe data
            try
            {
                String id = this.recipeList.SelectedValue.ToString();
                String name = this.recipeName.Text;
                String detail = this.recipeDetail.Text;
                String recipe = this.recipeMaterial.Text;
                Boolean inputCheck = false;
                inputCheck = string.IsNullOrWhiteSpace(name);
                inputCheck = string.IsNullOrWhiteSpace(recipe);
                inputCheck = string.IsNullOrWhiteSpace(detail);
                if (inputCheck == true)
                {
                    throw new Exception("Form is not complete!");
                }


                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
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
                review = 1;
                this.recipeList.SelectedIndex = -1;
                MessageBox.Show("ทำการบันทึกข้อมูลสูตรเรียบร้อยแล้ว", "สถานะการบันทึก");
            }
            catch (Exception x)
            {
                ErrorLogCreate(x);
                MessageBox.Show("กรอกข้อมูลที่จำเป็นไม่ครบ", "สถานะการบันทึก");

            }
            conn.Close();

        }

        private void DeleteRecipe()
        {
            try
            {
                if (!(string.IsNullOrWhiteSpace(this.recipeList.SelectedValue.ToString())))
                {
                    conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                    conn.Open();
                    String sql_del = "DELETE FROM recipe WHERE recipe_id = @recipe_id";
                    cmd = new MySqlCommand(sql_del, conn);
                    cmd.Parameters.AddWithValue("@recipe_id", this.recipeList.SelectedValue.ToString());
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    del = 1;
                    RetreiveRecipeList();
                    CheckMaterialPerm();

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

        private void recipeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((review == 0 || recipeCount != 0) && del == 0)
            {
                ReviewRecipeData(this.recipeList.SelectedValue.ToString());
            }
            else
            {
                del = 0;
                review = 0;
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
            ReceiveMaterial(this.materialList.SelectedValue.ToString(), this.materialAmount.Text);
        }

        private void materialList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(add > 0))
            {
                InsertMaterial(this.materialList.SelectedValue.ToString());
            }
            else
            {
                add = 0;
                matCount = 0;
            }

        }

        private void materialAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckAmount(this.materialAmount.Text);
        }

        private void DelRecBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("ต้องการจะลบข้อมูลสูตรรหัส " + this.recipeList.SelectedValue.ToString() + " หรือไม่?", "Factory Manager 2015: ลบข้อมูลสูตรการผลิต", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                DeleteRecipe();
            }
        }
    }
}
