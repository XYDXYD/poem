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
    public class Package
    {
        private List<Poem> _poems;//在下载时请注意，这里没有初始化
        private string _packageName;

        public Package(string packageName)
        {
            _packageName = packageName;
        }

        /*
         *本构造函数用于在下载后添加
         */
        public Package(string packageName,List<Poem> poems)
        {
            _packageName = packageName;
            _poems = poems;
        }

        /*
         *从本地数据库载入诗歌包
         *诗歌包每行格式为
         *id$标题$作者$内容，其中内容得换行用$分割
         */
        public void Load()
        {
            string path = "Package" + _packageName + ".dat";
            List<string> result = FileManager.LoadFile(path);
            _poems = new List<Poem>();

            foreach (string s in result)
            {
                Poem p = ConvertStringToPoem(s);
                _poems.Add(p);
            }
        }


        /*
         *保存诗歌包，用于在下载包后保存
         *诗歌包每行格式为：Id$标题$作者$内容，其中内容换行用$分割
         */
        public void Save()
        {
            string path = "Package" + _packageName + ".dat";
            List<string> result = new List<string>();

            for (int i = 0; i < _poems.Count; i++)
            {
                result.Add( ConvertPoemToString(_poems[i]));
                
            }

            FileManager.SaveFile(path,result);
        }

        /*
         *查找在本包中的诗歌
         *若不存在，则返回null
         */
        public Poem FindPoem(string id)
        {
            foreach (Poem p in _poems)
            {
                if (p.Id == id)
                    return p;
            }

            return null;
        }



        static public Poem ConvertStringToPoem(string s)
        {
            string[] temp = s.Split('$');
            string content = "";

            for (int i = 3; i < temp.Length; i++)
            {
                content += temp[i];
                if (i != temp.Length - 1) content += "\n";
            }

            Poem poem = new Poem(temp[0], temp[1], temp[2], content);
            return poem;
        }

        static private string ConvertPoemToString(Poem p)
        {
            string result = p.Id + "$" + p.Title + "$" + p.Author + "$";
            string[] temp = p.Content.Split('\n');
            for (int i = 0; i < temp.Length; i++)
                if (i != temp.Length - 1) result += temp[i] + "$";
                else result += temp[i];
            return result;
        }


        public string PackageName
        {
            set
            {
                _packageName = value;
            }
            get
            {
                return _packageName;
            }
        }

        public List<Poem> Poems
        {
            set
            {
                _poems = value;
            }
            get
            {
                return _poems;
            }
        }
    }
}
