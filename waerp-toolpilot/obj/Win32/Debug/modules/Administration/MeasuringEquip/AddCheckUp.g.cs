﻿#pragma checksum "..\..\..\..\..\..\modules\Administration\MeasuringEquip\AddCheckUp.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "D5F2F523F887E27CE2FDA61246F505ADB46A37F1E0B65CB72290F9C5EFC1824F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
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
using waerp_toolpilot.modules.Administration.MeasuringEquip;


namespace waerp_toolpilot.modules.Administration.MeasuringEquip {
    
    
    /// <summary>
    /// AddCheckUp
    /// </summary>
    public partial class AddCheckUp : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 51 "..\..\..\..\..\..\modules\Administration\MeasuringEquip\AddCheckUp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock MeasureEquipName;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\..\..\..\modules\Administration\MeasuringEquip\AddCheckUp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock MeasureEquipVendor;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\..\..\..\..\modules\Administration\MeasuringEquip\AddCheckUp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock MeasureEquipQuant;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\..\..\..\..\modules\Administration\MeasuringEquip\AddCheckUp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker selectedDate;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\..\..\..\..\modules\Administration\MeasuringEquip\AddCheckUp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PDFFilePath;
        
        #line default
        #line hidden
        
        
        #line 109 "..\..\..\..\..\..\modules\Administration\MeasuringEquip\AddCheckUp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SelectFileBtn;
        
        #line default
        #line hidden
        
        
        #line 129 "..\..\..\..\..\..\modules\Administration\MeasuringEquip\AddCheckUp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CloseDialog;
        
        #line default
        #line hidden
        
        
        #line 138 "..\..\..\..\..\..\modules\Administration\MeasuringEquip\AddCheckUp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CreateCheckUp;
        
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
            System.Uri resourceLocater = new System.Uri("/toolpilot;component/modules/administration/measuringequip/addcheckup.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\modules\Administration\MeasuringEquip\AddCheckUp.xaml"
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
            
            #line 28 "..\..\..\..\..\..\modules\Administration\MeasuringEquip\AddCheckUp.xaml"
            ((System.Windows.Controls.Border)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Border_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.MeasureEquipName = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.MeasureEquipVendor = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.MeasureEquipQuant = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.selectedDate = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 6:
            this.PDFFilePath = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.SelectFileBtn = ((System.Windows.Controls.Button)(target));
            
            #line 113 "..\..\..\..\..\..\modules\Administration\MeasuringEquip\AddCheckUp.xaml"
            this.SelectFileBtn.Click += new System.Windows.RoutedEventHandler(this.selectPdfDocument);
            
            #line default
            #line hidden
            return;
            case 8:
            this.CloseDialog = ((System.Windows.Controls.Button)(target));
            
            #line 134 "..\..\..\..\..\..\modules\Administration\MeasuringEquip\AddCheckUp.xaml"
            this.CloseDialog.Click += new System.Windows.RoutedEventHandler(this.CloseDialog_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.CreateCheckUp = ((System.Windows.Controls.Button)(target));
            
            #line 143 "..\..\..\..\..\..\modules\Administration\MeasuringEquip\AddCheckUp.xaml"
            this.CreateCheckUp.Click += new System.Windows.RoutedEventHandler(this.CreateCheckUp_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

