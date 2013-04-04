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
using System.Windows.Media.Imaging;

namespace SiChi.View
{
    public partial class UserInformation : PhoneApplicationPage
    {
        public UserInformation()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            listBox.Items.Clear();
            AddPackageInfo();

            base.OnNavigatedTo(e);
        }

        private void AddPoemsInfo(int index,int pos)
        {
            List<Poem> poems = App._packageManager.Packages[index].Poems;
            Dictionary<string,PoemStatus> dictionary = App._userManager.UserNow.PoemsStatus;

            StackPanel totalStackPanel = new StackPanel { Width = 300, Margin = new Thickness(60,0,0,0) };
            for (int i = 0; i < poems.Count; i++)
            {
                Border border = new Border { Width = 300, BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness = new Thickness(0,0,0,1)};
                StackPanel stackPanel = new StackPanel { Width = 300, Orientation = System.Windows.Controls.Orientation.Horizontal};
                TextBlock textBlock = new TextBlock { Width = 200, Text = poems[i].Title, FontSize = 20, Foreground = new SolidColorBrush(Colors.Black) };

                int score = 0;
                if (dictionary.ContainsKey(poems[i].Id))
                {
                    PoemStatus p = dictionary[poems[i].Id];
                    if (p == PoemStatus.PhaseTwo) score = 25;
                    else if (p == PoemStatus.PhaseThree) score = 50;
                    else if (p == PoemStatus.PhaseFour) score = 75;
                    else if (p == PoemStatus.Pass) score = 100;
                }
                TextBlock scoreBlock = new TextBlock { Width = 100, Text = "进度:" + score.ToString() + "%", FontSize = 18, Foreground = new SolidColorBrush(Colors.Black) };

                stackPanel.Children.Add(textBlock);
                stackPanel.Children.Add(scoreBlock);
                border.Child = stackPanel;
                totalStackPanel.Children.Add(border);
            }
            listBox.Items.Insert(pos + 1, totalStackPanel);
        }

        private void AddPackageInfo()
        {
            List<string> temp = App._packageManager.PackageNames;
            Dictionary<string,PoemStatus> dictionary = App._userManager.UserNow.PoemsStatus;

            for (int i = 0; i < temp.Count; i++)
            {
                StackPanel stackPanel = new StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal};
                int count = 0;

                List<Poem> poems = App._packageManager.Packages[i].Poems;
                for (int j = 0; j < poems.Count; j++)
                {
                    if (!dictionary.ContainsKey(poems[j].Id)) continue;
                    PoemStatus status = dictionary[poems[j].Id];
                    if (status == PoemStatus.PhaseTwo) count++;
                    else if (status == PoemStatus.PhaseThree) count += 2;
                    else if (status == PoemStatus.PhaseFour) count += 3;
                    else if (status == PoemStatus.Pass) count += 4;
                }
                count = count * 100 / (poems.Count * 4);

                Image image = new Image { Source = new BitmapImage(new Uri("/Image/appbar.download.rest.png", UriKind.Relative)), Width = 40 };
                TextBlock titleTextBlock = new TextBlock { Width = 200, Text = App._packageManager.PackageNames[i], TextWrapping = TextWrapping.Wrap, Foreground = new SolidColorBrush(Colors.Black), FontSize = 24 };
                TextBlock achieveTextBlock = new TextBlock { Width = 120, TextWrapping = TextWrapping.Wrap, Text = "已完成:" + count.ToString() + "%", FontSize = 18, HorizontalAlignment = System.Windows.HorizontalAlignment.Right, Foreground = new SolidColorBrush(Colors.Black) };

                stackPanel.Children.Add(image);
                stackPanel.Children.Add(titleTextBlock);
                stackPanel.Children.Add(achieveTextBlock);

                Border border = new Border { BorderThickness = new Thickness(0, 0, 0, 1), Width = 360, Margin = new Thickness(10,0,0,0), BorderBrush = new SolidColorBrush(Colors.Black) };
                border.Child = stackPanel;
                listBox.Items.Add(border);
            }
        }

        private int CheckIndex(int index)
        {
            if (index == -1) return -1;

            Border b = listBox.Items[index] as Border;
            if (b == null) return -1;
            else return index;
        }

        private bool CheckOpen(int pos)
        {
            if (pos + 1 >= listBox.Items.Count) return false;

            StackPanel b = listBox.Items[pos + 1] as StackPanel;
            if (b != null) return true;
            else return false;
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = listBox.SelectedIndex;
            int pos = CheckIndex(index);
            if (pos == -1) return;

            if (!CheckOpen(pos))
            {
                Border b = listBox.Items[pos] as Border;
                StackPanel s = b.Child as StackPanel;
                TextBlock titleTextBlock = s.Children[1] as TextBlock;
                string title = titleTextBlock.Text;


                for (index = 0; index < App._packageManager.PackageNames.Count; index++)
                    if (App._packageManager.PackageNames[index] == title) break;

                Image image = new Image { Source = new BitmapImage(new Uri("/Image/appbar.upload.rest.png", UriKind.Relative)), Width = 40 };
                s.Children[0] = image;
                AddPoemsInfo(index, pos);
            }
            else
            {
                listBox.Items.RemoveAt(pos + 1);

                Border b = listBox.Items[pos] as Border;
                StackPanel s = b.Child as StackPanel;
                Image image = new Image { Source = new BitmapImage(new Uri("/Image/appbar.download.rest.png", UriKind.Relative)), Width = 40 };
                s.Children[0] = image;
            }

            listBox.SelectedIndex = -1;
        }
    }
}