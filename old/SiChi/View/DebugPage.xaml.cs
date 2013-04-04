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
    public partial class DebugPage : PhoneApplicationPage
    {
        public DebugPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            listBox.Items.Clear();

            User user = App._userManager.UserNow;
            foreach (KeyValuePair<string, PoemStatus> p in user.PoemsStatus)
            {
                listBox.Items.Add(p.Key + " " + p.Value);
            }

            base.OnNavigatedTo(e);
        }
    }
}