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
using System.IO.IsolatedStorage;

namespace SiChi.Model
{
    /*
     *本类使用流程：
     *当程序第一次启动时，调用initialize
     *每次打开用户界面时，先load（已在app中完成）
     *若在有网环境下注册账号，在网络上注册成功后用register
     *若在有网环境下登录账号，在网络上登录成功并且获取返回信息后用loginLinked
     *若在无网环境下登录账号，用loginnotlinked
     *若想获取本地用户名字，用Usernames,密码Passwords
     */
    public class UserManager
    {
        private string _path = "UserDirectory.dat";
        private List<string> _usernames;
        private List<string> _passwords;
        private User _user;//当前用户

        /*
         *初始化用户目录文件
         *每行对应为：用户名字$密码
         */
        public void Initialize()
        {
            FileManager.CreateFile(_path);
        }

        /*
         *载入用户目录文件
         */
        public void Load()
        {
            List<string> input = FileManager.LoadFile(_path);
            _usernames = new List<string>();
            _passwords = new List<string>();

            foreach (string s in input)
            { 
                _usernames.Add(s.Split('$')[0]);
                _passwords.Add(s.Split('$')[1]);
            }
        }

        /*
         *保存用户目录文件
         */
        public void Save()
        {
            List<string> result = new List<string>();
            for (int i = 0; i < _usernames.Count; i++ )
            {
                result.Add(_usernames[i] + "$" + _passwords[i]);
            }

            FileManager.SaveFile(_path, result);
        }

        /*
         *在本地上注册账号，若注册失败返回false
         *记得在网络上注册成功后再调用
         */
        public bool Register(string name,string password)
        {
            if (Exist(name)) return false;

            _usernames.Add(name);
            _passwords.Add(password);
            User user = new User(name);
            user.Initialize();
            user.Load();
            Save();

            _user = user;

            SaveLastUser(name, password);
            return true;
        }

        /*
         *判断某个用户是否已存在
         */
        private bool Exist(string name)
        {
            for (int i = 0; i < _usernames.Count; i++)
            {
                if (_usernames[i] == name)
                    return true;
            }
            return false;
        }

        /*
         *在本地上进行连线登录
         *第3个参数开始为从网络获取的：
         *修改时间、今天背诵时间、总背诵时间、通过诗歌数目、测试次数
         *记得在网络上登录成功后才调用本函数
         */
        public void LoginLinked(string name,string password,DateTime editTime,int todaySpan,int totalSpan,int totalPass,int totalRecite,string status)
        {
            //在本地上有这用户
            if (Exist(name))
            {
                User user = new User(name);
                user.Load();
                //if (user.EditTime < editTime)
                {
                    user.Set(editTime, todaySpan, totalSpan, totalPass, totalRecite, status);
                    user.Save();
                }

                _user = user;
            }
            //在本地上没这用户
            else
            {
                User user = new User(name);
                user.Initialize();
                user.Load();
                user.Set(editTime, todaySpan, totalSpan, totalPass, totalRecite, status);
                user.Save();

                _usernames.Add(name);
                _passwords.Add(password);
                Save();

                _user = user;
            }

            SaveLastUser(name, password);
        }

        
        /*
         *在本地上对该用户进行脱机登录
         *若返回false则登录失败
         */
        public bool LoginNotLinked(string name, string password)
        {
            //本地上有这用户，可以离线登录
            if (Exist(name))
            {
                User user = new User(name);
                user.Load();

                _user = user;

                SaveLastUser(name, password);
                return true;
            }
            //没有，拒绝
            else return false;
        }

        private void SaveLastUser(string name, string password)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("userLast"))
            {
                settings["userLast"] = name;
                settings["passwordLast"] = password;
            }
            else
            {
                settings.Add("userLast", name);
                settings.Add("passwordLast", password);
            }
        }

        /*
         *用户名列表
         */
        public List<string> Usernames
        {
            get
            {
                return _usernames;
            }
            private set
            {
                _usernames = value;
            }
        }

        /*
         *密码列表
         */
        public List<string> Passwords
        {
            get
            {
                return _passwords;
            }
            private set
            {
                _passwords = value;
            }
        }

        public User UserNow
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
            }
        }

        public DateTime GetEditTime()
        {
            return _user.EditTime;
        }

        public int GetTodaySpan()
        {
            return _user.TodaySpan;
        }

        public int GetTotalSpan()
        {
            return _user.TotalSpan;
        }

        public int GetTotalPass()
        {
            return _user.TotalPass;
        }

        public int GetTotalRecite()
        {
            return _user.TotalRecite;
        }

        /*
         *发上网需要的用户status
         */
        public string GetStatus()
        {
            List<string> s = new List<string>();

            Dictionary<string, PoemStatus> d = _user.PoemsStatus;
            foreach (KeyValuePair<string, PoemStatus> p in d)
            {
                s.Add(p.Key + "$" + (int)p.Value);
            }

            string result = "";
            for (int i = 0; i < s.Count; i++)
            {
                result += s[i];
                if (i != s.Count - 1) result += '^';
            }

            return result;
        }
    }
}
