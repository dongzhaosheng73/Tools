using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using ADODB;
using System.IO;
using System.Windows.Navigation;
using DTTOOLS.Tools;

namespace DTTOOLS.IOHepler
{
    /// <summary>
    /// IO操作类
    /// </summary>
    public class IOHepler
    {
        public  IOHepler()
        {
             
        }

        public delegate void DeleteIndex(int index);

        public event DeleteIndex Event_DeletnIndex;
        /// <summary>
        /// 删除指定目录下的所有目录
        /// </summary>
        /// <param name="path">目标目录</param>
        /// <returns></returns>
        public bool DeleteDirectorys(string path)
        {
            var index = 0;
            foreach (
                var dir in Directory.GetDirectories(path))
            {
                foreach (var file in Directory.GetFiles(dir ,"*.*", SearchOption.AllDirectories))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                        errorlog.WriteError(ex);
                    }
                    index=index+1;
                    if (Event_DeletnIndex != null) Event_DeletnIndex(index);
                }
                try
                {
                    if (!Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories).Any()) Directory.Delete(dir);
                }
                catch (Exception ex)
                {
                    var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                    errorlog.WriteError(ex);
                }
            }

            return true;
        }
    }
   

}
