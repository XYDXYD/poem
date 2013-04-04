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
using System.Text;

namespace SiChi.View
{
    public partial class TotalRankPage : PhoneApplicationPage
    {
        public TotalRankPage()
        {
            InitializeComponent();
            pageNumber = 0;
        }
        private int pageNumber;

        private void lastPage_Click(object sender)
        {
            if (pageNumber > 0)
            {
                pageNumber--;
                LoadData();
            }
        }

        private void nextPage_Click(object sender)
        {
            pageNumber++;
            LoadData();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }


        private void LoadData()
        {
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
            wc.DownloadStringAsync(new Uri("http://1.winphonexiang.sinaapp.com/guwen/total_rank.php?page=" + pageNumber.ToString()));
            wc.Encoding = Encoding.UTF8;
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("请连接网络~");
            }
            else
            {
                if (!e.Result.Contains('$'))
                {
                    pageNumber--;
                    MessageBox.Show("已经是最后一页");
                }
                else
                {
                    itemsStack.Children.Clear();
                    string[] items = e.Result.Split('#');
                    foreach (var i in items)
                    {
                        string name = i.Split('$')[0];
                        string passed = i.Split('$')[1];
                        string recited = i.Split('$')[2];
                        TextBlock tbName = new TextBlock();
                        tbName.Text = "用户名：" + name;
                        tbName.Foreground = new SolidColorBrush(Colors.Black);

                        TextBlock tbAchieve = new TextBlock();
                        tbAchieve.Text = "通过数：" + passed + "      背诵数：" + recited;
                        tbAchieve.Foreground = new SolidColorBrush(Colors.Black);

                        StackPanel sp = new StackPanel();
                        sp.Children.Add(tbName);
                        sp.Children.Add(tbAchieve);

                        Border border = new Border();
                        border.Child = sp;
                        border.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 243));
                        border.BorderThickness = new Thickness(0, 0, 0, 1);

                        itemsStack.Children.Add(border);
                    }
                }
            }
        }

        private void lastPage_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            ((Button)sender).Width -= 10;
            ((Button)sender).Height -= 10;
            e.Handled = true;
        }

        private void lastPage_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            ((Button)sender).Width += 10;
            ((Button)sender).Height += 10;
            if (((Button)sender).Name == "nextPage")
            {
                nextPage_Click(sender);
            }
            else
            {
                lastPage_Click(sender);
            }
        }

          
    }
}