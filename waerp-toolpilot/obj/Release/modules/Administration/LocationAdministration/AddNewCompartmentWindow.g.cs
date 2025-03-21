﻿#pragma checksum "..\..\..\..\..\modules\Administration\LocationAdministration\AddNewCompartmentWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "48E53A06A36D514C439EA6768625A8C0871DDEE4B746E694AD2B0A61D7ED1761"
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
using waerp_toolpilot.modules.Administration.LocationAdministration;


namespace waerp_toolpilot.modules.Administration.LocationAdministration {
    
    
    /// <summary>
    /// AddNewCompartmentWindow
    /// </summary>
    public partial class AddNewCompartmentWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 49 "..\..\..\..\..\modules\Administration\LocationAdministration\AddNewCompartmentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CompartmentName;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\..\..\modules\Administration\LocationAdministration\AddNewCompartmentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox OnlyUsed;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\..\..\modules\Administration\LocationAdministration\AddNewCompartmentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox OnlyNew;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\..\..\..\modules\Administration\LocationAdministration\AddNewCompartmentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CompartmentIsDynamic;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\..\..\..\modules\Administration\LocationAdministration\AddNewCompartmentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MaterialDesignThemes.Wpf.Card CompartmentSettings;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\..\..\..\modules\Administration\LocationAdministration\AddNewCompartmentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ItemIdentReserved;
        
        #line default
        #line hidden
        
        
        #line 112 "..\..\..\..\..\modules\Administration\LocationAdministration\AddNewCompartmentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CloseDialog;
        
        #line default
        #line hidden
        
        
        #line 122 "..\..\..\..\..\modules\Administration\LocationAdministration\AddNewCompartmentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CreateCompartment;
        
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
            System.Uri resourceLocater = new System.Uri("/toolpilot;component/modules/administration/locationadministration/addnewcompartm" +
                    "entwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\modules\Administration\LocationAdministration\AddNewCompartmentWindow.xaml"
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
            
            #line 30 "..\..\..\..\..\modules\Administration\LocationAdministration\AddNewCompartmentWindow.xaml"
            ((System.Windows.Controls.Border)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Border_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.CompartmentName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.OnlyUsed = ((System.Windows.Controls.CheckBox)(target));
            
            #line 62 "..\..\..\..\..\modules\Administration\LocationAdministration\AddNewCompartmentWindow.xaml"
            this.OnlyUsed.Click += new System.Windows.RoutedEventHandler(this.OnlyUsed_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.OnlyNew = ((System.Windows.Controls.CheckBox)(target));
            
            #line 69 "..\..\..\..\..\modules\Administration\LocationAdministration\AddNewCompartmentWindow.xaml"
            this.OnlyNew.Click += new System.Windows.RoutedEventHandler(this.OnlyNew_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.CompartmentIsDynamic = ((System.Windows.Controls.CheckBox)(target));
            
            #line 77 "..\..\..\..\..\modules\Administration\LocationAdministration\AddNewCompartmentWindow.xaml"
            this.CompartmentIsDynamic.Click += new System.Windows.RoutedEventHandler(this.CompartmentIsDynamic_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.CompartmentSettings = ((MaterialDesignThemes.Wpf.Card)(target));
            return;
            case 7:
            this.ItemIdentReserved = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 8:
            this.CloseDialog = ((System.Windows.Controls.Button)(target));
            
            #line 117 "..\..\..\..\..\modules\Administration\LocationAdministration\AddNewCompartmentWindow.xaml"
            this.CloseDialog.Click += new System.Windows.RoutedEventHandler(this.CloseDialog_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.CreateCompartment = ((System.Windows.Controls.Button)(target));
            
            #line 127 "..\..\..\..\..\modules\Administration\LocationAdministration\AddNewCompartmentWindow.xaml"
            this.CreateCompartment.Click += new System.Windows.RoutedEventHandler(this.CreateCompartment_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

