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
    /// Interaction logic for RecipeCreate.xaml
    /// </summary>
    public partial class RecipeCreate : Window
    {
        MySqlConnection conn = new MySqlConnection();
        MySqlCommand cmd, count;
        int matCount = 0, recordCount = 0;
        String mat_id = "";
        Boolean recPass = false;
        public RecipeCreate()
        {
            InitializeComponent();
            GetMaterial();
            CheckMaterialPerm();
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

        private void CheckMaterialPerm()
        {
            ///Prevent user from enter data with no material added
            if (matCount == 0)
            {
                this.materialList.Items.Clear();
                this.recipeCode.Text = "ไม่มีข้อมูลส่วนผสมในระบบ";
                this.materialList.IsEnabled = false;
                this.recipeCode.IsEnabled = false;
                this.recipeName.IsEnabled = false;
                this.recipeDetail.IsEnabled = false;
                this.recipeMaterial.IsEnabled = false;
                this.materialList.IsEnabled = false;
                this.materialName.IsEnabled = false;
                this.materialAmount.IsEnabled = false;
                this.recBtn.IsEnabled = false;
                this.delBtn.IsEnabled = false;
                this.addBtn.IsEnabled = false;
                this.clearBtn.IsEnabled = false;
            }
            else
            {
                this.recipeCode.Text = "";
                this.materialList.Items.Clear();
                this.materialList.IsEnabled = true;
                this.recipeCode.IsEnabled = true;
                this.recipeName.IsEnabled = true;
                this.recipeDetail.IsEnabled = true;
                this.recipeMaterial.IsEnabled = true;
                this.materialList.IsEnabled = true;
                this.materialName.IsEnabled = true;
                this.materialAmount.IsEnabled = true;
                this.recBtn.IsEnabled = true;
                this.delBtn.IsEnabled = true;
                this.addBtn.IsEnabled = true;
                this.clearBtn.IsEnabled = true;
                GetMaterial();
            }
        }

        private void InsertMaterial(String matId)
        {
            ///Retreive material data and insert output in name box and material box
            try { 
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
            conn.Close();
            }catch(Exception e){
                conn.Close();
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
                        throw new Exception("overload");
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
                recordCount = 1;
            }
            catch (Exception y)
            {
                ErrorLogCreate(y);
                MessageBox.Show("ปริมาณวัตถุดิบเกิน 100%", "ข้อผิดพลาด");
            }

        }

        private void CheckAmount(String amountIn)
        {
            ///Checking inserting amount data not greater than 100 or more than one decimal dot
            try
            {
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
                    else if (amountIn[i] != '.')
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

        private byte[] EncoderUTF(String text)
        {
            ///translate normal input to utf8
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encoded = utf8.GetBytes(text);
            return encoded;

        }

        private void RecordRecipeData()
        {
            ///when user click record button it will record recipe data
            try
            {
                String id = this.recipeCode.Text;
                String name = this.recipeName.Text;
                String detail = this.recipeDetail.Text;
                String recipe = this.recipeMaterial.Text;
                Boolean compare = string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name) ||
                       string.IsNullOrWhiteSpace(recipe);

                if (compare == true)
                {
                    throw new Exception("Form is not complete!");
                }

                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
                String sql_insert = "INSERT INTO recipe (recipe_id, recipe_name, detail, materialCode) VALUES(@recipe_id, @recipe_name, @detail, @materialCode)";
                cmd = new MySqlCommand(sql_insert, conn);
                cmd.Parameters.AddWithValue("@recipe_id", EncoderUTF(id));
                cmd.Parameters.AddWithValue("@recipe_name", EncoderUTF(name));
                cmd.Parameters.AddWithValue("@detail", EncoderUTF(detail));
                cmd.Parameters.AddWithValue("@materialCode", EncoderUTF(recipe));
                cmd.ExecuteNonQuery();
                recordCount = 1;
                recPass = true;
                this.recipeCode.Clear();
                this.recipeDetail.Clear();
                this.recipeMaterial.Clear();
                this.recipeName.Clear();
                this.materialList.SelectedIndex = -1;
                MessageBox.Show("ทำการบันทึกข้อมูลสูตรเรียบร้อยแล้ว", "สถานะการบันทึก");
            }
            catch (Exception xx)
            {
                ErrorLogCreate(xx);
                MessageBox.Show("กรอกข้อมูลที่จำเป็นไม่ครบหรือมีรหัสสูตรนี้แล้ว", "สถานะการบันทึก");
            }
            conn.Close();

        }

        private void materialList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mat_id = (String)this.materialList.SelectedValue;
            if (matCount > 0 || recordCount == 0)
            {
                InsertMaterial(mat_id);
            }
            else
            {
                recordCount = 0;
            }
        }

        private void recBtn_Click(object sender, RoutedEventArgs e)
        {
            RecordRecipeData();
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            String amu = this.materialAmount.Text;
            if (mat_id != "" && amu != "")
            {
                ReceiveMaterial(mat_id, amu);
            }
            else
            {
                MessageBox.Show("กรอกข้อมูลวัตถุดิบไม่ครบ!");
            }
        }

        private void materialAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            String amu = this.materialAmount.Text;
            CheckAmount(amu);
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            this.recipeMaterial.Clear();
        }

        private void delBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteMaterial();
        }




    }
}
