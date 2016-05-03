using System;
using System.IO;

namespace DTTOOLS.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class Errorlog
    {
        private string Path { set; get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public Errorlog(string path)
        {
            Path = path;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        public void WriteError(Exception error)
        {
            FileStream errorlog = File.Open(Path + DateTime.Now.ToString("yyMMdd") + ".log", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            errorlog.Position = errorlog.Length;
            StreamWriter sw = new StreamWriter(errorlog);
            sw.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
            sw.WriteLine(error.Message + error.Source + error.StackTrace);
            sw.WriteLine(DateTime.Now.ToString("------------------------"));
            sw.Close();
            sw.Dispose();
            errorlog.Close();
            errorlog.Dispose();
        }
        
        public void WriteError(string error)
        {
            FileStream errorlog = File.Open(Path + DateTime.Now.ToString("yyMMdd") + ".log", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            errorlog.Position = errorlog.Length;
            StreamWriter sw = new StreamWriter(errorlog);
            sw.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
            sw.WriteLine(error);
            sw.WriteLine(DateTime.Now.ToString("------------------------"));
            sw.Close();
            sw.Dispose();
            errorlog.Close();
            errorlog.Dispose();
        }
        public void WriteError(string error,string fileName)
        {
            FileStream errorlog = File.Open(Path + fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            errorlog.Position = errorlog.Length;
            StreamWriter sw = new StreamWriter(errorlog);
            sw.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
            sw.WriteLine(error);
            sw.WriteLine(DateTime.Now.ToString("------------------------"));
            sw.Close();
            sw.Dispose();
            errorlog.Close();
            errorlog.Dispose();
        }
    }
}
