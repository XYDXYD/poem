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
using System.IO;
using System.Runtime.Serialization.Json;
using Microsoft.Phone.Net.NetworkInformation;
//using GB2312;
using System.Collections;
using System.Runtime.Serialization;

namespace SiChi.Model
{
    public class GetData
    {
        public delegate void GetStringComplete(string result);
        public delegate void GetDictComplete(IDictionary<string, string> result);


        //这里是获取某分类中的titles
        public class TitleAndId
        {
            public string id { get; set; }
            public string title { get; set; }
        }
        GetDictComplete GetTitlesViaCategoryComplete;
        public void GetTitlesViaCategory(string Category, GetDictComplete DataComplete)
        {
            string CategoryCpy = Category;
            GetTitlesViaCategoryComplete = DataComplete;
            WebClient WebGetTitles = new WebClient();
            WebGetTitles.Encoding = Encoding.UTF8;
            WebGetTitles.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WebGetTitles_DownloadStringCompleted);
            WebGetTitles.DownloadStringAsync(new Uri("http://1.winphonexiang.sinaapp.com/guwen/get_titles_via_class.php?class=" + CategoryCpy));
        }

        void WebGetTitles_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                IDictionary<string, string> ReturnData = new Dictionary<string, string>();
                ReturnData["null"] = "连接失败，请检查网络。";
                GetTitlesViaCategoryComplete(ReturnData);
            }
            else
            {
                List<TitleAndId> list = new List<TitleAndId>();
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(e.Result));
                DataContractJsonSerializer ser = new DataContractJsonSerializer(list.GetType());
                list = (List<TitleAndId>)ser.ReadObject(ms);
                ms.Close();
                IDictionary<string, string> ReturnData = new Dictionary<string, string>();
                foreach (var i in list)
                {
                    ReturnData[i.id] = i.title;
                }
                GetTitlesViaCategoryComplete(ReturnData);
            }
        }


        //这里是获取题目,作者,内容，换行用$
        GetStringComplete GetAllViaIdComplete;
        public void GetTitleNameContentViaId(string Id, GetStringComplete DataComplete)
        {
            GetAllViaIdComplete = DataComplete;
            WebClient WebGetAll = new WebClient();
            WebGetAll.Encoding = Encoding.UTF8;
            WebGetAll.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WebGetAll_DownloadStringCompleted);
            //System.Threading.Thread.Sleep(100);
            WebGetAll.DownloadStringAsync(new Uri("http://1.winphonexiang.sinaapp.com/guwen/get_author_and_content_via_id.php?id=" + Id));
        }

        void WebGetAll_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                GetAllViaIdComplete("downloadError");
            }
            else
            {
                GetAllViaIdComplete(e.Result);
            }
        }


        //获得某文章的英文
        GetStringComplete GetEngViaIdComplete;
        public void GetEnglishViaId(string Id, GetStringComplete DataComplete)
        {
            GetEngViaIdComplete = DataComplete;
            WebClient WebGetEnglish = new WebClient();
            WebGetEnglish.Encoding = Encoding.UTF8;
            WebGetEnglish.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WebGetEnglist_DownloadStringCompleted);
            WebGetEnglish.DownloadStringAsync(new Uri("http://1.winphonexiang.sinaapp.com/guwen/get_english_via_id.php?id=" + Id));
        }

        void WebGetEnglist_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                GetEngViaIdComplete("连接失败，请检查网络。");
            }
            else
            {
                GetEngViaIdComplete(e.Result);
            }
        }


        //获得作者的生平事迹
        GetStringComplete GetLifeViaAuthorComplete;
        public void GetLifeViaAuthor(string Author, GetStringComplete DataComplete)
        {
            /*string AuthorCpy = "";
            byte[] b = Encoding.UTF8.GetBytes(Author);
            foreach (int i in b)
            {
                AuthorCpy += "%";
                AuthorCpy += Convert.ToString(i, 16);
            }*/
            GetLifeViaAuthorComplete = DataComplete;
            WebClient WebGetLife = new WebClient();
            WebGetLife.Encoding = UTF8Encoding.UTF8;
            WebGetLife.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WebGetLife_DownloadStringCompleted);
            WebGetLife.DownloadStringAsync(new Uri("http://1.winphonexiang.sinaapp.com/guwen/get_about_via_author.php?author=" + Author));
        }

        void WebGetLife_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                GetLifeViaAuthorComplete("连接失败，请检查网络。");
            }
            else
            {
                GetLifeViaAuthorComplete(e.Result);
            }
        }



        //获得创作背景
        GetStringComplete GetBgViaIdComplete;
        public void GetBackgroundViaId(string Id, GetStringComplete DataComplete)
        {
            GetBgViaIdComplete = DataComplete;
            WebClient WebGetBg = new WebClient();
            WebGetBg.Encoding = Encoding.UTF8;
            WebGetBg.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WebGetBg_DownloadStringCompleted);
            WebGetBg.DownloadStringAsync(new Uri("http://1.winphonexiang.sinaapp.com/guwen/get_background_via_id.php?id=" + Id));
        }

        void WebGetBg_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                GetBgViaIdComplete("连接失败，请检查网络。");
            }
            else
            {
                GetBgViaIdComplete(e.Result);
            }
        }

        GetStringComplete GetBgViaPoemComplete;
        public void GetBackgroundViaPoem(string title, string author, string package, GetStringComplete DataComplete)
        {
            string AuthorCpy = "";
            byte[] b = Encoding.UTF8.GetBytes(author);
            foreach (int i in b)
            {
                AuthorCpy += "%";
                AuthorCpy += Convert.ToString(i, 16);
            }
            string TitleCpy = "";
            byte[] bb = Encoding.UTF8.GetBytes(title);
            foreach (int i in bb)
            {
                TitleCpy += "%";
                TitleCpy += Convert.ToString(i, 16);
            }
            GetBgViaPoemComplete = DataComplete;
            WebClient WebGetBg = new WebClient();
            WebGetBg.Encoding = Encoding.UTF8;
            WebGetBg.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WebGetBg_DownloadStringCompleted1);
            WebGetBg.DownloadStringAsync(new Uri("http://1.winphonexiang.sinaapp.com/guwen/get_background_via_poem.php?title=" + TitleCpy + "&author=" + AuthorCpy + "&package=" + package));
        }

        void WebGetBg_DownloadStringCompleted1(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                GetBgViaPoemComplete("连接失败，请检查网络。");
            }
            else
            {
                GetBgViaPoemComplete(e.Result);
            }
        }


        //获得中文解释
        GetStringComplete GetParaViaPoemComplete;
        public void GetParaphraseViaPoem(string id, GetStringComplete DataComplete)
        {
            GetParaViaPoemComplete = DataComplete;
            WebClient WebGetPara = new WebClient();
            WebGetPara.Encoding = Encoding.UTF8;
            WebGetPara.DownloadStringCompleted += new DownloadStringCompletedEventHandler(WebGetPara_DownloadStringCompleted);
            WebGetPara.DownloadStringAsync(new Uri("http://1.winphonexiang.sinaapp.com/guwen/get_paraphrase_via_poem.php?id=" + id ));
        }

        void WebGetPara_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                GetParaViaPoemComplete("连接失败，请检查网络。");
            }
            else
            {
                GetParaViaPoemComplete(e.Result);
            }
        }
    }
}
