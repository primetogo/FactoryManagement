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
    /// Interaction logic for ReportPrint.xaml
    /// </summary>
    public partial class ReportPrint : Window
    {
        public ReportPrint()
        {
            InitializeComponent();
            this.dateCalendar.Visibility = System.Windows.Visibility.Hidden;
            this.dateCalendarText.Visibility = System.Windows.Visibility.Hidden;
        }
        int modeSelected = 0, typeSelected = 0;
        String daySelect = "none";

        private void ActionCalendar(String action)
        {
            if (action.Equals("show"))
            {
                this.dateCalendar.Visibility = System.Windows.Visibility.Visible;
                this.dateCalendarText.Visibility = System.Windows.Visibility.Visible;
            }
            else if (action.Equals("hide"))
            {
                this.dateCalendar.Visibility = System.Windows.Visibility.Hidden;
                this.dateCalendarText.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void ActionQueryBox(String action)
        {
            if (action.Equals("show"))
            {
                this.queryString.Visibility = System.Windows.Visibility.Visible;
            }
            else if (action.Equals("hide"))
            {
                this.queryString.Visibility = System.Windows.Visibility.Hidden;
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
        private void ModeSelect()
        {
            if (this.orderNo.IsChecked == true)
            {
                modeSelected = 1;
            }
            else if (this.receiveNo.IsChecked == true)
            {
                modeSelected = 2;
            }
            else if (this.recordDate.IsChecked == true)
            {
                modeSelected = 4;
            }
            else if (this.finishDate.IsChecked == true)
            {
                modeSelected = 5;
            }
        }

        private void TypeSelect()
        {
            if (this.cut.IsChecked == true)
            {
                typeSelected = 1;
            }
            else if (this.blow.IsChecked == true)
            {
                typeSelected = 2;
            }
            else if (this.print.IsChecked == true)
            {
                typeSelected = 3;
            }
            else if (this.material.IsChecked == true)
            {
                typeSelected = 4;
            }
        }

        private void GetDate()
        {
            CultureInfo ci = new CultureInfo("en-US");
            DateTime day = (DateTime)dateCalendar.SelectedDate;
            daySelect = day.ToString("dd/MM/yyyy", ci);
        }
        private void printBtn_Click(object sender, RoutedEventArgs e)
        {
            Boolean gatePass = false;
            ModeSelect();
            TypeSelect();
            if (modeSelected == 0 || typeSelected == 0)
            {
                MessageBox.Show("ยังไม่ได้เลือกการค้นหาหรือยังไม่ได้เลือกประเภทใบคำสั่ง", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                Application.Current.Properties["cardType"] = typeSelected.ToString();
                if (modeSelected == 4)
                {
                    GetDate();
                    Application.Current.Properties["queryData"] = daySelect;
                    Application.Current.Properties["queryType"] = "byRecordDate";
                    gatePass = true;
                }
                else if (modeSelected == 5)
                {
                    GetDate();
                    Application.Current.Properties["queryData"] = daySelect;
                    Application.Current.Properties["queryType"] = "byFinishDate";
                    gatePass = true;
                }
                else if (modeSelected == 1)
                {
                    Application.Current.Properties["queryData"] = this.queryString.Text;
                    Application.Current.Properties["queryType"] = "byOrderNumber";
                    if (queryString.Text.Contains('-') == false || queryString.Text[0] == '-' || queryString.Text[queryString.Text.Length - 1] == '-')
                    {
                        MessageBox.Show("กรอกลำดับสั่งไม่ถูกต้อง กรุณากรอกใหม่", "ข้อผิดพลาด", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        gatePass = true;
                    }
                }
                else if (modeSelected == 2)
                {
                    Application.Current.Properties["queryData"] = this.queryString.Text;
                    Application.Current.Properties["queryType"] = "byReceiveNumber";
                    gatePass = true;
                }
                if (gatePass == true)
                {
                    try
                    {
                        Application.Current.Properties["cardType"] = typeSelected;
                        gatePass = false;
                        SucceedLogCreate("reportPrint : printing click");
                        cardList ses = new cardList();
                        ses.Show();
                    }
                    catch (Exception ex)
                    {
                        ErrorLogCreate(ex);
                    }
                }

            }
        }

        private void recordDate_Checked(object sender, RoutedEventArgs e)
        {
            ActionCalendar("show");
            ActionQueryBox("hide");
        }

        private void finishDate_Checked(object sender, RoutedEventArgs e)
        {
            ActionCalendar("show");
            ActionQueryBox("hide");
        }
        private void receiveNo_Checked(object sender, RoutedEventArgs e)
        {
            ActionCalendar("hide");
            ActionQueryBox("show");
        }

        private void orderNo_Checked(object sender, RoutedEventArgs e)
        {
            ActionCalendar("hide");
            ActionQueryBox("show");
        }


    }
}
