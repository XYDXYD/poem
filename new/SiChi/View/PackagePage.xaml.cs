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
using SiChi.Model;

namespace SiChi.View
{
    public partial class PackagePage : PhoneApplicationPage
    {
        public PackagePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            PoemsListBox.Items.Clear();
            int index = App._packageIndex;
            Package package = App._packageManager.Packages[index];
            AddIntoListBox(package);
            base.OnNavigatedTo(e);
        }

        
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = (sender as ListBox).SelectedIndex;
            if (index == -1) return;

            App._poemIndex = index;
            this.NavigationService.Navigate(new Uri("/View/PoemPage.xaml", UriKind.Relative));
        }

        private void AddIntoListBox(Package p)
        {
            for (int i = 0; i < p.Poems.Count; i ++)
            {
                Poem poem = p.Poems[i];
                StackPanel stackPanel = new StackPanel { Orientation = System.Windows.Controls.Orientation.Vertical };
                TextBlock titleTextBlock = new TextBlock { Foreground = new SolidColorBrush(Colors.Black), Width = 400, Text = poem.Title, FontSize = 26,TextWrapping = TextWrapping.Wrap };
                TextBlock authorTextBlock = new TextBlock { Foreground = new SolidColorBrush(Colors.Black), Text = poem.Author, FontSize = 20 };
                Border border = new Border
                {
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    BorderBrush = new SolidColorBrush(Colors.White),
                    Width = 400,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center
                };
                stackPanel.Children.Add(titleTextBlock);
                stackPanel.Children.Add(authorTextBlock);
                if (i != p.Poems.Count - 1) stackPanel.Children.Add(border);

                PoemsListBox.Items.Add(stackPanel);
            }

            PoemsListBox.SelectedIndex = -1;

        }

    }
}