using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Windows;
using DTTOOLS.Tools;

namespace DTTOOLS.ExpandClass
{
    public static class StringEx
    {
        /// <summary>
        /// 文件拷贝
        /// </summary>
        /// <param name="sfile">源文件路径</param>
        /// <param name="dfile">目的文件路径</param>
        /// <param name="SectSize">传输大小 1048576</param>
        /// <param name="type">文件扩展名</param>
        /// <returns>是否成功</returns>
        public static bool DTCopyFile(this string sfile, string dfile, int SectSize, string type ,bool delect)
        {
            try
            {
                FileStream fileToCreate = new FileStream(dfile, FileMode.Create);//创建目的文件，如果已存在将被覆盖
                fileToCreate.Close();//关闭所有资源
                fileToCreate.Dispose();//释放所有资源
                FileStream FormerOpen = new FileStream(sfile, FileMode.Open, FileAccess.Read);//以只读方式打开源文件
                FileStream ToFileOpen = new FileStream(dfile, FileMode.Append, FileAccess.Write);//以写方式打开目的文件
                int max = Convert.ToInt32(Math.Ceiling((double)FormerOpen.Length / (double)SectSize));//根据一次传输的大小，计算传输的个数
                int FileSize;//要拷贝的文件的大小
                if (SectSize < FormerOpen.Length)//如果分段拷贝，即每次拷贝内容小于文件总长度
                {
                    byte[] buffer = new byte[SectSize];//根据传输的大小，定义一个字节数组
                    int copied = 0;//记录传输的大小

                    while (copied <= ((int)FormerOpen.Length - SectSize))//拷贝主体部分
                    {
                        FileSize = FormerOpen.Read(buffer, 0, SectSize);//从0开始读，每次最大读SectSize
                        FormerOpen.Flush();//清空缓存
                        ToFileOpen.Write(buffer, 0, SectSize);//向目的文件写入字节
                        ToFileOpen.Flush();//清空缓存
                        ToFileOpen.Position = FormerOpen.Position;//使源文件和目的文件流的位置相同
                        copied += FileSize;//记录已拷贝的大小

                    }
                    int left = (int)FormerOpen.Length - copied;//获取剩余大小
                    FileSize = FormerOpen.Read(buffer, 0, left);//读取剩余的字节
                    FormerOpen.Flush();//清空缓存
                    ToFileOpen.Write(buffer, 0, left);//写入剩余的部分
                    ToFileOpen.Flush();//清空缓存
                }
                else//如果整体拷贝，即每次拷贝内容大于文件总长度
                {
                    byte[] buffer = new byte[FormerOpen.Length];//获取文件的大小
                    FormerOpen.Read(buffer, 0, (int)FormerOpen.Length);//读取源文件的字节
                    FormerOpen.Flush();//清空缓存
                    ToFileOpen.Write(buffer, 0, (int)FormerOpen.Length);//写放字节
                    ToFileOpen.Flush();//清空缓存
                }
                FormerOpen.Close();//释放所有资源
                FormerOpen.Dispose();
                ToFileOpen.Close();//释放所有资源
                ToFileOpen.Dispose();
                string a = System.IO.Path.GetDirectoryName(dfile) + "\\" + System.IO.Path.GetFileNameWithoutExtension(dfile) + type;
                if (File.Exists(a)) File.Delete(a);
                File.Move(dfile, a);
                if (delect) File.Delete(sfile);                             
                return true;
            }
            catch
            {
                return false;
            }

        }
        /// <summary>
        /// 文件拷贝
        /// </summary>
        /// <param name="sfile">源文件路径</param>
        /// <param name="dfile">目的文件路径</param>
        /// <param name="SectSize">传输大小 1048576</param>
        /// <param name="type">文件扩展名</param>
        /// <param name="index">进度值</param>
        /// <returns>是否成功</returns>
        public static bool DTCopyFile(this string sfile, string dfile, int SectSize, string type , int index)
        {
            try
            {
                FileStream fileToCreate = new FileStream(dfile, FileMode.Create);//创建目的文件，如果已存在将被覆盖
                fileToCreate.Close();//关闭所有资源
                fileToCreate.Dispose();//释放所有资源
                FileStream FormerOpen = new FileStream(sfile, FileMode.Open, FileAccess.Read);//以只读方式打开源文件
                FileStream ToFileOpen = new FileStream(dfile, FileMode.Append, FileAccess.Write);//以写方式打开目的文件
                int max = Convert.ToInt32(Math.Ceiling((double)FormerOpen.Length / (double)SectSize));//根据一次传输的大小，计算传输的个数
                int FileSize;//要拷贝的文件的大小
                if (SectSize < FormerOpen.Length)//如果分段拷贝，即每次拷贝内容小于文件总长度
                {
                    byte[] buffer = new byte[SectSize];//根据传输的大小，定义一个字节数组
                    int copied = 0;//记录传输的大小

                    while (copied <= ((int)FormerOpen.Length - SectSize))//拷贝主体部分
                    {
                        FileSize = FormerOpen.Read(buffer, 0, SectSize);//从0开始读，每次最大读SectSize
                        FormerOpen.Flush();//清空缓存
                        ToFileOpen.Write(buffer, 0, SectSize);//向目的文件写入字节
                        ToFileOpen.Flush();//清空缓存
                        ToFileOpen.Position = FormerOpen.Position;//使源文件和目的文件流的位置相同
                        copied += FileSize;//记录已拷贝的大小

                    }
                    int left = (int)FormerOpen.Length - copied;//获取剩余大小
                    FileSize = FormerOpen.Read(buffer, 0, left);//读取剩余的字节
                    FormerOpen.Flush();//清空缓存
                    ToFileOpen.Write(buffer, 0, left);//写入剩余的部分
                    ToFileOpen.Flush();//清空缓存
                }
                else//如果整体拷贝，即每次拷贝内容大于文件总长度
                {
                    byte[] buffer = new byte[FormerOpen.Length];//获取文件的大小
                    FormerOpen.Read(buffer, 0, (int)FormerOpen.Length);//读取源文件的字节
                    FormerOpen.Flush();//清空缓存
                    ToFileOpen.Write(buffer, 0, (int)FormerOpen.Length);//写放字节
                    ToFileOpen.Flush();//清空缓存
                }
                FormerOpen.Close();//释放所有资源
                FormerOpen.Dispose();
                ToFileOpen.Close();//释放所有资源
                ToFileOpen.Dispose();
                string a = System.IO.Path.GetDirectoryName(dfile) + "\\" + System.IO.Path.GetFileNameWithoutExtension(dfile) + type;
                if (File.Exists(a)) File.Delete(a);
                File.Move(dfile, a);
                File.Delete(sfile);
                return true;
            }
            catch
            { 
                return false;
            }

        }

