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
    /// Interaction logic for materialCreate.xaml
    /// </summary>
    public partial class materialCreate : Window
    {
        MySqlConnection conn = new MySqlConnection();
        MySqlCommand cmd;
        MySqlDataReader reader;
        String selectedMaterialCoder = "", selectedMaterialName = "";

        public materialCreate()
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

        private void recordingMaterial(String materialCode, String materialType)
        {
            ///function for recording material data
            try
            {
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
                String recordMat = "INSERT INTO materiallist (materialcode, materialname) VALUES(@code, @type)";
                cmd = new MySqlCommand(recordMat, conn);
                cmd.Parameters.AddWithValue("@code", materialCode);
                cmd.Parameters.AddWithValue("@type", materialType);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                conn.Close();
                ErrorLogCreate(e);
            }
        }

        private Boolean ValidateInputAction()
        {
            Boolean validateFlag = false;
            validateFlag = !(string.IsNullOrEmpty(this.searchQuery.Text) || string.IsNullOrWhiteSpace(this.searchQuery.Text));
            return validateFlag;
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

        private void CheckStateDB()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
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



        private List<List<String>> ResolveMaterialName(String materialList)
        {
            List<String> materialNameList = new List<String>(), materialCodeList = new List<String>();
            List<List<String>> materialData = new List<List<string>>();
            try
            {
                CheckStateDB();
                String[] materialGroup = materialList.Split(',');
                for (int i = 0; i < materialGroup.Length; i++)
                {
                    String materialCode = materialGroup[i].Split(':')[0];
                    String getMaterialName = "SELECT materialCode, materialName FROM materiallist WHERE idmaterial = @id";
                    cmd = new MySqlCommand(getMaterialName, conn);
                    cmd.Parameters.AddWithValue("@id", materialCode);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        materialCodeList.Add(reader.GetString("materialCode"));
                        materialNameList.Add(reader.GetString("materialName"));
                    }
                    reader.Close();
                }
                materialData.Add(materialCodeList);
                materialData.Add(materialNameList);
                SucceedLogCreate("Material Data Retrieved:materialCreate");
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return materialData;
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.updateBtn.IsEnabled = true;
            this.recordBtn.IsEnabled = false;
            this.searchQuery.Clear();
            this.searchQuery.Visibility = Visibility.Hidden;
            this.typeSearch.SelectedIndex = -1;
            this.searchQueryLabel.Visibility = Visibility.Hidden;

            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                dynamic selectedItem = materialResult.SelectedItems[0];
                selectedMaterialCoder = selectedItem.materialC;
                selectedMaterialName = selectedItem.materialN;
            }
            this.materialCode.Text = selectedMaterialCoder;
            this.materialType.Text = selectedMaterialName;
        }

        private void recordBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(this.materialCode.Text) || string.IsNullOrWhiteSpace(this.materialCode.Text) ||
                string.IsNullOrWhiteSpace(this.materialType.Text) || string.IsNullOrEmpty(this.materialType.Text) == false)
                {
                    CheckStateDB();
                    String sqlUpdateMaterial = "UPDATE materiallist SET materialcode = @code, materialname = @name " +
                   "WHERE materialcode = @origincode AND materialname = @originname";
                    cmd = new MySqlCommand(sqlUpdateMaterial, conn);
                    cmd.Parameters.AddWithValue("@code", this.materialCode.Text);
                    cmd.Parameters.AddWithValue("@name", this.materialType.Text);
                    cmd.Parameters.AddWithValue("@origincode", selectedMaterialCoder);
                    cmd.Parameters.AddWithValue("@originname", selectedMaterialName);
                    cmd.ExecuteNonQuery();
                    SucceedLogCreate("Update material data:materialCreate");
                    this.materialResult.Items.Clear();
                    this.recordBtn.IsEnabled = true;
                    this.updateBtn.IsEnabled = false;
                    this.materialCode.Clear();
                    this.materialType.Clear();
                    MessageBox.Show("บันทึกข้อมูลวัตถุดิบสำเร็จแล้ว"
                   , "สถานะการบันทึก", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ErrorLogCreate(ex);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckStateDB();
                int selectedSearchType = this.typeSearch.SelectedIndex;
                Boolean inputFlag = ValidateInputAction();
                if (inputFlag == true && selectedSearchType == 0)
                {
                    String recipeID = this.searchQuery.Text;
                    String searchingRecipeId = "SELECT materialCode FROM recipe WHERE recipe_id REGEXP @id";
                    cmd = new MySqlCommand(searchingRecipeId, conn);
                    cmd.Parameters.AddWithValue("@id", recipeID);
                    reader = cmd.ExecuteReader();
                    String materialDetail = "";
                    List<String> materialCodeList = new List<String>(), materialNameList = new List<String>();
                    List<List<String>> materialData = new List<List<String>>();
                    if (reader.HasRows == false)
                    {
                        this.materialResult.Items.Clear();
                        this.materialResult.IsEnabled = false;
                        reader.Close();
                        MessageBox.Show("ไม่พบสูตรที่ค้นหา", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        this.materialResult.IsEnabled = true;
                        while (reader.Read())
                        {
                            materialDetail = reader.GetString("materialCode");
                        }
                        reader.Close();
                        materialData = ResolveMaterialName(materialDetail);
                        materialCodeList = materialData[0];
                        materialNameList = materialData[1];
                        for (int i = 0; i < materialCodeList.Count; i++)
                        {
                            materialResult.Items.Add(new
                            {
                                materialC = materialCodeList[i],
                                materialN = materialNameList[i]
                            });
                        }
                    }
                }
                else if (inputFlag == true && selectedSearchType == 1)
                {
                    CheckStateDB();
                    String recipeName = searchQuery.Text;
                    String searchByRecipeName = "SELECT materialCode FROM recipe WHERE recipe_name REGEXP @name";
                    cmd = new MySqlCommand(searchByRecipeName, conn);
                    cmd.Parameters.AddWithValue("@name", recipeName);
                    reader = cmd.ExecuteReader();
                    String materialDetail = "";
                    List<String> materialCodeList = new List<String>(), materialNameList = new List<String>();
                    List<List<String>> materialData = new List<List<String>>();
                    if (reader.HasRows == false)
                    {
                        this.materialResult.Items.Clear();
                        this.materialResult.IsEnabled = false;
                        reader.Close();
                        MessageBox.Show("ไม่พบสูตรที่ค้นหา", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        this.materialResult.IsEnabled = true;
                        while (reader.Read())
                        {
                            materialDetail = reader.GetString("materialCode");
                        }
                        reader.Close();
                        materialData = ResolveMaterialName(materialDetail);
                        materialCodeList = materialData[0];
                        materialNameList = materialData[1];
                        for (int i = 0; i < materialCodeList.Count; i++)
                        {
                            materialResult.Items.Add(new
                            {
                                materialC = materialCodeList[i],
                                materialN = materialNameList[i]
                            });
                        }
                    }

                }
                else if (inputFlag == true && selectedSearchType == 2)
                {
                    this.materialResult.Items.Clear();
                    if (this.searchQuery.Text.Contains('-'))
                    {
                        String yearCode = this.searchQuery.Text.Split('-')[0];
                        String rowId = this.searchQuery.Text.Split('-')[1];
                        CheckStateDB();
                        String sqlGetproduct = "SELECT productId FROM command_card WHERE year=@year AND rowid=@row";
                        String productId = "", recipeId = "", materialCode = "";
                        cmd = new MySqlCommand(sqlGetproduct, conn);
                        cmd.Parameters.AddWithValue("@year", yearCode);
                        cmd.Parameters.AddWithValue("@row", int.Parse(rowId.TrimStart('0')));
                        reader = cmd.ExecuteReader();
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                productId = reader.GetString("productId");
                            }
                            reader.Close();
                            CheckStateDB();
                            String sqlGetRecipe = "SELECT recipe_id FROM product WHERE product_id = @id";
                            cmd = new MySqlCommand(sqlGetRecipe, conn);
                            cmd.Parameters.AddWithValue("@id", productId);
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                recipeId = reader.GetString("recipe_id");
                            }
                            reader.Close();
                            CheckStateDB();
                            String sqlGetMaterial = "SELECT materialCode FROM recipe WHERE recipe_id = @id";
                            cmd = new MySqlCommand(sqlGetMaterial, conn);
                            cmd.Parameters.AddWithValue("@id", recipeId);
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                materialCode = reader.GetString("materialCode");
                            }
                            reader.Close();
                            List<List<String>> materialData = ResolveMaterialName(materialCode);
                            List<String> materialCoder = materialData[0];
                            List<String> materialName = materialData[1];

                            for (int k = 0; k < materialCoder.Count(); k++)
                            {
                                materialResult.Items.Add(new
                                {
                                    materialC = materialCoder[k],
                                    materialN = materialName[k]
                                });
                            }
                            this.materialResult.IsEnabled = true;

                        }
                        else
                        {
                            this.materialResult.Items.Clear();
                            this.materialResult.IsEnabled = false;
                            reader.Close();
                            MessageBox.Show("ไม่พบลำดับสั่งที่ค้นหา", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }

                    }
                    else
                    {
                        MessageBox.Show("กรุณาใส่ลำดับสั่งให้ถูกต้อง", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else if (inputFlag == true && selectedSearchType == 3)
                {
                    String materialCode = this.searchQuery.Text;
                    CheckStateDB();
                    String sqlGetMaterialName = "SELECT materialcode, materialname FROM materiallist WHERE materialcode REGEXP @code";
                    cmd = new MySqlCommand(sqlGetMaterialName, conn);
                    cmd.Parameters.AddWithValue("@code", materialCode);
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows == true)
                    {
                        this.materialResult.Items.Clear();
                        this.materialResult.IsEnabled = true;
                        while (reader.Read())
                        {
                            materialResult.Items.Add(new
                            {
                                materialC = reader.GetString("materialcode"),
                                materialN = reader.GetString("materialname")
                            });
                        }
                        reader.Close();
                    }
                    else
                    {
                        this.materialResult.Items.Clear();
                        this.materialResult.IsEnabled = false;
                        reader.Close();
                        MessageBox.Show("ไม่พบรหัสที่ค้นหา", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                }
                else if (inputFlag == true && selectedSearchType == 4)
                {
                    String materialCode = this.searchQuery.Text;
                    CheckStateDB();
                    String sqlGetMaterialName = "SELECT materialcode, materialname FROM materiallist WHERE materialname REGEXP @code";
                    cmd = new MySqlCommand(sqlGetMaterialName, conn);
                    cmd.Parameters.AddWithValue("@code", materialCode);
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows == true)
                    {
                        this.materialResult.Items.Clear();
                        this.materialResult.IsEnabled = true;
                        while (reader.Read())
                        {
                            materialResult.Items.Add(new
                            {
                                materialC = reader.GetString("materialcode"),
                                materialN = reader.GetString("materialname")
                            });
                        }
                        reader.Close();
                    }
                    else
                    {
                        this.materialResult.Items.Clear();
                        this.materialResult.IsEnabled = false;
                        reader.Close();
                        MessageBox.Show("ไม่พบชนิดที่ค้นหา", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else if (inputFlag == true && selectedSearchType == 5)
                {
                    this.materialResult.Items.Clear();
                    String order = this.searchQuery.Text;
                    CheckStateDB();
                    String sqlGetproduct = "SELECT productId FROM command_card WHERE cardCode REGEXP @order";
                    String productId = "", recipeId = "", materialCode = "";
                    cmd = new MySqlCommand(sqlGetproduct, conn);
                    cmd.Parameters.AddWithValue("@order", order);
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows == true)
                    {
                        while (reader.Read())
                        {
                            productId = reader.GetString("productId");
                        }
                        reader.Close();
                        CheckStateDB();
                        String sqlGetRecipe = "SELECT recipe_id FROM product WHERE product_id = @id";
                        cmd = new MySqlCommand(sqlGetRecipe, conn);
                        cmd.Parameters.AddWithValue("@id", productId);
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            recipeId = reader.GetString("recipe_id");
                        }
                        reader.Close();
                        CheckStateDB();
                        String sqlGetMaterial = "SELECT materialCode FROM recipe WHERE recipe_id = @id";
                        cmd = new MySqlCommand(sqlGetMaterial, conn);
                        cmd.Parameters.AddWithValue("@id", recipeId);
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            materialCode = reader.GetString("materialCode");
                        }
                        reader.Close();
                        List<List<String>> materialData = ResolveMaterialName(materialCode);
                        List<String> materialCoder = materialData[0];
                        List<String> materialName = materialData[1];

                        for (int k = 0; k < materialCoder.Count(); k++)
                        {
                            materialResult.Items.Add(new
                            {
                                materialC = materialCoder[k],
                                materialN = materialName[k]
                            });
                        }
                        this.materialResult.IsEnabled = true;

                    }
                    else
                    {
                        this.materialResult.Items.Clear();
                        this.materialResult.IsEnabled = false;
                        reader.Close();
                        MessageBox.Show("ไม่พบเลขที่ที่ค้นหา", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }



                }
                SucceedLogCreate("Retreiving material list:materialCreate");

            }
            catch (Exception ex)
            {
                ErrorLogCreate(ex);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }

        private void ShowSearchQueryBox()
        {
            if (this.typeSearch.SelectedIndex > -1)
            {
                this.searchQuery.Visibility = Visibility.Visible;
                this.searchQueryLabel.Visibility = Visibility.Visible;
            }
            else
            {
                this.searchQueryLabel.Visibility = Visibility.Hidden;
                this.searchQuery.Visibility = Visibility.Hidden;
            }
        }

        private void typeSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.typeSearch.SelectedIndex != -1)
            {
                ShowSearchQueryBox();
            }
        }







    }
}
