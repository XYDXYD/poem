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
    public partial class LoginRegisterPage : PhoneApplicationPage
    {
        public LoginRegisterPage()
        {
            InitializeComponent();
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            pivot.SelectedItem = registerPivot;
        }

        private void returnButton_Click(object sender, RoutedEventArgs e)
        {
            pivot.SelectedItem = loginPivot;
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            string passwd1 = passwdBox1.Password.Trim();
            string passwd2 = passwdBox2.Password.Trim();
            string userName = nameBoxReg.Text.Trim();

            if (passwd1 == "" || passwd2 == "" || userName == "")
                MessageBox.Show("输入不能为空");
            else if (passwd1 != passwd2)
            {
                MessageBox.Show("两次输入的密码不一致。");
            }
            else
            {
                LoginAndRegister lg = new LoginAndRegister(userName, passwd1);
                lg.Register(registerComplete);
            }
        }
        void registerComplete()
        {
            pivot.SelectedItem = loginPivot;
            usernameBox.Text = nameBoxReg.Text.Trim();
        }


        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            string userName = usernameBox.Text.Trim();
            string passwd = passwdBox.Password.Trim();

            if (userName == "" || passwd == "")
                MessageBox.Show("输入不能为空");
            else
            {
                LoginAndRegister lg = new LoginAndRegister(userName, passwd);
                lg.Login(loginComplete);
            }
        }

        void loginComplete()
        {
            NavigationService.GoBack();
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (App._userManager.UserNow == null) e.Cancel = true;
        }

        private void usernameBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) //判断按键，执行代码 
            {
                this.Focus();
            }
        }
    }
}