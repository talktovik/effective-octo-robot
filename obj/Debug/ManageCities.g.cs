#pragma checksum "..\..\ManageCities.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "B379D08960BA5DE62D0360012FF2A0EF18E1549000A4A7EA5FE414B6503FD5E1"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using QM;
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


namespace QM {
    
    
    /// <summary>
    /// ManageCities
    /// </summary>
    public partial class ManageCities : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\ManageCities.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid gridCities;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\ManageCities.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtCity;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\ManageCities.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtState;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\ManageCities.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtCountry;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\ManageCities.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtPincode;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\ManageCities.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtStdCode;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\ManageCities.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSaveCity;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\ManageCities.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnDeleteCity;
        
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
            System.Uri resourceLocater = new System.Uri("/QM;component/managecities.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ManageCities.xaml"
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
            this.gridCities = ((System.Windows.Controls.DataGrid)(target));
            
            #line 10 "..\..\ManageCities.xaml"
            this.gridCities.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.gridCities_SelectionChanged);
            
            #line default
            #line hidden
            
            #line 10 "..\..\ManageCities.xaml"
            this.gridCities.AutoGeneratingColumn += new System.EventHandler<System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs>(this.gridCities_AutoGeneratingColumn);
            
            #line default
            #line hidden
            
            #line 10 "..\..\ManageCities.xaml"
            this.gridCities.Loaded += new System.Windows.RoutedEventHandler(this.gridCities_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.txtCity = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.txtState = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.txtCountry = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.txtPincode = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.txtStdCode = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.btnSaveCity = ((System.Windows.Controls.Button)(target));
            
            #line 66 "..\..\ManageCities.xaml"
            this.btnSaveCity.Click += new System.Windows.RoutedEventHandler(this.btnSaveCity_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.btnDeleteCity = ((System.Windows.Controls.Button)(target));
            
            #line 67 "..\..\ManageCities.xaml"
            this.btnDeleteCity.Click += new System.Windows.RoutedEventHandler(this.btnDeleteCity_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

