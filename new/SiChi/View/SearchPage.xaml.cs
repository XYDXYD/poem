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
using Newtonsoft.Json.Linq;
using System.Text;
using SiChi.Model;

namespace SiChi.View
{
    public partial class SearchPage : PhoneApplicationPage
    {
        public SearchPage()
        {
            InitializeComponent();
        }
        List<int> _packageIndex = new List<int>();
        List<int> _poemIndex = new List<int>();

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            int index = listBox.SelectedIndex;
            if (index == -1) return;

            App._packageIndex = _packageIndex[index];
            App._poemIndex = _poemIndex[index];
            index = -1;
            this.NavigationService.Navigate(new Uri("/View/PoemPage.xaml", UriKind.Relative));
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            PHPRPC_Client client = new PHPRPC_Client("http://1.winphonexiang.sinaapp.com/guwen/search_poem.php");
            string input = textBox.Text;
            client.Invoke("search", new Object[] {input}, Callback);

            showAnimation.Begin();
            wait.Visibility = System.Windows.Visibility.Visible;
            listBox.IsEnabled = false;
            //button.IsEnabled = false;
        }

        public void Callback(Object result, Object[] args, String output, PHPRPC_Error warning)
        {
            if (warning != null)
            {
                MessageBox.Show("网络连接错误~");
                return;
            }
            byte[] temp = result as byte[];
            string json = Encoding.UTF8.GetString(temp, 0, temp.Length);

            JObject input = JObject.Parse(json);
            int count = 0;

            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            foreach (var i in input["result"])
            {
                string[] content = i.ToString().Split('$');
                count++;

                if (content.Length >= 2)
                {
                    string clause = content[0];
                    for (int j = 1; j < content.Length; j++)
                    {
                        content[j] = content[j].Trim();
                        if (content[j] == "") continue;

                        if (!dictionary.ContainsKey(content[j])) dictionary[content[j]] = 0;
                        dictionary[content[j]]++;
                    }
                }

            }

            List<string> resultId = new List<string>();
            List<int> resultScore = new List<int>();

            foreach (KeyValuePair<string, int> s in dictionary)
            {
                resultId.Add(s.Key);
                resultScore.Add(s.Value);

                System.Diagnostics.Debug.WriteLine(s.Key + " " + s.Value);
            }

            Sort(ref resultId, ref resultScore);
            PackageManager manager = App._packageManager;

            int number = 0;
            listBox.Items.Clear();
            _packageIndex.Clear();
            _poemIndex.Clear();

            for (int i = 0; i < resultId.Count; i++)
            {
                if (resultScore[i] >= count && number < 15)
                {
                    number++;
                    string title = "", author = "";
                    int packageNumber = 0, poemNumber = 0;

                    manager.FindThroughId(resultId[i], ref title, ref author, ref packageNumber, ref poemNumber);

                    if (packageNumber != -1 && poemNumber != -1)
                    {
                        _packageIndex.Add(packageNumber);
                        _poemIndex.Add(poemNumber);

                        StackPanel stackPanel = new StackPanel();
                        TextBlock t1 = new TextBlock { Text = title, FontSize = 28, Foreground = new SolidColorBrush(Colors.Black) };
                        TextBlock t2 = new TextBlock { Text = author, FontSize = 22, Foreground = new SolidColorBrush(Colors.Black) };
                        Border b = new Border { BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness = new Thickness(0, 0, 0, 1), Margin = new Thickness(0, 10, 0, 0), Width = 400, HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
                        stackPanel.Children.Add(t1);
                        stackPanel.Children.Add(t2);
                        stackPanel.Children.Add(b);
                        listBox.Items.Add(stackPanel);
                    }
                }
                else break;
            }

            fadeAnimation.Begin();
            fadeAnimation.Completed += new EventHandler(fadeAnimation_Completed);
        }

        private void fadeAnimation_Completed(object sender, EventArgs e)
        {
            wait.Visibility = System.Windows.Visibility.Collapsed;
            listBox.IsEnabled = true;
        }

        public void Sort(ref List<string> id, ref List<int> score)
        {
            int temp;
            string s;

            for (int i = 0; i < id.Count; i++)
            {
                for (int j = 0; j < id.Count - 1; j++)
                {
                    if (score[j] < score[j + 1])
                    {
                        temp = score[j];
                        score[j] = score[j + 1];
                        score[j + 1] = temp;

                        s = id[j];
                        id[j] = id[j + 1];
                        id[j + 1] = s;
                    }
                }
            }
        }

        private void Button_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            ((Image)sender).Margin = new Thickness(((Image)sender).Margin.Left + 3, ((Image)sender).Margin.Top + 3, ((Image)sender).Margin.Right + 3, ((Image)sender).Margin.Bottom + 3);
        }

        private void Button_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            ((Image)sender).Margin = new Thickness(((Image)sender).Margin.Left - 3, ((Image)sender).Margin.Top - 3, ((Image)sender).Margin.Right - 3, ((Image)sender).Margin.Bottom - 3);
        }

        private void Image_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}