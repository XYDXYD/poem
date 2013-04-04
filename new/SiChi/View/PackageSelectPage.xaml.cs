using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace SiChi.View
{
    public partial class PackageSelectPage : PhoneApplicationPage
    {
        public PackageSelectPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            PackageListBox.Items.Clear();
            List<string> packagesName = App._packageManager.PackageNames;
            AddIntoListBox(packagesName);
            base.OnNavigatedTo(e);
        }


        //添加到listbox中
        private void AddIntoListBox(List<string> input)
        {
            foreach (string s in input)
            {
                StackPanel stackPanel = new StackPanel { Orientation = System.Windows.Controls.Orientation.Vertical };
                stackPanel.Children.Add(new TextBlock { Foreground = new SolidColorBrush(Colors.Black), Text = s, FontSize = 32 });
                stackPanel.Children.Add(new Border { BorderBrush = new SolidColorBrush(Colors.White), BorderThickness = new Thickness(0, 0, 0, 1), Width = 400 });
                PackageListBox.Items.Add(stackPanel);
            }

            PackageListBox.Items.Add(new TextBlock { Foreground = new SolidColorBrush(Colors.Black), Text = "下载教材", FontSize = 32 });
            PackageListBox.SelectedIndex = -1;

        }


        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox.SelectedIndex == -1) return;

            string address;
            if (listBox.SelectedIndex == listBox.Items.Count - 1)
            {
                address = "/View/DownloadPage.xaml";
            }
            else
            {
                App._packageIndex = listBox.SelectedIndex;
                address = "/View/PackagePage.xaml";
            }
            this.NavigationService.Navigate(new Uri(address, UriKind.Relative));
        }

    }
}