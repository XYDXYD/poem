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
using System.IO.IsolatedStorage;
using WindowsPhonePostClient;
using System.Windows.Threading;

namespace SiChi
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            //cloud1Move.Begin();

        }

        private void mediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            //mediaElement.Visibility = System.Windows.Visibility.Collapsed;
            //mediaElement.AutoPlay = false;
            LayoutRoot.Visibility = System.Windows.Visibility.Visible;      
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/PackageSelectPage.xaml", UriKind.Relative));
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/TotalTestSelectPage.xaml", UriKind.Relative));
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/LoginRegisterPage.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender1, RoutedEventArgs e1)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("userLast") && App._userManager.UserNow == null)
            {
                LoginAndRegister lg = new LoginAndRegister(settings["userLast"].ToString(), settings["passwordLast"].ToString());
                lg.Login(AutoLoginComplete, true);

            }
            else if (App._userManager.UserNow != null)
            {
                //button1.IsEnabled = button2.IsEnabled = button3.IsEnabled = button4.IsEnabled = button5.IsEnabled = button6.IsEnabled =  button7.IsEnabled = true;
                string userName = App._userManager.UserNow.Name;
                string status = App._userManager.GetStatus();
                string todaySpan = App._userManager.UserNow.TodaySpan.ToString();
                string totalSpan = App._userManager.UserNow.TotalSpan.ToString();
                string totalPass = App._userManager.UserNow.TotalPass.ToString();
                string totalRecite = App._userManager.UserNow.TotalRecite.ToString();
                string editTime = DateTime.Now.ToString();
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("user_name", userName);
                parameters.Add("status", status);
                parameters.Add("today_span", todaySpan);
                parameters.Add("total_span", totalSpan);
                parameters.Add("total_pass", totalPass);
                parameters.Add("total_recite", totalRecite);
                parameters.Add("edit_time", editTime);
                PostClient proxy = new PostClient(parameters);
                PostClient proxy1 = new PostClient(parameters);
                proxy.DownloadStringCompleted += (sender, e) =>
                {
                    if (e.Error == null && e.Result != "")
                    {
                        textBlock.Text = "用户信息同步成功！";
                        animation.Begin();
                        //MessageBox.Show("上传成功，" + e.Result);
                    }
                    else
                    {
                        textBlock.Text = "欢迎回来：" + App._userManager.UserNow.Name;
                        animation.Begin();
                        //MessageBox.Show("上传不成功，没网");
                    }
                };
                proxy.DownloadStringAsync(new Uri("http://1.winphonexiang.sinaapp.com/guwen/upload_information.php", UriKind.Absolute));
                proxy1.DownloadStringAsync(new Uri("http://appdev.sysu.edu.cn/~mad/10389233/shici/upload_information.php", UriKind.Absolute));
            }
            else
            {
                NavigationService.Navigate(new Uri("/View/LoginRegisterPage.xaml", UriKind.Relative));
            }
        }

        void AutoLoginComplete()
        {
            //button1.IsEnabled = button2.IsEnabled = button3.IsEnabled = button4.IsEnabled = button5.IsEnabled = button6.IsEnabled = button7.IsEnabled = true;
            textBlock.Text = "欢迎回来：" + App._userManager.UserNow.Name;
            animation.Begin();
        }

        //出，更新本地用户累计时间
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {

            var settings = IsolatedStorageSettings.ApplicationSettings;
            settings["accTime"] = DateTime.Now;

            base.OnNavigatedFrom(e);
        }

        //页面跳入时，更新本地用户累计时间
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            if (App._userManager.UserNow != null)
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                DateTime before = (DateTime)settings["accTime"];
                DateTime now = DateTime.Now;
                int acc = (now - before).Minutes;
                App._userManager.UserNow.TotalSpan += acc;
                settings["accTime"] = now;
            }

            base.OnNavigatedTo(e);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/RankPage.xaml", UriKind.Relative));
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/SearchPage.xaml", UriKind.Relative));
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/PostsListPage.xaml", UriKind.Relative));
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/UserInformation.xaml", UriKind.Relative));
        }

        private void Button_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            ((Image)sender).Margin = new Thickness(((Image)sender).Margin.Left + 3, ((Image)sender).Margin.Top + 3, ((Image)sender).Margin.Right + 3, ((Image)sender).Margin.Bottom + 3);
        }

        private void Button_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            ((Image)sender).Margin = new Thickness(((Image)sender).Margin.Left - 3, ((Image)sender).Margin.Top - 3, ((Image)sender).Margin.Right - 3, ((Image)sender).Margin.Bottom - 3);
        }
    }
}