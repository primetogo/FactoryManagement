using MySql.Data.MySqlClient;
using System;
using System.Collections;
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

    public partial class cardList : Window
    {
        MySqlConnection conn = new MySqlConnection();
        MySqlCommand cmd;
        MySqlDataReader reader;
        public cardList()
        {
            InitializeComponent();
            RetreiveJobList();
        }

        private void CheckStateDB()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.ConnectionString = (String)Application.Current.Properties["sqlCon"];
                conn.Open();
            }
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
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(currentDateAndTime +" => "+ text.ToString());
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
                FileInfo file = new FileInfo(path);
                file.Directory.Create();
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(currentDateAndTime + " => Operation "+act+" success!");
                tw.Close();
            }
            else if (File.Exists(path))
            {
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(currentDateAndTime + " => Operation "+act+" success!");
                tw.Close();
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


        private void RetreiveJobList()
        {
            try
            {
                Boolean terminater = false;
                String queryType = (String)Application.Current.Properties["queryType"];
                String queryData = (String)Application.Current.Properties["queryData"];
                CheckStateDB();
                if (queryType.Equals("byRecordDate"))
                {                
                    String sql_data = "SELECT rowid, customerId, finishDate, recordDate, productId, year, receiveNumber FROM command_card WHERE recordDate=@rec";
                    cmd = new MySqlCommand(sql_data, conn);
                    cmd.Parameters.AddWithValue("@rec", queryData);
                    reader = cmd.ExecuteReader();
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
                    for (int i = 0; i < orderNumber.Count(); i++ )
                    {
                        jobList.Items.Add(new
                        {
                            Col1 = orderNumber[i],
                            Col2 = receiveNumber[i],
                            Col3 = recordDate[i],
                            Col4 = finishDate[i],
                            Col5 = ResolveCustomerName(customerId[i]),
                            Col6 = ResolveProductName(productId[i])
                        });
                    }
                    
                }
                else if (queryType.Equals("byFinishDate"))
                {
                    String sql_data = "SELECT rowid, customerId, finishDate, recordDate, productId, year, receiveNumber FROM command_card WHERE finishDate=@rec";
                    cmd = new MySqlCommand(sql_data, conn);
                    cmd.Parameters.AddWithValue("@rec", queryData);
                    reader = cmd.ExecuteReader();
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
                        jobList.Items.Add(new
                        {
                            Col1 = orderNumber[i],
                            Col2 = receiveNumber[i],
                            Col3 = recordDate[i],
                            Col4 = finishDate[i],
                            Col5 = ResolveCustomerName(customerId[i]),
                            Col6 = ResolveProductName(productId[i])
                        });
                    }
                }
                else if (queryType.Equals("byOrderNumber"))
                {

                    String[] row = queryData.Split('-');
                    int test = int.Parse(row[1]);                   
                    String sql_data = "SELECT rowid, customerId, finishDate, recordDate, productId, year, receiveNumber FROM command_card WHERE rowid=@rec1 AND year=@rec2";
                    cmd = new MySqlCommand(sql_data, conn);
                    cmd.Parameters.AddWithValue("@rec1", test);
                    cmd.Parameters.AddWithValue("@rec2", row[0]);
                    reader = cmd.ExecuteReader();
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
                        jobList.Items.Add(new
                        {
                            Col1 = orderNumber[i],
                            Col2 = receiveNumber[i],
                            Col3 = recordDate[i],
                            Col4 = finishDate[i],
                            Col5 = ResolveCustomerName(customerId[i]),
                            Col6 = ResolveProductName(productId[i])
                        });
                    }
                }
                else if (queryType.Equals("byReceiveNumber"))
                {
                    String sql_data = "SELECT rowid, customerId, finishDate, recordDate, productId, year, receiveNumber FROM command_card WHERE receiveNumber = @rec";
                    cmd = new MySqlCommand(sql_data, conn);
                    cmd.Parameters.AddWithValue("@rec", queryData);
                    reader = cmd.ExecuteReader();
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
                        jobList.Items.Add(new
                        {
                            Col1 = orderNumber[i],
                            Col2 = receiveNumber[i],
                            Col3 = recordDate[i],
                            Col4 = finishDate[i],
                            Col5 = ResolveCustomerName(customerId[i]),
                            Col6 = ResolveProductName(productId[i])
                        });
                    }
                }
                SucceedLogCreate("cardList : retreive job list");
                if (terminater == true)
                {
                    this.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private String GetMaterialData(String matCode)
        {
            String code = "", name = "";
            try
            {
                CheckStateDB();
                String sql_get = "SELECT materialcode, materialname FROM materiallist WHERE idmaterial = @id";
                cmd = new MySqlCommand(sql_get, conn);
                cmd.Parameters.AddWithValue("@id", matCode);
                reader = cmd.ExecuteReader();
                reader.Read();
                code = reader.GetString("materialcode");
                name = reader.GetString("materialname");
                reader.Close();
                SucceedLogCreate("cardList : get material data");
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return code + ":" + name;
        }

        private String CalculateMaterialWeight(String materialNumber, String prodWeight, String prodAmount)
        {
            float result = 0;
            try
            {
                int amount = int.Parse(prodAmount);
                float weight = float.Parse(prodWeight);
                float mat = float.Parse(materialNumber);
                result = (float)((mat / 100) * (amount * weight));
                SucceedLogCreate("cardList : calculate material weight");
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return result.ToString("0.00");
        }


        private void PrepMaterialCard(String cardId)
        {
            try
            {
                CheckStateDB();
                String[] card = cardId.Split('-');
                String sql_data = "SELECT cardCode, produceAmount, productId, cardDetail, requiredAmount FROM command_card WHERE rowid = @row AND year = @year";
                cmd = new MySqlCommand(sql_data, conn);
                cmd.Parameters.AddWithValue("@year", card[0]);
                cmd.Parameters.AddWithValue("@row", int.Parse(card[1]));
                reader = cmd.ExecuteReader();
                reader.Read();
                String cardCode = reader.GetString("cardCode"), productCode = reader.GetString("productId"),
                    cardDetail = reader.GetString("cardDetail"), prodAmount = reader.GetString("produceAmount"),
                    prodRequired = reader.GetString("requiredAmount");
                reader.Close();
                
                sql_data = "SELECT recipe_id, product_weight, unit, product_type FROM product WHERE product_id = @id";
                cmd = new MySqlCommand(sql_data, conn);
                cmd.Parameters.AddWithValue("@id", productCode);
                reader = cmd.ExecuteReader();
                reader.Read();
                String recipeId = reader.GetString("recipe_id"), prodWeight = reader.GetString("product_weight"),
                    unit = reader.GetString("unit"), productType = reader.GetString("product_type");
                reader.Close();
                
                sql_data = "SELECT materialCode FROM recipe WHERE recipe_id = @id";
                cmd = new MySqlCommand(sql_data, conn);
                cmd.Parameters.AddWithValue("@id", recipeId);
                reader = cmd.ExecuteReader();
                reader.Read();
                String materialCode = reader.GetString("materialCode");
                reader.Close();

                String[] materialCodeSplit = materialCode.Split(',');
                String[] cardRunCode = cardCode.Split(',');
                List<String> matOrder = new List<String>();
                List<String> matCode = new List<String>();
                List<String> matName = new List<String>();
                List<String> matAmount = new List<String>();
                List<String> matCalWeight = new List<String>();
                float totalAmount = 0;
                for (int k = 0; k < materialCodeSplit.Length; k++)
                {
                    String[] materialSplit = materialCodeSplit[k].Split(':');
                    matAmount.Add(float.Parse(materialSplit[1]).ToString("0.00"));
                    String[] temp = GetMaterialData(materialSplit[0]).Split(':');
                    matCode.Add(temp[0]);
                    matName.Add(temp[1]);
                    matOrder.Add((k + 1).ToString());
                    matCalWeight.Add(CalculateMaterialWeight(materialSplit[1], prodWeight, prodAmount));
                    totalAmount += float.Parse(materialSplit[1]);
                }
                List<List<String>> detailCardData = new List<List<string>>();
                detailCardData.Add(matOrder);
                detailCardData.Add(matCode);
                detailCardData.Add(matName);
                detailCardData.Add(matAmount);
                detailCardData.Add(matCalWeight);

                List<String> mainCardData = new List<String>();
                mainCardData.Add(cardRunCode[3]);
                mainCardData.Add(prodAmount);
                mainCardData.Add(prodRequired);
                mainCardData.Add(productType);
                mainCardData.Add(productCode);
                mainCardData.Add((float.Parse(prodWeight) * int.Parse(prodAmount)).ToString("0.00")); //Produce weight
                mainCardData.Add((float.Parse(prodWeight) * int.Parse(prodRequired)).ToString("0.00")); //Requied weight
                mainCardData.Add(unit);
                mainCardData.Add(totalAmount.ToString("0.00"));

                Application.Current.Properties["cardDetail"] = detailCardData;
                Application.Current.Properties["cardMainData"] = mainCardData;
                SucceedLogCreate("cardList : prepare material card data");
            }
            catch (Exception ex)
            {
                ErrorLogCreate(ex);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void PrepPrintCard(String cardId)
        {
            try
            {
                CheckStateDB();
                String[] card = cardId.Split('-');
                String sql_data = "SELECT cardCode, produceAmount, productId, cardDetail, requiredAmount FROM command_card WHERE rowid = @row AND year = @year";
                cmd = new MySqlCommand(sql_data, conn);
                cmd.Parameters.AddWithValue("@year", card[0]);
                cmd.Parameters.AddWithValue("@row", int.Parse(card[1]));
                reader = cmd.ExecuteReader();
                reader.Read();
                String cardCode = reader.GetString("cardCode"), productCode = reader.GetString("productId"),
                    cardDetail = reader.GetString("cardDetail"), prodAmount = reader.GetString("produceAmount"),
                    prodRequired = reader.GetString("requiredAmount");
                reader.Close();
                
                sql_data = "SELECT product_weight, unit, product_type, productRecord, printSize, "+
                    "printFrontColor, printBackColor, printFrontAmount, printBackAmount, product_name,"+
                    "printSize FROM product WHERE product_id = @id";
                cmd = new MySqlCommand(sql_data, conn);
                cmd.Parameters.AddWithValue("@id", productCode);
                reader = cmd.ExecuteReader();
                reader.Read();
                String prodWeight = reader.GetString("product_weight"),
                    unit = reader.GetString("unit"), productType = reader.GetString("product_type"),
                    productName = reader.GetString("product_name"), productRecord = reader.GetString("productRecord"),
                    printSize = reader.GetString("printSize"), printFrontAmount = reader.GetString("printFrontAmount"),
                    printBackAmount = reader.GetString("printBackAmount"), printFrontColor = reader.GetString("printFrontColor"),
                    printBackColor = reader.GetString("printBackColor");
                reader.Close();

                List<String> mainCardData = new List<String>();
                mainCardData.Add(cardCode.Split(',')[0]);
                mainCardData.Add(productCode);
                mainCardData.Add(cardDetail.Split(',')[0]);
                mainCardData.Add(prodAmount);
                mainCardData.Add(prodRequired);
                mainCardData.Add((float.Parse(prodWeight) * int.Parse(prodAmount)).ToString("0.00"));
                mainCardData.Add((float.Parse(prodWeight) * int.Parse(prodRequired)).ToString("0.00"));
                mainCardData.Add(productName);
                mainCardData.Add(productRecord);
                mainCardData.Add(printSize);
                mainCardData.Add(printFrontAmount);
                mainCardData.Add(printBackAmount);
                mainCardData.Add(printFrontColor);
                mainCardData.Add(printBackColor);
                mainCardData.Add(productType);
                mainCardData.Add(unit);

                Application.Current.Properties["cardMainData"] = mainCardData;
                SucceedLogCreate("cardList : prepare print card");
            }
            catch (Exception ex)
            {
            
                ErrorLogCreate(ex);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void PrepBlowCard(String cardId)
        {
            try
            {
                CheckStateDB();
                String[] card = cardId.Split('-');
                String sql_data = "SELECT cardCode, produceAmount, productId, cardDetail, requiredAmount FROM command_card WHERE rowid = @row AND year = @year";
                cmd = new MySqlCommand(sql_data, conn);
                cmd.Parameters.AddWithValue("@year", card[0]);
                cmd.Parameters.AddWithValue("@row", int.Parse(card[1]));
                reader = cmd.ExecuteReader();
                reader.Read();
                String cardCode = reader.GetString("cardCode"), productCode = reader.GetString("productId"),
                    cardDetail = reader.GetString("cardDetail"), prodAmount = reader.GetString("produceAmount"),
                    prodRequired = reader.GetString("requiredAmount");
                reader.Close();
                
                sql_data = "SELECT product_weight, unit, product_type, productRecord, " +
                    "product_name, blowSize " +
                    "FROM product WHERE product_id = @id";
                cmd = new MySqlCommand(sql_data, conn);
                cmd.Parameters.AddWithValue("@id", productCode);
                reader = cmd.ExecuteReader();
                reader.Read();
                String prodWeight = reader.GetString("product_weight"),
                    unit = reader.GetString("unit"), productType = reader.GetString("product_type"),
                    productName = reader.GetString("product_name"), productRecord = reader.GetString("productRecord"),
                    blowSize = reader.GetString("blowSize");
                reader.Close();

                List<String> mainCardData = new List<String>();
                mainCardData.Add(cardCode.Split(',')[1]);
                mainCardData.Add(productCode);
                mainCardData.Add(cardDetail.Split(',')[1]);
                mainCardData.Add(prodAmount);
                mainCardData.Add(prodRequired);
                mainCardData.Add((float.Parse(prodWeight) * int.Parse(prodAmount)).ToString("0.00"));
                mainCardData.Add((float.Parse(prodWeight) * int.Parse(prodRequired)).ToString("0.00"));
                mainCardData.Add(productName);
                mainCardData.Add(productRecord);
                mainCardData.Add(productType);
                mainCardData.Add(unit);
                mainCardData.Add(blowSize);
                

                Application.Current.Properties["cardMainData"] = mainCardData;
                SucceedLogCreate("cardList : prepare blowing card data");
            }catch(Exception ex){
                ErrorLogCreate(ex);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PrepCuttingCard(String cardId)
        {
            try
            {
                CheckStateDB();
                String[] card = cardId.Split('-');
                String sql_data = "SELECT cardCode, produceAmount, productId, cardDetail, requiredAmount FROM command_card WHERE rowid = @row AND year = @year";
                cmd = new MySqlCommand(sql_data, conn);
                cmd.Parameters.AddWithValue("@year", card[0]);
                cmd.Parameters.AddWithValue("@row", int.Parse(card[1]));
                reader = cmd.ExecuteReader();
                reader.Read();
                String cardCode = reader.GetString("cardCode"), productCode = reader.GetString("productId"),
                    cardDetail = reader.GetString("cardDetail"), prodAmount = reader.GetString("produceAmount"),
                    prodRequired = reader.GetString("requiredAmount");
                reader.Close();
                
                sql_data = "SELECT product_weight, unit, product_type, productRecord, " +
                    "product_name, cutSize " +
                    "FROM product WHERE product_id = @id";
                cmd = new MySqlCommand(sql_data, conn);
                cmd.Parameters.AddWithValue("@id", productCode);
                reader = cmd.ExecuteReader();
                reader.Read();
                String prodWeight = reader.GetString("product_weight"),
                    unit = reader.GetString("unit"), productType = reader.GetString("product_type"),
                    productName = reader.GetString("product_name"), productRecord = reader.GetString("productRecord"),
                    cutSize = reader.GetString("cutSize");
                reader.Close();

                List<String> mainCardData = new List<String>();
                mainCardData.Add(cardCode.Split(',')[2]);
                mainCardData.Add(productCode);
                mainCardData.Add(cardDetail.Split(',')[2]);
                mainCardData.Add(prodAmount);
                mainCardData.Add(prodRequired);
                mainCardData.Add((float.Parse(prodWeight) * int.Parse(prodAmount)).ToString("0.00"));
                mainCardData.Add((float.Parse(prodWeight) * int.Parse(prodRequired)).ToString("0.00"));
                mainCardData.Add(productName);
                mainCardData.Add(productRecord);
                mainCardData.Add(productType);
                mainCardData.Add(unit);
                mainCardData.Add(cutSize);


                Application.Current.Properties["cardMainData"] = mainCardData;
                SucceedLogCreate("cardList : prepare cutting card data");
            }
            catch (Exception ex)
            {
    
                ErrorLogCreate(ex);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = sender as ListViewItem;
                if (item != null && item.IsSelected)
                {
                    int cardType = (int)Application.Current.Properties["cardType"];
                    dynamic selectedItem = jobList.SelectedItems[0];
                    var orderNumber = selectedItem.Col1;
                    var receiveNumber = selectedItem.Col2;
                    var recordDate = selectedItem.Col3;
                    var finishDate = selectedItem.Col4;
                    var customerName = selectedItem.Col5;
                    var productName = selectedItem.Col6;
                    List<String> cardData = new List<String>();
                    cardData.Add(orderNumber);
                    cardData.Add(receiveNumber);
                    cardData.Add(recordDate);
                    cardData.Add(finishDate);
                    cardData.Add(customerName);
                    cardData.Add(productName);
                    if (cardType == 4)
                    {
                        //material card
                        PrepMaterialCard(orderNumber);
                    }
                    else if (cardType == 3)
                    {
                        //printing card
                        PrepPrintCard(orderNumber);
                    }
                    else if (cardType == 2)
                    {
                        //printing blowing card
                        PrepBlowCard(orderNumber);
                    }
                    else if (cardType == 1)
                    {
                        PrepCuttingCard(orderNumber);
                    }
                    Application.Current.Properties["cardData"] = cardData;
                    SucceedLogCreate("cardList : preview job list");
                    ReportPreview ses = new ReportPreview();
                    ses.Show();
                    this.Close();
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
