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
                    MySqlDataReader reader = cmd.ExecuteReader();
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
            }catch(Exception e){
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return materialData;
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void recordBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {

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
                    String searchingRecipeId = "SELECT materialCode FROM recipe WHERE recipe_id = @id";
                    cmd = new MySqlCommand(searchingRecipeId, conn);
                    cmd.Parameters.AddWithValue("@id", recipeID);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    String materialDetail = "";
                    List<String> materialCodeList = new List<String>(), materialNameList = new List<String>();
                    List<List<String>> materialData = new List<List<String>>();
                    if (reader.HasRows == false)
                    {
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
                        for(int i = 0; i < materialCodeList.Count; i++){
                            materialResult.Items.Add(new
                            {
                                materialC = materialCodeList[i],
                                materialN = materialNameList[i]
                            });
                        }
                    }
                }else if(inputFlag == true && selectedSearchType == 1){
                    CheckStateDB();
                    String recipeName = searchQuery.Text;
                    String searchByRecipeName = "SELECT materialCode FROM recipe WHERE recipe_name = @name";
                    cmd = new MySqlCommand(searchByRecipeName, conn);
                    cmd.Parameters.AddWithValue("@name", recipeName);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    String materialDetail = "";
                    List<String> materialCodeList = new List<String>(), materialNameList = new List<String>();
                    List<List<String>> materialData = new List<List<String>>();
                    if (reader.HasRows == false)
                    {
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
            ShowSearchQueryBox();
        }







    }
}
