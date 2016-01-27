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
using System.Threading;

namespace Factory_Manager
{
    /// <summary>
    /// Interaction logic for ReportPreview.xaml
    /// </summary>
    public partial class ReportPreview : Window
    {
        public ReportPreview()
        {
            InitializeComponent();
            LoadingReport();
        }

        

        private void LoadingReport()
        {
            try
            {
                int type = (int)Application.Current.Properties["cardType"];
                if (type == 4)
                {
                    ProgressBarWindow ses = new ProgressBarWindow();
                    ses.Show();
                    string currentDirectory = "C:\\FMreport";
                    string filePath = System.IO.Path.Combine(currentDirectory, "material.rpt");
                    ReportDocument report = new ReportDocument();
                    report.Load(filePath);
                    List<List<String>> detailCardData = (List<List<String>>)Application.Current.Properties["cardDetail"];
                    List<String> cardData = (List<String>)Application.Current.Properties["cardMainData"];
                    List<String> mainData = (List<String>)Application.Current.Properties["cardData"];
                    MaterialReport(detailCardData, report, cardData, mainData);
                    this.viewer.ViewerCore.ReportSource = report;
                    if (report.IsLoaded)
                    {
                        ses.Close();
                    }
                }
                else if (type == 3)
                {
                    ProgressBarWindow ses = new ProgressBarWindow();
                    ses.Show();
                    string currentDirectory = "C:\\FMreport";
                    string filePath = System.IO.Path.Combine(currentDirectory, "printing.rpt");
                    ReportDocument report = new ReportDocument();
                    report.Load(filePath);
                    List<String> mainData = (List<String>)Application.Current.Properties["cardMainData"];
                    List<String> cardData = (List<String>)Application.Current.Properties["cardData"];
                    PrintReport(cardData, mainData, report);
                    this.viewer.ViewerCore.ReportSource = report;
                    if (report.IsLoaded)
                    {
                        ses.Close();
                    }
                }

                else if (type == 2)
                {
                    ProgressBarWindow ses = new ProgressBarWindow();
                    ses.Show();
                    string currentDirectory = "C:\\FMreport";
                    string filePath = System.IO.Path.Combine(currentDirectory, "blowing.rpt");
                    ReportDocument report = new ReportDocument();
                    report.Load(filePath);
                    List<String> mainData = (List<String>)Application.Current.Properties["cardMainData"];
                    List<String> cardData = (List<String>)Application.Current.Properties["cardData"];
                    BlowReport(cardData, mainData, report);
                    this.viewer.ViewerCore.ReportSource = report;
                    if (report.IsLoaded)
                    {
                        ses.Close();
                    }
                }

                else if (type == 1)
                {
                    ProgressBarWindow ses = new ProgressBarWindow();
                    ses.Show();
                    string currentDirectory = "C:\\FMreport";
                    string filePath = System.IO.Path.Combine(currentDirectory, "cutting.rpt");
                    ReportDocument report = new ReportDocument();
                    report.Load(filePath);
                    List<String> mainData = (List<String>)Application.Current.Properties["cardMainData"];
                    List<String> cardData = (List<String>)Application.Current.Properties["cardData"];
                    CutReport(cardData, mainData, report);
                    this.viewer.ViewerCore.ReportSource = report;
                    if (report.IsLoaded)
                    {
                        ses.Close();
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


        private void CutReport(List<String> cardData, List<String> mainData, ReportDocument reportIn)
        {
            try
            {
                this.Title = "ใบสั่งตัด";
                CultureInfo ci = new CultureInfo("en-US");
                String currentDate = DateTime.Now.ToString("dd/MM/yyyy", ci);
                String cardCode = mainData[0], productCode = mainData[1], cardDetail = mainData[2], produceAmount = mainData[3],
                   requiredProduct = mainData[4], produceProductWeight = mainData[5], requiredProductWeight = mainData[6],
                   productName = mainData[7], productRecord = mainData[8], productType = mainData[9], unit = mainData[10],
                   cutSize = mainData[11], runNumber = cardData[0], receiveNumber = cardData[1], recordDate = cardData[2],
                   finishDate = cardData[3], customerName = cardData[4];
                reportIn.SetParameterValue("card_number", cardCode);
                reportIn.SetParameterValue("card_date", currentDate);
                reportIn.SetParameterValue("recieve_number", receiveNumber);
                reportIn.SetParameterValue("record_Date", recordDate);
                reportIn.SetParameterValue("run_number", runNumber);
                reportIn.SetParameterValue("finish_date", finishDate);
                reportIn.SetParameterValue("customer_name", customerName);
                reportIn.SetParameterValue("product_type", productName);
                reportIn.SetParameterValue("product_id", productCode);
                reportIn.SetParameterValue("unit", unit);
                reportIn.SetParameterValue("product_name", productName);
                reportIn.SetParameterValue("require_amount", requiredProduct);
                reportIn.SetParameterValue("require_weight", requiredProductWeight);
                reportIn.SetParameterValue("produce_amount", produceAmount);
                reportIn.SetParameterValue("produce_weight", produceProductWeight);
                reportIn.SetParameterValue("cutting_size", cutSize);
                reportIn.SetParameterValue("ps", cardDetail);
                reportIn.SetParameterValue("product_record", productRecord);
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PrintReport(List<String> cardData, List<String> mainData, ReportDocument reportIn)
        {
            try
            {
                this.Title = "ใบสั่งพิมพ์";
                CultureInfo ci = new CultureInfo("en-US");
                String currentDate = DateTime.Now.ToString("dd/MM/yyyy", ci);
                String cardNumber = mainData[0], receiveNumber = cardData[1], recordDate = cardData[2],
                    runNumber = cardData[0], finishDate = cardData[3], customerName = cardData[4],
                    productId = mainData[1], productType = mainData[14], productName = cardData[5],
                    requiredAmount = mainData[4], requiredWeight = mainData[6], produceAmount = mainData[3],
                    produceWeight = mainData[5], unit = mainData[15], productRecord = mainData[8],
                    printingSize = mainData[9], frontPrintAmount = mainData[10], backPrintAmount = mainData[11],
                    frontPrintColor = mainData[12], backPrintColor = mainData[13], cardDetail = mainData[2];

                reportIn.SetParameterValue("card_number", cardNumber);
                reportIn.SetParameterValue("card_date", currentDate);
                reportIn.SetParameterValue("recieve_number", receiveNumber);
                reportIn.SetParameterValue("record_Date", recordDate);
                reportIn.SetParameterValue("run_number", runNumber);
                reportIn.SetParameterValue("finish_date", finishDate);
                reportIn.SetParameterValue("customer_name", customerName);
                reportIn.SetParameterValue("product_type", productType);
                reportIn.SetParameterValue("product_id", productId);
                reportIn.SetParameterValue("unit", unit);
                reportIn.SetParameterValue("product_name", productName);
                reportIn.SetParameterValue("printing_size", printingSize);
                reportIn.SetParameterValue("ps", cardDetail);
                reportIn.SetParameterValue("front_page", frontPrintAmount);
                reportIn.SetParameterValue("back_page", backPrintAmount);
                reportIn.SetParameterValue("front_color", frontPrintColor);
                reportIn.SetParameterValue("back_color", backPrintColor);
                reportIn.SetParameterValue("product_record", productRecord);
                reportIn.SetParameterValue("require_amount", requiredAmount);
                reportIn.SetParameterValue("require_weight", requiredWeight);
                reportIn.SetParameterValue("produce_amount", produceAmount);
                reportIn.SetParameterValue("produce_weight", produceWeight);
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void MaterialReport(List<List<String>> material, ReportDocument reportIn, List<String> cardData, List<String> mainCardData)
        {
            try
            {
                this.Title = "ใบเบิกวัตถุดิบ";
                CultureInfo ci = new CultureInfo("en-US");
                String currentDate = DateTime.Now.ToString("dd/MM/yyyy", ci);
                String matOrder = "";
                String matCode = "";
                String matName = "";
                String matAmount = "";
                String matWeight = "";

                List<String> orderNumber = material[0], materialCode = material[1];
                List<String> materialName = material[2], materialAmount = material[3];
                List<String> materialCalWeight = material[4];

                String cardCode = cardData[0], requiredProduct = cardData[2], produceAmount = cardData[1];
                String productType = cardData[3], productCode = cardData[4], produceWeight = cardData[5];
                String requiredProductWeight = cardData[6], unit = cardData[7], totalAmount = cardData[8];
                String cardOrderNumber = mainCardData[0], receiveNumber = mainCardData[1], recordDate = mainCardData[2];
                String finishDate = mainCardData[3], customerName = mainCardData[4], productName = mainCardData[5];


                for (int i = 0; i < orderNumber.Count; i++)
                {
                    matCode += (materialCode[i] + "\n");
                    matOrder += (orderNumber[i] + "\n");
                    matName += (materialName[i] + "\n");
                    matAmount += (materialAmount[i] + "\n");
                    matWeight += (materialCalWeight[i] + "\n");
                }
                reportIn.SetParameterValue("mat_no1", matOrder);
                reportIn.SetParameterValue("mat_code1", matCode);
                reportIn.SetParameterValue("mat_name1", matName);
                reportIn.SetParameterValue("mat_quan1", matAmount);
                reportIn.SetParameterValue("mat_weight1", matWeight);
                reportIn.SetParameterValue("card_number", cardCode);
                reportIn.SetParameterValue("card_date", currentDate);
                reportIn.SetParameterValue("recieve_number", receiveNumber);
                reportIn.SetParameterValue("unit", unit);
                reportIn.SetParameterValue("record_Date", recordDate);
                reportIn.SetParameterValue("run_number", cardOrderNumber);
                reportIn.SetParameterValue("finish_date", finishDate);
                reportIn.SetParameterValue("customer_name", customerName);
                reportIn.SetParameterValue("product_type", productType);
                reportIn.SetParameterValue("product_id", productCode);
                reportIn.SetParameterValue("product_name", productName);
                reportIn.SetParameterValue("require_amount", requiredProduct);
                reportIn.SetParameterValue("require_weight", requiredProductWeight);
                reportIn.SetParameterValue("produce_amount", produceAmount);
                reportIn.SetParameterValue("produce_weight", produceWeight);
                reportIn.SetParameterValue("total_quan", totalAmount);
                reportIn.SetParameterValue("total_weight", produceWeight);
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BlowReport(List<String> cardData, List<String> mainData, ReportDocument reportIn)
        {
            try
            {
                this.Title = "ใบสั่งเป่า";
                CultureInfo ci = new CultureInfo("en-US");
                String currentDate = DateTime.Now.ToString("dd/MM/yyyy", ci);
                String cardCode = mainData[0], productCode = mainData[1], cardDetail = mainData[2], produceAmount = mainData[3],
                   requiredProduct = mainData[4], produceProductWeight = mainData[5], requiredProductWeight = mainData[6],
                   productName = mainData[7], productRecord = mainData[8], productType = mainData[9], unit = mainData[10],
                   blowSize = mainData[11], runNumber = cardData[0], receiveNumber = cardData[1], recordDate = cardData[2],
                   finishDate = cardData[3], customerName = cardData[4];

                reportIn.SetParameterValue("card_number", cardCode);
                reportIn.SetParameterValue("card_date", currentDate);
                reportIn.SetParameterValue("recieve_number", receiveNumber);
                reportIn.SetParameterValue("record_Date", recordDate);
                reportIn.SetParameterValue("run_number", runNumber);
                reportIn.SetParameterValue("finish_date", finishDate);
                reportIn.SetParameterValue("customer_name", customerName);
                reportIn.SetParameterValue("product_type", productType);
                reportIn.SetParameterValue("product_id", productCode);
                reportIn.SetParameterValue("unit", unit);
                reportIn.SetParameterValue("product_name", productName);
                reportIn.SetParameterValue("require_amount", requiredProduct);
                reportIn.SetParameterValue("require_weight", requiredProductWeight);
                reportIn.SetParameterValue("produce_amount", produceAmount);
                reportIn.SetParameterValue("produce_weight", produceProductWeight);
                reportIn.SetParameterValue("blowing_size", blowSize);
                reportIn.SetParameterValue("ps", cardDetail);
                reportIn.SetParameterValue("product_record", productRecord);
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
