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
using System.Windows.Threading;
using System.IO.IsolatedStorage;

namespace SiChi.View
{
    public partial class TotalTestPage : PhoneApplicationPage
    {
        private User _user = App._userManager.UserNow;
        private Poem _nowPoem;
        private Random _rand = new Random(DateTime.Now.Millisecond);

        ImageBrush _buttonBackground = new ImageBrush() { ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Image/Button.png", UriKind.Relative)) };

        bool reviewMode = false;

        public TotalTestPage()
        {
            InitializeComponent();

            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("reviewMode"))
                reviewMode = (bool)settings["reviewMode"];
        }

        //进入本界面
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            SliderBarInitialize();
            NextTest();
            base.OnNavigatedTo(e);
        }


        //离开本界面
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            SliderBarEnd();
            base.OnNavigatedFrom(e);
        }

        private void NextTest()
        {
            wrongAnswer = false;
            selectStackPanel.IsHitTestVisible = true;
            selectStackPanel.Children.Clear();

            Poem nextPoem = NextPoem();
            if (nextPoem == null)
            {
                LayoutRoot.Visibility = System.Windows.Visibility.Collapsed;
                MessageBox.Show("( ⊙o⊙ )哇，这个测试包全通过了，好厉害啊");
                if (this.NavigationService.CanGoBack) this.NavigationService.GoBack();
            }
            else
            {
                _nowPoem = nextPoem;
                PoemStatus status = _user.FindPoemStatus(_nowPoem.Id);
                
                if (status == PoemStatus.Jumped || status == PoemStatus.NotViewed || status == PoemStatus.Viewed || status == PoemStatus.PhaseOne)
                {
                    SliderBarNewRun();
                    MutiSelectTest(_nowPoem);
                }
                else if (status == PoemStatus.PhaseTwo)
                {
                    SliderBarStop();
                    FillTest(_nowPoem);
                }
                else if (status == PoemStatus.PhaseThree)
                {
                    SliderBarNewRun();
                    CreatePoemTest(_nowPoem);
                }
                else if (status == PoemStatus.PhaseFour)
                { 
                    SliderBarStop();
                    CreateReciteTest(_nowPoem);
                }
            }
        }

        //时间提醒进度条
        #region sliderBar
        private DispatcherTimer tmr;
        private const int tmrSleepTime = 3;
        private int tmrSleepCount = 0;
        private bool tmrIsRunning;
        private bool wrongAnswer;

        private void SliderBarInitialize()
        {
            wrongAnswer = false;
            sliderBar.Value = 0;
            tmrSleepCount = 0;
            tmrIsRunning = true;
            tmr = new DispatcherTimer();
            tmr.Interval = TimeSpan.FromSeconds(1);
            tmr.Tick += new EventHandler(SliderBarRun);
            tmr.Start();
        }

        private void SliderBarRun(object sender, EventArgs e)
        {
            if (sliderBar.Value < 10 && tmrIsRunning)
            {
                sliderBar.Value++;
                if (sliderBar.Value == 10)
                {
                    warnTextBlock.Text = "时间到。答案是： " + answerString;
                    warnTextBlock.Visibility = System.Windows.Visibility.Visible;
                    wrongAnswer = true;
                }
            }
            else
            {

                tmrSleepCount++;

                if (tmrSleepCount == tmrSleepTime)
                {
                    SliderBarPause();

                    if (wrongAnswer && reviewMode)
                    {
                        int index = App._packageManager.Packages[App._packageIndex].Poems.IndexOf(_nowPoem);
                        App._poemIndex = index;
                        this.NavigationService.Navigate(new Uri("/View/PoemPage.xaml", UriKind.Relative));
                    }
                    else
                    {
                        NextTest();
                    }
                }

            }
        }

        private void SliderBarEnd()
        {
            tmr.Tick -= SliderBarRun;
            tmr.Stop();
        }

        private void SliderBarStop()
        {
            sliderBar.Value = 0;
            warnTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            tmrSleepCount = 0;
            tmr.Stop();
        }

        private void SliderBarResume()
        {
            tmr.Start();
        }

        private void SliderBarPause()
        {
            tmr.Stop();
        }

        private void SliderBarNewRun()
        {
            tmrIsRunning = true;
            tmrSleepCount = 0;
            sliderBar.Value = 0;
            warnTextBlock.Visibility = System.Windows.Visibility.Collapsed;
            tmr.Start();
        }


        #endregion

        //选择题
        #region MutiSelectTest
        private string answerString;

        private void MutiSelectTest(Poem poem)
        {
            string content = MutiSelectTestFill(poem.Content);
            List<string> selectionStrings = TestData.GetFourData(answerString.Length);
            selectionStrings[_rand.Next(4)] = answerString;

            titleTextBlock.Text = poem.Title;
            authorTextBlock.Text = poem.Author;
            contentTextBlock.Text = content;

            TextBlock descriptionTextBlock = new TextBlock { Text = "请选择合适的句子填到下划线处：", Foreground = new SolidColorBrush(Colors.Black) };
            selectStackPanel.Children.Add(descriptionTextBlock);

            for (int i = 0; i < 2; i++)
            {
                StackPanel stackPanel = new StackPanel() { Orientation = System.Windows.Controls.Orientation.Horizontal, HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
                for (int j = 0; j < 2; j++)
                {
                    Button button = new Button() { Content = selectionStrings[i * 2 + j], HorizontalAlignment = System.Windows.HorizontalAlignment.Center, Background = _buttonBackground, 
                                                   Foreground = new SolidColorBrush(Colors.White), BorderThickness = new Thickness(0,0,0,0) };
                    button.Click += new RoutedEventHandler(MutiSelectOnButtonClicked);
                    stackPanel.Children.Add(button);
                }
                selectStackPanel.Children.Add(stackPanel);
            }
        }

        private string MutiSelectTestFill(string input)
        {
            input = GetHandledContent(input);
            string[] paragraphs = input.Split('\n');
            List<string> paragraphsList = new List<string>();
            for (int i = 0; i < paragraphs.Length; i++)
                if (paragraphs[i] != "") paragraphsList.Add(paragraphs[i]);

            int paragraphsListNumber = _rand.Next(paragraphsList.Count);
            string content = "";

            for (int i = 0; i < paragraphsList.Count; i++)
            {
                if (i != paragraphsListNumber)
                {
                    content += paragraphsList[i];
                    if (i != paragraphsList.Count - 1) content += '\n';
                }
                else
                {
                    List<char> punctuates = new List<char>();
                    int punctuatesCounter = 0;
                    char tempChar;
                    for (int j = 0; j < paragraphsList[i].Length; j++)
                    {
                        tempChar = paragraphsList[i][j];
                        if (tempChar == '.' || tempChar == ',' || tempChar == '!' || tempChar == '?' || tempChar == ';' ||
                            tempChar == '。' || tempChar == '，' || tempChar == '！' || tempChar == '？' || tempChar == '；' || tempChar == ';')
                            punctuates.Add(tempChar);
                    }

                    string[] clauses = paragraphsList[i].Split(new char[] { ',', '.', '?', '!', ';', '，', '。', '？', '！', '；', ';' });
                    List<string> clausesList = new List<string>();
                    for (int j = 0; j < clauses.Length; j++)
                        if (clauses[j] != "") clausesList.Add(clauses[j]);

                    int clauseListNumber = _rand.Next(clausesList.Count);
                    while (clausesList[clauseListNumber].Length < 4 || clausesList[clauseListNumber].Length > 7)
                        clauseListNumber = _rand.Next(clausesList.Count);

                    for (int j = 0; j < clausesList.Count; j++)
                    {
                        if (j != clauseListNumber)
                        {
                            content += clausesList[j];
                        }
                        else
                        {
                            answerString = clausesList[j];
                            for (int k = 0; k < clausesList[j].Length * 2; k++)
                                content += '_';
                        }
                        content += punctuates[punctuatesCounter++];
                    }
                    content += '\n';
                }
            }


            return content;
        }

        


        private void MutiSelectHandleRepeat(List<string> s)
        {
            s.Sort();
            bool flag = true;

            while (flag)
            {
                flag = false;
                for (int i = 0; i < s.Count - 1; i++)
                    if (s[i] == s[i + 1] || s[i] == answerString)
                    {
                        flag = true;
                        s.RemoveAt(i + 1);
                        break;
                    }
            }
        }

        private void MutiSelectOnButtonClicked(object sender, EventArgs e)
        {
            selectStackPanel.IsHitTestVisible = false;

            Button button = sender as Button;
            if (button.Content.ToString() == answerString)
            {
                SliderBarPause();
                UpdateUserStatus(true);
                NextTest();
            }
            else
            {
                tmrIsRunning = false;
                warnTextBlock.Text = "错误，答案是： " + answerString;
                UpdateUserStatus(false);
                warnTextBlock.Visibility = System.Windows.Visibility.Visible;
                wrongAnswer = true;
            }
        }
        #endregion

        #region FillTest
        //填空
        private void FillTest(Poem poem)
        {
            string content = MutiSelectTestFill(poem.Content);
            titleTextBlock.Text = poem.Title;
            authorTextBlock.Text = poem.Author;
            contentTextBlock.Text = content;

            TextBlock descriptionTextBlock = new TextBlock { Text = "请在下面填入正确的句子", Foreground = new SolidColorBrush(Colors.Black) };

            StackPanel stackPanel = new StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal };
            TextBox textBox = new TextBox { Width = 250 };
            Button button = new Button { Content = "OK", Foreground = new SolidColorBrush(Colors.White), Background = _buttonBackground, BorderThickness = new Thickness(0, 0, 0, 0) };
            button.Click += new RoutedEventHandler(FillTestOnButtonClicked);
            stackPanel.Children.Add(textBox);
            stackPanel.Children.Add(button);

            selectStackPanel.Children.Add(descriptionTextBlock);
            selectStackPanel.Children.Add(stackPanel);
        }

        private void FillTestOnButtonClicked(object sender, EventArgs e)
        {
            TextBox textBox = ((sender as Button).Parent as StackPanel).Children[0] as TextBox;
            textBox.IsEnabled = false;
            (sender as Button).IsEnabled = false;

            if (textBox.Text == answerString)
            {
                UpdateUserStatus(true);
                warnTextBlock.Text = "正确";
                warnTextBlock.Visibility = System.Windows.Visibility.Visible;
                tmrIsRunning = false;
                SliderBarResume();
            }
            else
            {
                tmrIsRunning = false;
                warnTextBlock.Text = "错误，答案是： " + HandleAnswerString(textBox.Text, answerString);
                warnTextBlock.Visibility = System.Windows.Visibility.Visible;
                wrongAnswer = true;
                UpdateUserStatus(false);
                SliderBarResume();
            }
        }
        #endregion

        //连词成诗
        #region CreatePoemTest

        private void CreatePoemTest(Poem poem)
        {
            string content = CreatePoemTestFill(poem.Content);
            titleTextBlock.Text = poem.Title;
            authorTextBlock.Text = poem.Author;
            contentTextBlock.Text = content;

            TextBlock descriptionTextBlock = new TextBlock { Text = "请按顺序选择合适的句子填入文中数字处:", Foreground = new SolidColorBrush(Colors.Black) };
            selectStackPanel.Children.Add(descriptionTextBlock);
            List<string> result = CreatePoemTestRandAnswer();

            for (int i = 0; i < 2; i++)
            {
                StackPanel stackPanel = new StackPanel { HorizontalAlignment = System.Windows.HorizontalAlignment.Center, Orientation = System.Windows.Controls.Orientation.Horizontal };

                for (int j = 0; j < 2; j++)
                {
                    Button button = new Button { Content = result[i * 2 + j], HorizontalAlignment = System.Windows.HorizontalAlignment.Center, FontSize = 20, Width = 220, Foreground = new SolidColorBrush(Colors.White), Background = _buttonBackground, BorderThickness = new Thickness(0, 0, 0, 0) };
                    button.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(CreatePoemTestButtonClicked);
                    stackPanel.Children.Add(button);
                }

                selectStackPanel.Children.Add(stackPanel);
            }
        }

        private void CreatePoemTestButtonClicked(object sender, EventArgs e)
        {
            Button textButton = sender as Button;
            string textButtonContent = textButton.Content.ToString();

            //清除所有选择
            if (textButtonContent[0] <= '4' && textButtonContent[0] >= '1')
            {
                for (int i = 0; i < 2; i++)
                {
                    StackPanel stackPanel = selectStackPanel.Children[1 + i] as StackPanel;
                    for (int j = 0; j < 2; j++)
                    {
                        Button button = stackPanel.Children[j] as Button;
                        string buttonContent = button.Content.ToString();

                        if (buttonContent[0] <= '4' && buttonContent[0] >= '1')
                        {
                            button.Content = buttonContent.Substring(2);
                        }
                    }
                }
            }
            else
            {
                //赋值
                int max = 0;
                for (int i = 0; i < 2; i++)
                {
                    StackPanel stackPanel = selectStackPanel.Children[1 + i] as StackPanel;
                    for (int j = 0; j < 2; j++)
                    {
                        Button button = stackPanel.Children[j] as Button;
                        string buttonContent = button.Content.ToString();

                        if (buttonContent[0] <= '4' && buttonContent[0] >= '1')
                        {
                            if (buttonContent[0] - '0' > max)
                            {
                                max = buttonContent[0] - '0';
                            }
                        }
                    }
                }
                max++;

                textButton.Content = max.ToString() + '.' + textButton.Content;

                //检查答案
                if (max == 4)
                {
                    string result = "";

                    for (int l = 0; l < 4; l++)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            StackPanel stackPanel = selectStackPanel.Children[1 + i] as StackPanel;
                            for (int j = 0; j < 2; j++)
                            {
                                Button button = stackPanel.Children[j] as Button;

                                button.IsEnabled = false;
                                string buttonContent = button.Content.ToString();

                                if (buttonContent[0] - '0' == l + 1)
                                {
                                    result += buttonContent + ' ';
                                }
                            }
                        }
                    }

                    //答案正确
                    if (result == answerString)
                    {
                        tmrIsRunning = false;
                        UpdateUserStatus(true);
                        warnTextBlock.Text = "正确";
                        warnTextBlock.Visibility = System.Windows.Visibility.Visible;
                    }
                    //答案错误
                    else
                    {
                        wrongAnswer = true;
                        tmrIsRunning = false;
                        UpdateUserStatus(false);
                        warnTextBlock.Text = "错误，答案是： " + answerString;
                        warnTextBlock.Visibility = System.Windows.Visibility.Visible;
                    }
                }
            }

        }


        private List<string> CreatePoemTestRandAnswer()
        {
            string[] temp = answerString.Split(new char[] { ' ', '.' });
            List<string> result = new List<string>();

            for (int i = 0; i < temp.Length; i++)
                if (temp[i].Length >= 4) result.Add(temp[i]);

            string swap = "";
            int randNumber;
            for (int i = 0; i < 4; i++)
            {
                swap = result[i];
                randNumber = _rand.Next(4);
                result[i] = result[randNumber];
                result[randNumber] = swap;
            }

            return result;
        }

        private string CreatePoemTestFill(string input)
        {
            input = GetHandledContent(input);

            List<char> punctuates = new List<char>();
            int punctuatesNumber = 0;
            char temp;
            string content = "";

            for (int i = 0; i < input.Length; i++)
            {
                temp = input[i];
                if (temp == '\n' || temp == ',' || temp == '.' || temp == '?' || temp == '!' || temp == ';'
                    || temp == '。' || temp == '，' || temp == '？' || temp == '！' || temp == '；' || temp == ';')
                    punctuates.Add(temp);
            }

            string[] clausesTemp = input.Split(new char[] { '\n', ',', '.', '?', '!', ';', '。', '，', '？', '！', '；', ';' });
            List<string> clauses = new List<string>();


            for (int i = 0; i < clausesTemp.Length; i++)
                if (clausesTemp[i] != "") clauses.Add(clausesTemp[i]);

            bool[] clausesIsChoose = new bool[clauses.Count];
            int count = 4, randNumber = 0;

            for (int i = 0; i < clausesIsChoose.Length; i++)
                clausesIsChoose[i] = false;

            while (count != 0)
            {
                randNumber = _rand.Next(clauses.Count);
                if (clausesIsChoose[randNumber] == false && clauses[randNumber].Length >= 4)
                {
                    clausesIsChoose[randNumber] = true;
                    count--;
                }
            }

            answerString = "";
            int answerStringNumber = 1;

            for (int i = 0; i < clauses.Count; i++)
            {
                if (clausesIsChoose[i])
                {
                    answerString += answerStringNumber.ToString() + '.' + clauses[i] + ' ';

                    content += answerStringNumber.ToString() + '.';

                    for (int j = 0; j < clauses[i].Length * 2; j++)
                    {
                        content += '_';
                    }
                    content += punctuates[punctuatesNumber++];
                    if (punctuatesNumber != punctuates.Count && punctuates[punctuatesNumber] == '\n')
                    {
                        content += punctuates[punctuatesNumber++];
                    }

                    answerStringNumber++;
                }
                else
                {
                    content += clauses[i];
                    content += punctuates[punctuatesNumber++];
                    if (punctuatesNumber != punctuates.Count && punctuates[punctuatesNumber] == '\n')
                    {
                        content += punctuates[punctuatesNumber++];
                    }
                }
            }

            return content;
        }

        #endregion

        #region ReciteTest
        public void CreateReciteTest(Poem poem)
        {
            CreateReciteTestAnswer(poem.Content);
            string content = GetHandledContent(poem.Content);

            authorTextBlock.Text = poem.Author;
            titleTextBlock.Text = poem.Title;
            contentTextBlock.Text = CreateReciteTestGetViewString(poem.Content);

            CreateReciteTestView(content);
        }

        public void CreateReciteTestView(string content)
        {
            List<string> clauses = GetClauses(content);

            TextBlock descriptionTextBlock = new TextBlock { Text = "请按顺序填入文中空白处:", Foreground = new SolidColorBrush(Colors.Black) };
            selectStackPanel.Children.Add(descriptionTextBlock);

            for (int i = 0; i < clauses.Count; i++)
            {
                StackPanel stackPanel = new StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal };
                TextBlock textBlock = new TextBlock { Text = (i + 1).ToString() };
                TextBox textBox = new TextBox { Width = 240 };
                stackPanel.Children.Add(textBlock);
                stackPanel.Children.Add(textBox);

                if (i == clauses.Count - 1)
                {
                    Button button = new Button { Content = "OK", HorizontalAlignment = System.Windows.HorizontalAlignment.Center, Foreground = new SolidColorBrush(Colors.White), Background = _buttonBackground, BorderThickness = new Thickness(0, 0, 0, 0) };
                    button.Click += new RoutedEventHandler(CreateReciteTestButtonClicked);
                    stackPanel.Children.Add(button);
                }

                selectStackPanel.Children.Add(stackPanel);

            }

        }

        public string CreateReciteTestGetResult()
        {
            List<string> temp = new List<string>();
            List<char> punctuates = GetPunctuates(answerString);

            for (int i = 1; i < selectStackPanel.Children.Count; i++)
            {
                StackPanel stackPanel = selectStackPanel.Children[i] as StackPanel;
                TextBox textBox = stackPanel.Children[1] as TextBox;
                temp.Add(textBox.Text);
            }

            string result = "";
            int number = 0;
            for (int i = 0; i < temp.Count; i++)
            {
                result += temp[i];
                result += punctuates[number++];
                if (number != punctuates.Count && punctuates[number] == '\n') number++;
            }

            return result;
        }

        public void CreateReciteTestButtonClicked(object sender, EventArgs e)
        {
            string result = CreateReciteTestGetResult();



            //答案正确
            if (result == answerString)
            {
                tmrIsRunning = false;
                UpdateUserStatus(true);
                warnTextBlock.Text = "正确";
                warnTextBlock.Visibility = System.Windows.Visibility.Visible;
                SliderBarResume();
            }
            //答案错误
            else
            {
                wrongAnswer = true;
                tmrIsRunning = false;
                UpdateUserStatus(false);
                warnTextBlock.Text = "错误，答案是： " + HandleAnswerString(result, answerString);
                warnTextBlock.Visibility = System.Windows.Visibility.Visible;
                SliderBarResume();
            }
        }

        public string CreateReciteTestGetViewString(string content)
        {
            List<string> clauses = GetClauses(content);
            List<char> punctuate = GetPunctuates(content);
            int number = 0;

            string result = "";
            for (int i = 0; i < clauses.Count; i++)
            {
                result += (i + 1).ToString() + ".";
                for (int j = 0; j < clauses[i].Length * 2; j++)
                    result += '_';

                result += punctuate[number++];
                if (number != punctuate.Count && punctuate[number] == '\n') number++; 
            }

            return result;
        }

        public void CreateReciteTestAnswer(string content)
        {
            answerString = GetHandledContent(content);
        }

        #endregion

        private string HandleAnswerString(string input, string answer)
        {
            int count = answer.Length ;
            List<int> temp = new List<int>();

            for (int i = 0; i < input.Length && i < answer.Length; i++)
            {
                if (input[i] != answer[i]) { count--; temp.Add(i); }
            }

            if (count >= 4) return answer;
            string result = "";
            for (int i = 0; i < temp.Count; i++)
            {
                result += input[temp[i]] + "(错误) " + answer[temp[i]] + "(正确)；";
            }
            return result;
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

        private List<string> GetClauses(string content)
        {
            System.Diagnostics.Debug.WriteLine(content);

            string[] clausesTemp = content.Split(new Char[] { '。', '，', '！', '？', ',', '.', '?', '!', '\n', ';', '；','"', '“', '”' });
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
                    || tempChar == ';' || tempChar == '“' || tempChar == '“' || tempChar == '"')
                    punctuates.Add(tempChar);
            }
            return punctuates;
        }

        private void UpdateUserStatus(bool _isAdd)
        {
            string id = _nowPoem.Id;
            PoemStatus p = _user.FindPoemStatus(id);

            if (_isAdd)
            {
                switch (p)
                {
                    case PoemStatus.Jumped:
                    case PoemStatus.NotViewed:
                    case PoemStatus.Viewed:
                    case PoemStatus.PhaseOne:
                        p = PoemStatus.PhaseTwo;
                        break;
                    case PoemStatus.PhaseTwo:
                        p = PoemStatus.PhaseThree;
                        break;
                    case PoemStatus.PhaseThree:
                        p = PoemStatus.PhaseFour;
                        break;
                    case PoemStatus.PhaseFour:
                        p = PoemStatus.Pass;
                        break;
                }
                _user.TotalPass++;
                _user.TotalRecite++;
            }
            else if (!_isAdd)
            {
                switch (p)
                { 
                    case PoemStatus.Pass:
                        p = PoemStatus.PhaseFour;
                        break;
                    case PoemStatus.PhaseFour:
                        p = PoemStatus.PhaseThree;
                        break;
                    case PoemStatus.PhaseThree:
                        p = PoemStatus.PhaseTwo;
                        break;
                    case PoemStatus.PhaseTwo:
                        p = PoemStatus.PhaseOne;
                        break;
                    case PoemStatus.Jumped:
                    case PoemStatus.NotViewed:
                    case PoemStatus.Viewed:
                        p = PoemStatus.PhaseOne;
                        break;
                }
                _user.TotalRecite++;
            }

            _user.UpdatePoem(id, p);
        }


        //找下一首诗在poems中的下标号
        //若没有能复习的诗，则返回null
        //表示全通过
        private Poem NextPoem()
        {

            PackageManager _packageManager = App._packageManager;
            List<Poem> tempArray = new List<Poem>();
            Package package = _packageManager.Packages[App._packageIndex];

            foreach (Poem p in package.Poems)
            {
                PoemStatus s = _user.FindPoemStatus(p.Id);
                if (s != PoemStatus.Pass)
                    tempArray.Add(p);
            }

            if (tempArray.Count == 0) return null;
            else return tempArray[_rand.Next(tempArray.Count)];
        }
    }
}