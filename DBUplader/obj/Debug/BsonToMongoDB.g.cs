﻿#pragma checksum "..\..\BsonToMongoDB.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "D6CFD0F30F3FD2CAAB23C4797FB8BEA0B3AD32E6"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DBUplader;
using MahApps.Metro.Controls;
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


namespace DBUplader {
    
    
    /// <summary>
    /// BsonToMongoDB
    /// </summary>
    public partial class BsonToMongoDB : MahApps.Metro.Controls.MetroWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 32 "..\..\BsonToMongoDB.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TableNameTextBox;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\BsonToMongoDB.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ChooseFile;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\BsonToMongoDB.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CSVPathTextBox;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\BsonToMongoDB.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BulkToMongoDB;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\BsonToMongoDB.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label CSVToBsonTime;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\BsonToMongoDB.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label BsonToMongoTime;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\BsonToMongoDB.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label NumberOfRecords;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\BsonToMongoDB.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label WrongLinesNumber;
        
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
            System.Uri resourceLocater = new System.Uri("/DBUplader;component/bsontomongodb.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\BsonToMongoDB.xaml"
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
            this.TableNameTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 32 "..\..\BsonToMongoDB.xaml"
            this.TableNameTextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TableNameTextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ChooseFile = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\BsonToMongoDB.xaml"
            this.ChooseFile.Click += new System.Windows.RoutedEventHandler(this.ChooseFile_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.CSVPathTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 34 "..\..\BsonToMongoDB.xaml"
            this.CSVPathTextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.CSVPathTextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.BulkToMongoDB = ((System.Windows.Controls.Button)(target));
            
            #line 35 "..\..\BsonToMongoDB.xaml"
            this.BulkToMongoDB.Click += new System.Windows.RoutedEventHandler(this.BulkToMongoDB_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.CSVToBsonTime = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.BsonToMongoTime = ((System.Windows.Controls.Label)(target));
            return;
            case 7:
            this.NumberOfRecords = ((System.Windows.Controls.Label)(target));
            return;
            case 8:
            this.WrongLinesNumber = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

