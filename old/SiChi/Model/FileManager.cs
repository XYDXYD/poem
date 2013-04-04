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
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.IO;

namespace SiChi.Model
{
    public class FileManager
    {
        private static IsolatedStorageFile _isStore = IsolatedStorageFile.GetUserStoreForApplication();

        /*
         * 创建文件，如果该文件已存在，则覆盖
         */
        public static void CreateFile(string path)
        {
            //System.Diagnostics.Debug.WriteLine("Create:" + path);
            IsolatedStorageFileStream stream = _isStore.CreateFile(path);
            stream.Close();
        }

        /*
         *删除文件
         */
        public static void DeleteFile(string path)
        {
            if (_isStore.FileExists(path))
                _isStore.DeleteFile(path);
        }


        /*
         * 载入文件内容，返回List<string>
         * 假定文件已存在，否则会触发异常
         */ 
        public static List<string> LoadFile(string path)
        {
            //System.Diagnostics.Debug.WriteLine("Load:" + path);


            if (!_isStore.FileExists(path))
                 throw new FileNotFoundException();

            IsolatedStorageFileStream stream = _isStore.OpenFile(path, System.IO.FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            List<string> result = new List<string>();

            while (!reader.EndOfStream)
                result.Add(reader.ReadLine());

            reader.Close();
            

            //foreach (string s in result)
              //  System.Diagnostics.Debug.WriteLine(s);
            //System.Diagnostics.Debug.WriteLine("Load Complete\n");

            return result;
        }

        /*
         *按顺序把content内容写到path中
         *假定文件已存在，否则触发异常
         */
        public static void SaveFile(string path, List<string> content)
        {
            //System.Diagnostics.Debug.WriteLine("Save:" + path);

            if (!_isStore.FileExists(path))
                throw new FileNotFoundException();

            IsolatedStorageFileStream stream = _isStore.OpenFile(path, FileMode.Open, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);

            for (int i = 0; i < content.Count; i++)
                writer.WriteLine(content[i]);

            writer.Close();

            //foreach (string s in content)
               // System.Diagnostics.Debug.WriteLine(s);
           // System.Diagnostics.Debug.WriteLine("Save Complete\n");
        }

        public static void SaveFile(string path, string[] content)
        {
           // System.Diagnostics.Debug.WriteLine("Save:" + path);

            if (!_isStore.FileExists(path))
                throw new FileNotFoundException();

            IsolatedStorageFileStream stream = _isStore.OpenFile(path, FileMode.Open, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);

            for (int i = 0; i < content.Length; i ++)
                writer.WriteLine(content[i]);

            writer.Close();

            //foreach (string s in content)
             //   System.Diagnostics.Debug.WriteLine(s);
            //System.Diagnostics.Debug.WriteLine("Save Complete\n");
        }
    }
}
