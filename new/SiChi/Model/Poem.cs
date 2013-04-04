using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SiChi.Model
{
    /*用户的诗歌状态，Jumped为已跳过
        *NotViewed为未查看，为诗歌初始状态
        *Viewed为已查看但未背诵
        *PhaseOne、PhaseTwo、PhaseThree、PhaseFour为已背诵
        *分别对应选择题、填空题、连词成诗、全文默写阶段
        *Pass为已熟练掌握，不再出现
        */
    public enum PoemStatus : int
    {
        Jumped = -2,
        NotViewed = -1,
        Viewed = 0,
        PhaseOne = 1,
        PhaseTwo = 2,
        PhaseThree = 3,
        PhaseFour = 4,
        Pass = 5
    }

    public class Poem
    {
        private string _id;
        private string _title;
        private string _author;
        private string _content;

        public Poem(string id,string title, string author, string content)
        {
            _id = id;
            _title = title;
            _author = author;
            _content = content;
        }

        //从网上获取作者信息
        public string GetAuthorInformation()
        { 
            return null;
        }

        public string GetBackgroundInformation()
        {
            return null;
        }

        public string GetDescriptionInformation()
        {
            return null;
        }


        public string Id
        {
            set
            {
                _id = value;
            }
            get
            {
                return _id;
            }
        }

        public string Title
        {
            set
            {
                _title = value;
            }

            get
            {
                return _title;
            }
        }


        public string Author
        {
            set
            {
                _author = value;
            }
            get
            {
                return _author;
            }
        }


        public string Content
        {
            set
            {
                _content = value;

            }
            get
            {
                return _content;
            }
        }


    }
}
