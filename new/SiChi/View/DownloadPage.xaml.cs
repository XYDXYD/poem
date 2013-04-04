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
//using GB2312;
using System.Threading;
using System.Text;
using SiChi.Model;


namespace SiChi.View
{
    public partial class DownloadPage : PhoneApplicationPage
    {
        public DownloadPage()
        {
            InitializeComponent();
            page_opened = true;
        }

        private bool page_opened; //当前页是否处于打开状态，如果没打开则不再下载
        private GetData Data;
       

        private List<string> poems;
        private int num_of_poems;
        private string package_name;
        //private string item_name; //中文名加packa_name


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            WebClient WebGetPackageList = new WebClient();
            WebGetPackageList.Encoding = Encoding.UTF8;
            WebGetPackageList.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WebGetPackageList_DownloadStringCompleted);
            WebGetPackageList.DownloadStringAsync(new Uri("http://1.winphonexiang.sinaapp.com/guwen/get_packages.php"));

        }

        void WebGetPackageList_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Error != null)
            {
                MessageBox.Show("连接失败，请检查网络。");
            }
            else
            {
                List<string> items = new List<string>();
                string result = e.Result;
                string[] tmp_items = result.Split('#');

                foreach (var i in tmp_items)
                {
                    string[] name_and_package = i.Split('$');
                    items.Add(name_and_package[1] + '-' + name_and_package[0]);
                }
                listBoxItems.DataContext = items;
                listBoxItems.SelectedIndex = -1;
            }
        }

        private void listBoxItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedIndex == -1 || stackPanelProgressBar.Visibility == Visibility.Visible) return;
            Data = new Model.GetData();
            
            poems = new List<string>();
            num_of_poems = 0;
            package_name = (sender as ListBox).SelectedItem.ToString().Split('-')[0];
            if (App._packageManager.PackageNames.IndexOf(package_name) == -1)
            {
                showAnimation.Begin();
                stackPanelProgressBar.Visibility = Visibility.Visible;
                LoadTitle(package_name);
            }
            else
                MessageBox.Show("该教材已存在！");
        }


        private void LoadTitle(string package_name)
        {
            Data.GetTitlesViaCategory(package_name, LoadTitleComplete);
        }
        private void LoadTitleComplete(IDictionary<string, string> result)
        {
            num_of_poems = result.Count;

            foreach (var i in result)
            {
                Data.GetTitleNameContentViaId(i.Key.ToString(), LoadContentComplete);
            }
        }
        private void LoadContentComplete(string result)
        {
            poems.Add(result);

            downloadProgressBar.Value = poems.Count * 100 / num_of_poems;
            if (poems.Count == num_of_poems && page_opened == true)
            {
                if (poems.IndexOf("downloadError") != -1)
                {
                    MessageBox.Show("网络故障，下载失败。");
                }
                else
                {
                    poems.Sort();
                    
                    //Database.StorePackage(poems, "Package" + package_name);
                    //Database.UpdatePackageList(item_name);
                    List<Poem> poemsObj = new List<Poem>();
                    foreach (var i in poems)
                    {
                        poemsObj.Add(Package.ConvertStringToPoem(i));
                    }
                    Package package = new Package(package_name, poemsObj);
                    App._packageManager.Add(package);
                }
                listBoxItems.SelectedIndex = -1;
                downloadProgressBar.Value = 0;
                downAnimation.Begin();
                stackPanelProgressBar.Visibility = Visibility.Collapsed;
            }

        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            this.page_opened = false;
            base.OnNavigatedFrom(e);
        }
    }
}