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
using SiChi.View;
using SiChi.Model;
using System.IO.IsolatedStorage;

namespace SiChi.View
{
    public partial class RecitePage : PhoneApplicationPage
    {
        private Poem poem;
        public RecitePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            stackPanel.Children.Clear();
            poem = App._packageManager.Packages[App._packageIndex].Poems[App._poemIndex];
            AddTitleAndAuthorTextBlock(poem.Title, poem.Author);
            AddContentTextBlock(poem.Content);

            base.OnNavigatedTo(e);
        }

        private void AddTitleAndAuthorTextBlock(string title,string author)
        {
            TextBlock titleTextBlock = new TextBlock { Text = title, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, FontSize = 32, Foreground = new SolidColorBrush(Colors.Black) };
            TextBlock authorTextBlock = new TextBlock { Text = author, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, FontSize = 20, Foreground = new SolidColorBrush(Colors.Black) };
            stackPanel.Children.Add(titleTextBlock);
            stackPanel.Children.Add(authorTextBlock);
        }

        private void AddContentTextBlock(string content)
        {

            List<string> clauses = GetClauses(content);
            List<char> punctuates = GetPunctuates(content);
            bool[] clausesStatus = GetClausesStatus(clauses.Count);

            int punctuateNumber = 0;

            
            for (int i = 0; i < clauses.Count; i++)
            {
                StackPanel innerStackPanel = new StackPanel
                {
                    Orientation = System.Windows.Controls.Orientation.Horizontal,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center
                };
                if (clausesStatus[i] == true)
                {
                    TextBox textBox = new TextBox { Width = 250, TextWrapping = TextWrapping.NoWrap };
                    TextBlock punctuateTextBlock = new TextBlock { Text = "" + punctuates[punctuateNumber++], Foreground = new SolidColorBrush(Colors.Black) };
                    innerStackPanel.Children.Add(textBox);
                    innerStackPanel.Children.Add(punctuateTextBlock);
                }
                else
                {
                    TextBlock textBlock = new TextBlock { Text = clauses[i], FontSize = 24, Foreground = new SolidColorBrush(Colors.Black) };
                    TextBlock punctuateTextBlock = new TextBlock { Text = "" + punctuates[punctuateNumber++], Foreground = new SolidColorBrush(Colors.Black) };
                    innerStackPanel.Children.Add(textBlock);
                    innerStackPanel.Children.Add(punctuateTextBlock);
                }

                if (punctuateNumber != punctuates.Count && punctuates[punctuateNumber] == '\n') punctuateNumber++;

                stackPanel.Children.Add(innerStackPanel);
            }
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

        private bool[] GetClausesStatus(int count)
        {
            Random rand = new Random(DateTime.Now.Second);

            bool[] clausesStatus = new bool[count];

            for (int i = 0; i < clausesStatus.Length; i++)
                clausesStatus[i] = false;

            int randNumber;
            for (int i = 0; i < 2; i++)
            {
                randNumber = rand.Next(clausesStatus.Length);
                clausesStatus[randNumber] = true;
            }

            return clausesStatus;
        }

        private string GetHandledContent(string content)
        {
            List<string> clauses = GetClauses(content);
            List<char> punctuates = GetPunctuates(content);
            int number = 0;
            string result = "";


            for (int i = 0; i < clauses.Count; i++)
            {
                result += clauses[i];
                result += punctuates[number++];
                if (number != punctuates.Count && punctuates[number] == '\n') number++;
            }

            return result;
        }

        private void OnOkButtonClicked(object sender, EventArgs e)
        {
            string result = "";
            for (int i = 2; i < stackPanel.Children.Count; i++)
            {
                StackPanel innerStackPanel = stackPanel.Children[i] as StackPanel;

                for (int j = 0; j < innerStackPanel.Children.Count; j++)
                {
                    if (innerStackPanel.Children[j] is TextBox)
                    {
                        result += (innerStackPanel.Children[j] as TextBox).Text;
                    }
                    else
                    {
                        result += (innerStackPanel.Children[j] as TextBlock).Text;
                    }
                }

            }

            //若用户输入成功,跳转到成功页面
            string compareResult = GetHandledContent(poem.Content);

            //System.Diagnostics.Debug.WriteLine(result);
            //System.Diagnostics.Debug.WriteLine(compareResult);

            if (compareResult == result)
            {
                PassTest();   
                if (this.NavigationService.CanGoBack)
                    this.NavigationService.GoBack();
            }
            else
            {
                FailedTest();
                if (this.NavigationService.CanGoBack)
                    this.NavigationService.GoBack();
            }
        }

        private void PassTest()
        {
            MessageBox.Show("( ⊙o⊙ )哇好棒啊");
            User user = App._userManager.UserNow;
            if (user.FindPoemStatus(poem.Id) == PoemStatus.NotViewed ||
                user.FindPoemStatus(poem.Id) == PoemStatus.Jumped ||
                user.FindPoemStatus(poem.Id) == PoemStatus.Viewed)
                user.UpdatePoem(poem.Id, PoemStatus.PhaseOne);
        }

        private void FailedTest()
        {
            MessageBox.Show("背得不够熟练哦咕~~(╯﹏╰)");
            User user = App._userManager.UserNow;
            user.UpdatePoem(poem.Id, PoemStatus.Viewed);
        }

        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            if (this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
        }
    }
}