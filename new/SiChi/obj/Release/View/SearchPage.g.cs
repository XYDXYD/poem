﻿#pragma checksum "F:\备份\SiChi\View\SearchPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "CA104F041469DA671BE7CBC32CDE3B68"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.17929
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace SiChi.View {
    
    
    public partial class SearchPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Media.Animation.Storyboard showAnimation;
        
        internal System.Windows.Media.Animation.Storyboard fadeAnimation;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.ListBox listBox;
        
        internal System.Windows.Controls.TextBox textBox;
        
        internal System.Windows.Controls.Image button;
        
        internal System.Windows.Controls.Grid wait;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/SiChi;component/View/SearchPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.showAnimation = ((System.Windows.Media.Animation.Storyboard)(this.FindName("showAnimation")));
            this.fadeAnimation = ((System.Windows.Media.Animation.Storyboard)(this.FindName("fadeAnimation")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.listBox = ((System.Windows.Controls.ListBox)(this.FindName("listBox")));
            this.textBox = ((System.Windows.Controls.TextBox)(this.FindName("textBox")));
            this.button = ((System.Windows.Controls.Image)(this.FindName("button")));
            this.wait = ((System.Windows.Controls.Grid)(this.FindName("wait")));
        }
    }
}

