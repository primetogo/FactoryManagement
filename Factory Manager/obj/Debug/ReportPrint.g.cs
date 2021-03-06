﻿#pragma checksum "..\..\ReportPrint.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1B0A639F4B50155DAA44E910BEC32493"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using RootLibrary.WPF.Localization;
using SAPBusinessObjects.WPF.Viewer;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Factory_Manager {
    
    
    /// <summary>
    /// ReportPrint
    /// </summary>
    public partial class ReportPrint : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\ReportPrint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button printBtn;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\ReportPrint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton orderNo;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\ReportPrint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton receiveNo;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\ReportPrint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton recordDate;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\ReportPrint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton finishDate;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\ReportPrint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Calendar dateCalendar;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\ReportPrint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox queryString;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\ReportPrint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label dateCalendarText;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\ReportPrint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton cut;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\ReportPrint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton blow;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\ReportPrint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton print;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\ReportPrint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton material;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Factory Manager;component/reportprint.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ReportPrint.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.printBtn = ((System.Windows.Controls.Button)(target));
            
            #line 9 "..\..\ReportPrint.xaml"
            this.printBtn.Click += new System.Windows.RoutedEventHandler(this.printBtn_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.orderNo = ((System.Windows.Controls.RadioButton)(target));
            
            #line 11 "..\..\ReportPrint.xaml"
            this.orderNo.Checked += new System.Windows.RoutedEventHandler(this.orderNo_Checked);
            
            #line default
            #line hidden
            return;
            case 3:
            this.receiveNo = ((System.Windows.Controls.RadioButton)(target));
            
            #line 12 "..\..\ReportPrint.xaml"
            this.receiveNo.Checked += new System.Windows.RoutedEventHandler(this.receiveNo_Checked);
            
            #line default
            #line hidden
            return;
            case 4:
            this.recordDate = ((System.Windows.Controls.RadioButton)(target));
            
            #line 13 "..\..\ReportPrint.xaml"
            this.recordDate.Checked += new System.Windows.RoutedEventHandler(this.recordDate_Checked);
            
            #line default
            #line hidden
            return;
            case 5:
            this.finishDate = ((System.Windows.Controls.RadioButton)(target));
            
            #line 14 "..\..\ReportPrint.xaml"
            this.finishDate.Checked += new System.Windows.RoutedEventHandler(this.finishDate_Checked);
            
            #line default
            #line hidden
            return;
            case 6:
            this.dateCalendar = ((System.Windows.Controls.Calendar)(target));
            return;
            case 7:
            this.queryString = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.dateCalendarText = ((System.Windows.Controls.Label)(target));
            return;
            case 9:
            this.cut = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 10:
            this.blow = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 11:
            this.print = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 12:
            this.material = ((System.Windows.Controls.RadioButton)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

