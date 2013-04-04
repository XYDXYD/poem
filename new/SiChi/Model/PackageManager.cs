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
    public class PackageManager
    {
        private List<string> _packageNames;
        private List<Package> _packages;
        private string _path = "packagesDirectory.dat";

        /*
         *在程序第一次启动时初始化PackageManager
         */
        public void Initialize()
        {
            FileManager.CreateFile(_path);
        }

        /*
         *载入PackageManager列表
         *每行数据格式为：中文名字-英文名字
         */
        public void Load()
        {
            _packageNames = FileManager.LoadFile(_path);
            _packages = new List<Package>();

            foreach (string s in _packageNames)
            {
                Package p = new Package(s);
                p.Load();
                _packages.Add(p);
            }
        }

        /*
         *用于查找特定的诗歌,需在load后使用
         *假定该诗必须存在，若不存在，返回null
         */
        public Poem FindPoem(string id)
        {
            foreach (Package p in _packages)
            {
                Poem poem = p.FindPoem(id);
                if (poem != null) return poem;
            }

            return null;
        }


        public void FindThroughId(string id,ref string title,ref string author,ref int packageIndex,ref int poemIndex)
        {
            for (int i = 0; i < _packages.Count; i++)
            {
                for (int j = 0; j < _packages[i].Poems.Count; j++)
                { 
                    Poem p = _packages[i].Poems[j];
                    if (p.Id == id)
                    {
                        title = p.Title;
                        author = p.Author;
                        packageIndex = i;
                        poemIndex = j;
                        return;
                    }
                }
            }

            packageIndex = poemIndex = -1;
        }

        /*
         *用于在下载包后更新，已保存数据
         */
        public void Add(Package p)
        {
            _packageNames.Add(p.PackageName);
            _packages.Add(p);

            FileManager.SaveFile(_path, _packageNames);
            FileManager.CreateFile("Package" + p.PackageName + ".dat");
            p.Save();
        }
        

        public List<string> PackageNames
        {
            get
            {
                return _packageNames;
            }
            private set
            {
                _packageNames = value;
            }
        }

        public List<Package> Packages
        {
            get
            {
                return _packages;
            }
            private set
            {
                _packages = value;
            }
        }
    }
}
