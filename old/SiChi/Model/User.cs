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
using System.Collections.Generic;

namespace SiChi.Model
{
    public class User
    {
        private string _name;
        private DateTime _editTime;
        private int _todaySpan, _totalSpan;
        private int _totalPass, _totalRecite;
        private Dictionary<string, PoemStatus> _poemsStatus;//id + 状态

        public User(string name)
        {
            this._name = name;
        }

        /*
         *注册账号或本地初始化时用
         *初始化用户配置文件
         *用户配置文件信息：
         *第一行：修改时间
         *第二行：今天使用时间、总的使用时间
         *第三行：总的通过次数、总的测试次数
         *第四行：诗歌ID'$'对应诗歌状态
         */
        public void Initialize()
        {
            _editTime = DateTime.Now;
            _todaySpan = _totalSpan = 0;
            _totalPass = _totalRecite = 0;

            string _path = "User" + _name + ".dat";

            string[] content = { _editTime.ToString(), 
                                 _todaySpan.ToString() + "$" + _totalSpan.ToString() ,
                                 _totalPass.ToString() + "$" + _totalRecite.ToString() };
            FileManager.CreateFile(_path);
            FileManager.SaveFile(_path, content);
        }

        /*
         *载入用户配置文件
         */
        public void Load()
        {
            string _path = "User" + _name + ".dat";

            List<string> input = FileManager.LoadFile(_path);
            _editTime = Convert.ToDateTime(input[0]);
            _todaySpan = int.Parse(input[1].Split('$')[0]);
            _totalSpan = int.Parse(input[1].Split('$')[1]);
            _totalPass = int.Parse(input[2].Split('$')[0]);
            _totalRecite = int.Parse(input[2].Split('$')[1]);

            _poemsStatus = new Dictionary<string, PoemStatus>();

            for (int i = 3; i < input.Count; i++)
            {
                string[] content = input[i].Split('$');
                if (content[0] == "") continue;
                _poemsStatus.Add(content[0], (PoemStatus)Int32.Parse(content[1]));
            }
        }

        /*
         *保存用户配置文件
         */
        public void Save()
        {
            string _path = "User" + _name + ".dat";

            List<string> result = new List<string>();
            result.Add(DateTime.Now.ToString());
            result.Add(_todaySpan.ToString() + "$" + _totalSpan.ToString());
            result.Add(_totalPass.ToString() + "$" + _totalRecite.ToString());

            foreach (KeyValuePair<string, PoemStatus> p in _poemsStatus)
            {
                result.Add(p.Key + "$" + ((int)p.Value).ToString());
            }

            FileManager.SaveFile(_path, result);
        }

        public void Set(DateTime editTime, int todaySpan, int totalSpan, int totalPass, int totalRecite, string status)
        {
            this.EditTime = editTime;
            this.TodaySpan = todaySpan;
            this.TotalSpan = totalSpan;
            this.TotalPass = totalPass;
            this.TotalRecite = totalRecite;

            Dictionary<string, PoemStatus> dictionary = new Dictionary<string, PoemStatus>();

            if (status != null && status != "")
            {
                string[] content = status.Split('^');

                foreach (string s in content)
                {
                    string id = s.Split('$')[0];
                    PoemStatus p = (PoemStatus)(int.Parse(s.Split('$')[1]));
                    UpdatePoem(id, p);
                }
            }
        }

        /*
         *更新某一首诗歌的状态
         *未保存
         */
        public void UpdatePoem(string id, PoemStatus s)
        {
            _poemsStatus[id] = s;
        }

        /*
         *找出诗歌对应id的状态，若该id不存在，返回未看
         */
        public PoemStatus FindPoemStatus(string id)
        {
            if (_poemsStatus.ContainsKey(id))
                return _poemsStatus[id];
            else return PoemStatus.NotViewed;
        }

        /*
         *随机查找下首需要考察的诗歌
         *返回对应的id，若不存在返回null
         */
        public string FindNext()
        {
            List<string> temp = new List<string>();

            foreach (KeyValuePair<string, PoemStatus> p in _poemsStatus)
            {
                if (p.Value != PoemStatus.Pass && p.Value != PoemStatus.Jumped)
                    temp.Add(p.Key);
            }

            if (temp.Count == 0) return null;
            else 
            {
                Random rand = new Random(DateTime.Now.Second);
                return temp[rand.Next(temp.Count)];
            }
        }

        /*
         *返回三首需要考察的诗歌
         *格式跟findnext一样，若不满足三首，则不满足部分返回null
         */
        public List<string> FindThreeNext()
        {
            List<string> temp = new List<string>();
            foreach (KeyValuePair<string, PoemStatus> p in _poemsStatus)
            {
                if (p.Value != PoemStatus.Pass && p.Value != PoemStatus.Jumped)
                    temp.Add(p.Key);
            }

            if (temp.Count == 0)
            {
                temp.Add(null);
                temp.Add(null);
                temp.Add(null);
                return temp;
            }
            else if (temp.Count == 1)
            {
                temp.Add(null);
                temp.Add(null);
                return temp;
            }
            else if (temp.Count == 2)
            {
                temp.Add(null);
                return temp;
            }
            else if (temp.Count == 3)
            {
                return temp;
            }
            else
            {
                List<string> result = new List<string>();
                Random rand = new Random(DateTime.Now.Second);
                int number = rand.Next(temp.Count - 3);
                for (int i = number; i < number + 3; i++)
                    result.Add(temp[i]);
                return result;
            }
        }


        public Dictionary<string, PoemStatus> PoemsStatus
        {
            private set
            {
                _poemsStatus = value;
            }
            get
            {
                return _poemsStatus;
            }
        }

        public string Name
        {
            set
            {
                _name = value;
            }
            get
            {
                return _name;
            }
        }


        public DateTime EditTime
        {
            set
            {
                _editTime = value;
            }
            get
            {
                return _editTime;
            }
        }

        public int TodaySpan
        {
            set
            {
                _todaySpan = value;
            }
            get
            {
                return _todaySpan;
            }
        }

        public int TotalSpan
        {
            set
            {
                _totalSpan = value;
            }
            get
            { 
                return _totalSpan;
            }
        }

        public int TotalPass
        {
            set
            {
                _totalPass = value;
            }
            get
            {
                return _totalPass;
            }
        }

        public int TotalRecite
        {
            set
            {
                _totalRecite = value;
            }
            get
            {
                return _totalRecite;
            }
        }
    }
}
