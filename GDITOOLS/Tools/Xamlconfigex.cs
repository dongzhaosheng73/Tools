using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xaml;
using System.Xml.Serialization;

namespace DTTOOLS.Tools
{
    public class Xamlconfigex
    {
        public static bool XamlWriter<T>(string dir,T obj)where T:class 
        {
            try
            {
              
                using (var stringWriter = new StringWriter(new StringBuilder()))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(stringWriter, obj);
                    var fs = new FileStream(dir,
                        FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.ReadWrite);

                    fs.Seek(0, SeekOrigin.Begin);

                    fs.SetLength(0);

                    var sw = new StreamWriter(fs);
               
                    sw.Write(stringWriter.ToString());
                    sw.Close();
                    fs.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                return false;
            }
            
        }

        public static T XamlRead<T>(string dir) where T : new()
        {
            try
            {
                  if (!File.Exists(dir))
                  {
                      return new T();
                  }

                var result = new T();

                var xmlSerializer = new XmlSerializer(typeof(T));

                FileStream fs = new FileStream(dir, FileMode.Open,
                    FileAccess.Read, FileShare.Read);
                fs.Seek(0, SeekOrigin.Begin);

                StreamReader rw = new StreamReader(fs);

                result = (T)xmlSerializer.Deserialize(rw);

                rw.Close();

                fs.Close();

                return result;
            }
            catch (Exception ex)
            {
                var result = new T();
                var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                return result;
            }
        
        }



    }
}