        /// 检测文件是否可以使用true为可以使用false为不可使用
        /// 
        /// <param name="file">文件名</param>
        /// <returns></returns>
        public static bool DTLockft(this string file)
        {
            try
            {
                var fs = new FileStream(file as string, FileMode.Open, FileAccess.Read, FileShare.None);
                fs.Close();
                fs.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 内容写入
        /// </summary>
        /// <param name="path">保存路径</param>
        /// <returns></returns>
        public static bool WriteMessage(this string meg, string path)
        {
            try
            {
                FileStream log = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                log.Position = log.Length;
                StreamWriter sw = new StreamWriter(log);
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
        /// MD5加密函数
        /// <param name="input">参数</param>
        /// <returns>加密参数</returns>
        public static string GetMD5Hash(this string input)
        {
            var pwd = string.Empty;
            var md5 = MD5.Create(); //实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            var s = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            return s.Aggregate(pwd, (current, t) => current + t.ToString("x2"));
        }

       

        /// <summary>
        /// 启动应用
        /// </summary>
        /// <param name="repeat">是否重复启动</param>
        /// <returns></returns>
        public static bool ExeStart(this string path, bool repeat)
        {
            try
            {
                if (!repeat)
                {
                    var pr = System.Diagnostics.Process.GetProcessesByName(Path.GetFileNameWithoutExtension(path));
                    if (pr.Length > 0) return false;
                }

        
                    //创建启动对象  
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = path,
                    Verb = "runas",
               
                };
                //设置启动动作,确保以管理员身份运行  

                System.Diagnostics.Process.Start(startInfo);                 

                return true;
            }
            catch (Exception ex)
            {
                Tools.Errorlog errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                return false;
            }
           
        }
        /// <summary>
        /// 启动应用
        /// </summary>
        /// <param name="repeat">是否重复启动</param>
        /// <returns></returns>
        public static bool ExeStart(this string path, bool repeat, string Arguments)
        {
            try
            {
                if (!repeat)
                {
                    var pr = System.Diagnostics.Process.GetProcessesByName(Path.GetFileNameWithoutExtension(path));
                    if (pr.Length > 0) return false;
                }

        
                    //创建启动对象  
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = path,
                    Verb = "runas",
                    Arguments = Arguments
                };
                //设置启动动作,确保以管理员身份运行  

                System.Diagnostics.Process.Start(startInfo);                 

                return true;
            }
            catch (Exception ex)
            {
                Tools.Errorlog errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                return false;
            }
           
        }
        public static bool IsExeStart(this string exeName)
        {
            var pr = System.Diagnostics.Process.GetProcessesByName(exeName);
            return pr.Length > 0;
        }
    }
}
