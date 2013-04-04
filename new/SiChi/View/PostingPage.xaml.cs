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

namespace SiChi.View
{
    public partial class PostingPage : PhoneApplicationPage
    {
        public PostingPage()
        {
            InitializeComponent();
        }

        private void buttonSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxContent.Text.Trim() == "" || textBoxTitle.Text.Trim() == "")
            {
                MessageBox.Show("请输入内容~");
                return;
            }
            progressBar.Visibility = Visibility.Visible;
            PHPRPC_Client client = new PHPRPC_Client("http://1.winphonexiang.sinaapp.com/guwen/forum.class.php");
            client.Invoke("Posting", new Object[] {App._userManager.UserNow.Name, textBoxTitle.Text, textBoxContent.Text }, callback);
        }

        public void callback(Object Result, Object[] args, String output, PHPRPC_Error warning)
        {
            progressBar.Visibility = Visibility.Collapsed;
            byte[] a = Result as byte[];
            if (Encoding.UTF8.GetString(a, 0, a.Length) == "1")
            {
                MessageBox.Show("发表成功");
                if (this.NavigationService.CanGoBack) 
                    this.NavigationService.GoBack();
            }
            else
                MessageBox.Show("发表失败");
        }

        private void textBoxContent_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) //判断按键，执行代码 
            {
                this.Focus(); //隐藏虚拟键盘，回到程序界面 
            } 
        }
    }
}