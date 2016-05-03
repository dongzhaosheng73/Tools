using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DTTOOLS.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class TextReadWrite
    {
        /// <summary>
        /// 写入text
        /// </summary>
        /// <param name="path"></param>
        /// <param name="meg"></param>
        /// <returns></returns>
        public static bool WriteText(string path,string meg)
        {
            try
            {
                var log = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                log.Position = log.Length;
                var sw = new StreamWriter(log,Encoding.UTF8);
                sw.WriteLine(meg);
                sw.Close();
                sw.Dispose();
                log.Close();
                log.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        /// <summary>
        /// 读取text
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadText(string path)
        {
            try
            {
                var log = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                log.Position =0;
                var sw = new StreamReader(log,Encoding.UTF8);
                char[] readchar = new char[log.Length];
                sw.Read(readchar, 0, readchar.Length);
                sw.Close();
                sw.Dispose();
                log.Close();             
                log.Dispose();
                return new string(readchar);
            }
            catch (Exception)
            {
                return "";
            }
           
        }
    }
}
