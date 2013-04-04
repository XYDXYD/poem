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
    public partial class PoemDetailPage : PhoneApplicationPage
    {
        private Poem poem;
        private GetData data;

        public PoemDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            poem = App._packageManager.Packages[App._packageIndex].Poems[App._poemIndex];
            data = new GetData();
            GetAuthor();
            base.OnNavigatedTo(e);
        }

        //获取诗歌背景
        private void GetBackground()
        {
            data.GetBackgroundViaId(poem.Id, GetBackgroundComplete);
        }
        private void GetBackgroundComplete(string result)
        {
            backgroundText.Text = "      " + result;
        }
        //获取作者信息
        private void GetAuthor()
        {
            data.GetLifeViaAuthor(poem.Author, GetAuthorComplete);
        }
        private void GetAuthorComplete(string result)
        {
            authorText.Text = "      " + result;
            GetBackground();
            GetParaphrase();
        }
        //获取诗词解析
        private void GetParaphrase()
        {
            data.GetParaphraseViaPoem(poem.Id, GetParaphraseComplete);
        }
        private void GetParaphraseComplete(string result)
        {
            paraphraseText.Text = "      " + result;
        }


        
    }
}