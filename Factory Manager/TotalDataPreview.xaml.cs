using CrystalDecisions.CrystalReports.Engine;
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
    /// Interaction logic for TotalDataPreview.xaml
    /// </summary>
    public partial class TotalDataPreview : Window
    {
        public TotalDataPreview()
        {
            InitializeComponent();
            ScreenPreview();
        }


        private void ScreenPreview()
        {
            try
            {
                ProgressBarWindow ses = new ProgressBarWindow();
                ses.Show();
                ///String[] jobData = { jobNo, jobProduct, jobCustomer, jobStart, JobEnd, dataSet[2] + "-" + dataSet[3], dataSet[0], dataSet[1] };
                String[] dataIn = (String[])Application.Current.Properties["dataTable"];
                CultureInfo ci = new CultureInfo("en-US");
                String currentDate = DateTime.Now.ToString("dd/MM/yyyy", ci);
                String jobNo = dataIn[0];
                String jobProduct = dataIn[1];
                String jobCustomer = dataIn[2];
                String jobStart = dataIn[3];
                String JobEnd = dataIn[4];
                ReportDocument report = LoadingReport();
                report.SetParameterValue("runNumberTable", Convert.ToString(jobNo));
                report.SetParameterValue("productTable", Convert.ToString(jobProduct));
                report.SetParameterValue("customerTable", Convert.ToString(jobCustomer));
                report.SetParameterValue("startDateTable", Convert.ToString(jobStart));
                report.SetParameterValue("finishDateTable", Convert.ToString(JobEnd));
                report.SetParameterValue("dateQuery", Convert.ToString(currentDate));
                this.viewer.ViewerCore.ReportSource = report;
                if (report.IsLoaded == true)
                {
                    ses.Close();
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

        public ReportDocument LoadingReport()
        {
            ReportDocument report = new ReportDocument();
            try
            {
                string currentDirectory = "C:\\FMreport";
                string filePath = System.IO.Path.Combine(currentDirectory, "reportJob.rpt");
                report.Load(filePath);
            }
            catch (Exception e)
            {
                ErrorLogCreate(e);
                MessageBox.Show("เกิดข้อผิดพลาด ข้อมูล error บันทึกอยู่ในไฟล์ log กรุณาแจ้งข้อมูลดังกล่าวแก่ทีมติดตั้ง"
                                    , "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return report;
        }
    }
}
