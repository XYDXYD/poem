﻿#pragma checksum "F:\备份\SiChi\View\RankPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F353A9866305FA765FDC0F289C1AC433"
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
    
    
    public partial class RankPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.TextBlock ApplicationTitle;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.Image rankButton;
        
        internal System.Windows.Controls.TextBlock rankTextBlock;
        
        internal System.Windows.Controls.TextBlock percentageTextBlock;
        
        internal System.Windows.Controls.Image image1;
        
        internal System.Windows.Media.Animation.Storyboard animation;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/SiChi;component/View/RankPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.ApplicationTitle = ((System.Windows.Controls.TextBlock)(this.FindName("ApplicationTitle")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.rankButton = ((System.Windows.Controls.Image)(this.FindName("rankButton")));
            this.rankTextBlock = ((System.Windows.Controls.TextBlock)(this.FindName("rankTextBlock")));
            this.percentageTextBlock = ((System.Windows.Controls.TextBlock)(this.FindName("percentageTextBlock")));
            this.image1 = ((System.Windows.Controls.Image)(this.FindName("image1")));
            this.animation = ((System.Windows.Media.Animation.Storyboard)(this.FindName("animation")));
        }
    }
}
