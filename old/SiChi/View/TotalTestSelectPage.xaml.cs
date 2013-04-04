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
using System.IO.IsolatedStorage;

namespace SiChi.View
{
    public partial class TotalTestSelectPage : PhoneApplicationPage
    {
        public TotalTestSelectPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            PackageListBox.Items.Clear();
            List<string> packagesName = App._packageManager.PackageNames;
            AddIntoListBox(packagesName);

            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("reviewMode"))
                reviewMode.IsChecked = (bool)settings["reviewMode"];

            base.OnNavigatedTo(e);
        }


        //添加到listbox中
        private void AddIntoListBox(List<string> input)
        {
            for (int i = 0; i < input.Count; i++)
            {
                string s = input[i];
                StackPanel stackPanel = new StackPanel { Orientation = System.Windows.Controls.Orientation.Vertical };
                stackPanel.Children.Add(new TextBlock { Text = s, FontSize = 32 });
                if ( i != input.Count - 1 ) stackPanel.Children.Add(new Border { BorderBrush = new SolidColorBrush(Colors.White), BorderThickness = new Thickness(0, 0, 0, 1), Width = 400 });
                PackageListBox.Items.Add(stackPanel);
            }

            PackageListBox.SelectedIndex = -1;
        }


        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (PackageListBox.SelectedIndex == -1) return;
   
            App._packageIndex = listBox.SelectedIndex;
            string address = "/View/TotalTestPage.xaml";

            this.NavigationService.Navigate(new Uri(address, UriKind.Relative));
        }

        private void reviewMode_Click(object sender, RoutedEventArgs e)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (reviewMode.IsChecked == true)
                settings["reviewMode"] = true;
            else
                settings["reviewMode"] = false;
        }
    }
}