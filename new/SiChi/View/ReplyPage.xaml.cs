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
using Newtonsoft.Json.Linq;
using System.Text;

namespace SiChi.View
{
    public partial class ReplyPage : PhoneApplicationPage
    {
        public ReplyPage()
        {
            InitializeComponent();
        }

        string postId;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            postId = NavigationContext.QueryString["id"];
            string title = NavigationContext.QueryString["title"];
            string content = NavigationContext.QueryString["content"];
            string uid = NavigationContext.QueryString["uid"];
            string updateTime = NavigationContext.QueryString["update_time"].Replace("!", " ");

            authorText.Text = uid + ":";
            titleText.Text = title;
            contentText.Text = content;
            timeText.Text = updateTime;

            PHPRPC_Client client = new PHPRPC_Client("http://1.winphonexiang.sinaapp.com/guwen/forum.class.php");
            string input = postId;
            client.Invoke("getReply", new Object[] { input }, Callback);
            base.OnNavigatedTo(e);
        }

        void Callback(Object result, Object[] args, String output, PHPRPC_Error warning)
        {
            if (warning != null)
            {
                MessageBox.Show("网络连接错误~");
                return;
            }
            byte[] temp = result as byte[];
            string json = Encoding.UTF8.GetString(temp, 0, temp.Length);

            JObject input = JObject.Parse(json);

            replyList.Children.Clear();
            var list = input["result"];
            foreach (var i in list)
            {
                TextBlock user = new TextBlock();
                user.Text = i["uid"].ToString() + ":";
                user.FontSize = 22;
                user.Foreground = new SolidColorBrush(Colors.Black);

                TextBlock content = new TextBlock();
                content.FontSize = 18;
                content.Text = i["content"].ToString();
                content.TextWrapping = TextWrapping.Wrap;
                content.Foreground = new SolidColorBrush(Colors.Black);

                TextBlock time = new TextBlock();
                time.Text = i["time"].ToString();
                time.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                time.FontSize = 16;
                time.Foreground = new SolidColorBrush(Colors.Black);

                StackPanel sp = new StackPanel();
                sp.Width = 430;
                sp.Children.Add(user);
                sp.Children.Add(content);
                sp.Children.Add(time);

                Border border = new Border();
                border.BorderBrush = new SolidColorBrush(Colors.White);
                border.BorderThickness = new Thickness(0, 0, 0, 1);
                border.Child = sp;

                replyList.Children.Add(border);
            }

            replyGrid.Visibility = Visibility.Collapsed;
        }

        private void replyBtn_Click(object sender, RoutedEventArgs e)
        {
            replyGrid.Visibility = Visibility.Visible;
            /*PHPRPC_Client client = new PHPRPC_Client("http://1.winphonexiang.sinaapp.com/guwen/forum.class.php");
            string input = postId;
            client.Invoke("Reply", new Object[] { postId, App._userManager.UserNow.Name,  }, Callback);*/
        }

        private void submitBtn_Click(object sender, RoutedEventArgs e)
        {
            PHPRPC_Client client = new PHPRPC_Client("http://1.winphonexiang.sinaapp.com/guwen/forum.class.php");
            string input = postId;
            client.Invoke("Reply", new Object[] { postId, App._userManager.UserNow.Name, replyText.Text.Trim() }, Callback1);
        }
        void Callback1(Object result, Object[] args, String output, PHPRPC_Error warning)
        {
            if (warning != null)
            {
                MessageBox.Show("网络连接错误~");
                return;
            }
            byte[] temp = result as byte[];
            string res = Encoding.UTF8.GetString(temp, 0, temp.Length);

            if (res == "1")
            {
                PHPRPC_Client client = new PHPRPC_Client("http://1.winphonexiang.sinaapp.com/guwen/forum.class.php");
                string input = postId;
                client.Invoke("getReply", new Object[] { input }, Callback);
            }
            else
            {
                MessageBox.Show("服务器错误");
                replyGrid.Visibility = Visibility.Collapsed;
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (replyGrid.Visibility == Visibility.Visible)
            {
                replyGrid.Visibility = Visibility.Collapsed;
                e.Cancel = true;
            }
            base.OnBackKeyPress(e);
        }
    }
}