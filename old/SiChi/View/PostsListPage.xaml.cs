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
using org.phprpc;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SiChi.View
{
    public partial class PostsListPage : PhoneApplicationPage
    {
        public PostsListPage()
        {
            InitializeComponent();
        }
        JObject result;
        protected override void  OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            PHPRPC_Client client = new PHPRPC_Client("http://1.winphonexiang.sinaapp.com/guwen/forum.class.php");
            client.Invoke("getPostList", new Object[] { "0" }, callback); 
        }

        public void callback(Object Result, Object[] args, String output, PHPRPC_Error warning)
        {
            if (warning != null)
            {
                MessageBox.Show("网络连接错误");
                return;
            }
            postsList.Items.Clear();
            byte [] a = Result as byte[];
            string json = Encoding.UTF8.GetString( a,0,a.Length);
            result = JObject.Parse(json);
            foreach (var i in result["result"])
            {
                string title = i["title"].ToString();
                string author = i["uid"].ToString();
                string replyNum = i["reply_num"].ToString();
                string time = i["update_time"].ToString();

                TextBlock txtAuthor = new TextBlock();
                txtAuthor.Text = author + "：";

                TextBlock txtTitle = new TextBlock();
                txtTitle.Text = title;
                txtTitle.TextWrapping = TextWrapping.Wrap;

                TextBlock txtNum = new TextBlock();
                txtNum.Text = "回复（" + replyNum + "）";
                txtNum.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                TextBlock txtTime = new TextBlock();
                txtTime.Text = time;
                txtTime.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

                Grid grid = new Grid();
                grid.Children.Add(txtNum);
                grid.Children.Add(txtTime);
                grid.Width = 445;

                Border border = new Border();
                border.Child = grid;
                border.BorderThickness = new Thickness(0, 0, 0, 1);
                border.BorderBrush = new SolidColorBrush(Colors.Gray);

                StackPanel sp = new StackPanel();
                sp.Children.Add(txtAuthor);
                sp.Children.Add(txtTitle);
                sp.Children.Add(border);

                postsList.Items.Add(sp);
            }
        }

        private void sendPost_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/PostingPage.xaml", UriKind.Relative));
        }

        private void postsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedIndex == -1)
            {
                return;
            }
            int index = ((ListBox)sender).SelectedIndex;
            string id = result["result"][index]["id"].ToString();
            string title = result["result"][index]["title"].ToString();
            string content = result["result"][index]["content"].ToString();
            string uid = result["result"][index]["uid"].ToString();
            string updateTime = result["result"][index]["update_time"].ToString().Replace(" ", "!");

            NavigationService.Navigate(new Uri("/View/ReplyPage.xaml?id=" + id + "&title=" + title + "&content=" + content + "&uid=" + uid + "&update_time=" + updateTime, UriKind.Relative));
        }


    }
}