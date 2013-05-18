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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using WindowsPhonePostClient;
using WeiboSdk;
using WeiboSdk.PageViews;
using System.Diagnostics;
using System.IO.IsolatedStorage;


namespace SiChi.View
{
    public partial class PoemPage : PhoneApplicationPage
    {
        private Poem poem;

        private bool _exit = false;

        private bool haveToken;

        public PoemPage()
        {
            InitializeComponent();
            FrameworkDispatcher.Update();
            leaf1Ymove.Begin();
            leaf1Xmove.Begin();
            leaf2Xmove.Begin();
            leaf2Ymove.Begin();
            leaf3Xmove.Begin();
            leaf3Ymove.Begin();
            leaf4Xmove.Begin();
            leaf4Ymove.Begin();
            leaf5Xmove.Begin();
            leaf5Ymove.Begin();
            haveToken = false;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            poem = App._packageManager.Packages[App._packageIndex].Poems[App._poemIndex];
            titleTextBlock.Text = poem.Title;
            authorTextBlock.Text = poem.Author;
            contentTextBlock.Text = HandleContent(poem.Content);

            //若本诗歌未看过，则设置为看过
            string id = poem.Id;
            User user = App._userManager.UserNow;
            if (user.FindPoemStatus(id) == PoemStatus.NotViewed)
                user.UpdatePoem(id, PoemStatus.Viewed);

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            _exit = true;
            if (_sound != null) _sound.Dispose();
            base.OnNavigatedFrom(e);
        }

        //小测试
        private void OnReciteButtonClicked(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/RecitePage.xaml", UriKind.Relative));
        }

        //跳过该诗歌
        private void OnNextButtonClicked(object sender, EventArgs e)
        {

            if (_sound != null) _sound.Dispose();

            int index = App._poemIndex;
            Package package = App._packageManager.Packages[App._packageIndex];
            index++;

            if (index == package.Poems.Count) index = 0;

            App._poemIndex = index;
            poem = package.Poems[index];

            titleTextBlock.Text = poem.Title;
            authorTextBlock.Text = poem.Author;
            contentTextBlock.Text = HandleContent(poem.Content);

            string id = poem.Id;
            User user = App._userManager.UserNow;
            if (user.FindPoemStatus(id) == PoemStatus.NotViewed)
                user.UpdatePoem(id, PoemStatus.Viewed);
        }

        //获取详细资料
        private void OnDescribeButtonClicked(object sender, EventArgs e)
        { 
            this.NavigationService.Navigate(new Uri("/View/PoemDetailPage.xaml", UriKind.Relative));
        }

        //阅读
        private SoundEffect _sound;
        private void OnReadButtonClicked(object sender1, EventArgs e1)
        {
            Button button = sender1 as Button;
            System.Diagnostics.Debug.WriteLine(button.ToString());
            if (button.Content.ToString() == "朗读")
            {
                button.Content = "请稍候";
                if (_sound != null) _sound.Dispose();

                var applictionid = "5B75316E24BE0E1E19DE874CE806DD064AFAC5EA";
                var languageCode = "zh-cn";
                var objTranslator = new TranslatorService.LanguageServiceClient();
                objTranslator.SpeakCompleted += translator_SpeakCompleted;

                string text = titleTextBlock.Text + "\n" + authorTextBlock.Text + "\n" + contentTextBlock.Text;

                //测试网络
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("q", "0");
                PostClient proxy = new PostClient(parameters);

                proxy.DownloadStringCompleted += (sender, e) =>
                {
                    if (e.Error == null && e.Result == "200")
                    {
                        //有网则读
                        objTranslator.SpeakAsync(applictionid, text, languageCode, "audio/wav", string.Empty);                        
                        
                    }
                    else
                    {
                        MessageBox.Show("请连接网络");
                    }
                };
                proxy.DownloadStringAsync(new Uri("http://appdev.sysu.edu.cn/~mad/10389233/NetWorkTest.php", UriKind.Absolute));
            }
            else button.Content = "朗读";


            
        }


        void translator_SpeakCompleted(object sender, TranslatorService.SpeakCompletedEventArgs e)
        {
            if (!_exit)
            {
                var client = new WebClient();
                client.OpenReadCompleted += ((s, args) =>
                {
                    if (args != null && args.Error == null && args.Result != null)
                    {
                        _sound = SoundEffect.FromStream(args.Result);
                        _sound.Play();
                    }
                    else return;
                });
                client.OpenReadAsync(new Uri(e.Result));
                client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            }
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {

            button2.Content = "朗读";
        }


        private string HandleContent(string content)
        {
            List<string> clauses = GetClauses(content);
            List<char> punctuates = GetPunctuates(content);
            string result = "";
            int number = 0;

            if (IsEqualPoem(clauses))
            {
                int length = GetPoemLength(clauses);

                for (int i = 0; i < clauses.Count; i++)
                {
                    result += clauses[i];
                    result += punctuates[number++];

                    if ((i + 1) % 2 == 0)
                    {
                        result += '\n';
                    }

                    if (punctuates.Count != number && punctuates[number] == '\n') number++;

                }

            }
            else
            {
                result = content;
            }

            return result;
        }


        private int GetPoemLength(List<string> input)
        {
            return input[0].Length;
        }

        private bool IsEqualPoem(List<string> input)
        { 
            int number = input[0].Length;
            bool flag = true;

            for (int i = 0; i < input.Count; i++)
            {
                if (number != input[i].Length)
                {
                    flag = false;
                    break;
                }
            }

            return flag;
        }

        private List<string> GetClauses(string content)
        {
            string[] clausesTemp = content.Split(new Char[] { '。', '，', '！', '？', ',', '.', '?', '!', '\n', ';', '；' });
            List<string> clauses = new List<string>();

            for (int i = 0; i < clausesTemp.Length; i++)
            {
                clausesTemp[i] = clausesTemp[i].Trim();
                if (clausesTemp[i] != "") clauses.Add(clausesTemp[i]);
            }

            return clauses;
        }

        private List<char> GetPunctuates(string content)
        {
            List<char> punctuates = new List<char>();
            char tempChar;
            for (int i = 0; i < content.Length; i++)
            {
                tempChar = content[i];
                if (tempChar == '。' || tempChar == '，' || tempChar == '！' || tempChar == '？'
                    || tempChar == '.' || tempChar == ',' || tempChar == '!' || tempChar == '?' || tempChar == '\n' || tempChar == '；'
                    || tempChar == ';')
                    punctuates.Add(tempChar);
            }
            return punctuates;
        }

        private void shareButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
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
                        Message = titleTextBlock.Text + "（" + authorTextBlock.Text + "）" + contentTextBlock.Text + "——来自win phone应用“诗趣”",
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
                        Message = titleTextBlock.Text + "（" + authorTextBlock.Text + "）" + contentTextBlock.Text + "——来自win phone应用“诗趣”",
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