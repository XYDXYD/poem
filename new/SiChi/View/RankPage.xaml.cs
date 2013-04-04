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
using WeiboSdk;
using WeiboSdk.PageViews;
using System.Diagnostics;

namespace SiChi.View
{
    public partial class RankPage : PhoneApplicationPage
    {
        public RankPage()
        {
            InitializeComponent();
        }

        private void rankButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/TotalRankPage.xaml", UriKind.Relative));
        }

        private void IterateThroughChildren(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                if (VisualTreeHelper.GetChild(parent, i) is TextBlock)              //判断控件是不是文本框类型的如果是执行相应的操作
                {
                    TextBlock textblock = VisualTreeHelper.GetChild(parent, i) as TextBlock;
                    textblock.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                }
                else if (VisualTreeHelper.GetChildrenCount(VisualTreeHelper.GetChild(parent, i)) > 0)      // 判断该控件是否有下属控件。   
                {
                    IterateThroughChildren(VisualTreeHelper.GetChild(parent, i));    //递归，访问该控件的下属控件集。              
                }
            }
        }     

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            IterateThroughChildren(LayoutRoot);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
            wc.DownloadStringAsync(new Uri("http://1.winphonexiang.sinaapp.com/guwen/rank_info.php?user_name=" + App._userManager.UserNow.Name));
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("请连接网络~");
            }
            else
            {
                string[] result = e.Result.Split('$');
                rankTextBlock.Text = result[0];
                percentageTextBlock.Text = ((1 - double.Parse(result[1]))*100).ToString() + "%";
            }
        }

        string content;
        private void image1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            content = "我在“诗趣”中的排名为：" + rankTextBlock.Text + "。击败了全国" + percentageTextBlock.Text + "的用户。 “诗趣”——古诗词爱好者必备应用~"; 
            SdkData.AppKey = "2774680166";
            SdkData.AppSecret = "bf219ef464ea77bbcc9760ba7f3c3b1b";
            SdkData.RedirectUri = "http://xyd.cn.mu";

            AuthenticationView.OAuth2VerifyCompleted = (e1, e2, e3) => VerifyBack(e1, e2, e3);

            //AuthenticationView.OBrowserCancelled = (e1, e2) => a(e1, e2);

            
            //其它通知事件...
            if (App._accessToken == "")
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {

                    NavigationService.Navigate(new Uri("/WeiboSdk;component/PageViews/AuthenticationView.xaml", UriKind.Relative));

                });
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    ClientOAuth.RefleshAccessToken(App._refleshToken, (e1, e2, e3) =>
                    {

                        if (true == e1)
                        {
                            App._accessToken = e3.accesssToken;
                            App._refleshToken = e3.refleshToken;
                        }

                        else
                        {
                            if (e2.errCode == SdkErrCode.NET_UNUSUAL)
                            {
                            }

                            else if (e2.errCode == SdkErrCode.SERVER_ERR)

                                Debug.WriteLine("服务器返回错误，错误码:" + e2.specificCode);

                        }

                    });

                    SdkShare sdkShare = new SdkShare
                    {
                        AccessToken = App._accessToken,
                        //PicturePath = "/Image/LOGO.png",
                        Message = content
                    };

                    sdkShare.Completed = new EventHandler<SendCompletedEventArgs>(ShareCompleted1);

                    sdkShare.Show();
                });
            }
        }

        private void VerifyBack(bool isSucess, SdkAuthError errCode, SdkAuth2Res response)
        {
            if (errCode.errCode == SdkErrCode.SUCCESS)
            {
                if (null != response)
                {
                    App._accessToken = response.accesssToken;
                    App._refleshToken = response.refleshToken;
                }
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    SdkShare sdkShare = new SdkShare
                    {
                        AccessToken = App._accessToken,
                        //PicturePath = "/Image/LOGO.png",
                        Message = content
                    };

                    sdkShare.Completed = new EventHandler<SendCompletedEventArgs>(ShareCompleted);

                    sdkShare.Show();
                });

            }
            else if (errCode.errCode == SdkErrCode.NET_UNUSUAL)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("检查网络");
                });
            }
            else if (errCode.errCode == SdkErrCode.SERVER_ERR)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("服务器返回错误，错误代码:" + errCode.specificCode);
                });
            }
            else
                Debug.WriteLine("Other Err.");
        }

        void ShareCompleted(object sender, SendCompletedEventArgs e)
        {

            if (e.IsSendSuccess)
            {

            }
            else
                MessageBox.Show(e.Response, e.ErrorCode.ToString(), MessageBoxButton.OK);

        }

        void ShareCompleted1(object sender, SendCompletedEventArgs e)
        {

            if (e.IsSendSuccess)
            {
                MessageBox.Show("分享成功");
            }
            else
                MessageBox.Show(e.Response, e.ErrorCode.ToString(), MessageBoxButton.OK);

        }

    }
}