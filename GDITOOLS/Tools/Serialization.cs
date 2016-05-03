using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace DTTOOLS.Tools
{
    public class Serialization
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public static string GetSerialization(object ob)
        {
            var serializer = new JavaScriptSerializer();

            return serializer.Serialize(ob);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static T ScriptDeserialize<T>(string strJson)
        {
            var js = new JavaScriptSerializer();

            return js.Deserialize<T>(strJson);
        }
    }
}
