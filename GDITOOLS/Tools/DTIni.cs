using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTTOOLS.WinAIP;

namespace DTTOOLS
{
    public class DTIni
    {
        #region API
    

        #endregion
        #region 变量
        /// <summary>
        /// INI路径
        /// </summary>
        private static string INIsPath { set; get; }
        #endregion
        #region INI
        /// <summary>
        /// 设置INI路径
        /// </summary>
        /// <param name="path"></param>
        public static void Ini(string path)
        {
            INIsPath = path;
        }
        /// <summary>
        /// 设置INI路径
        /// </summary>
        public static void Ini()
        {
            INIsPath = System.IO.Directory.GetCurrentDirectory() + "\\Set.ini";
        }
        /// <summary>
        /// INI写入 使用前先调用void Ini 赋值路径
        /// </summary>
        /// <param name="section">配置节</param>
        /// <param name="key">键名</param>
        /// <param name="value">值</param>
        public static void Write(string section, string key, string value)
        {

            // section=配置节，key=键名，value=键值，path=路径

            WindowsAPI.WritePrivateProfileString(section, key, value, INIsPath);

        }
        /// <summary>
        /// INI读取 使用前先调用void Ini 赋值路径
        /// </summary>
        /// <param name="section">配置节</param>
        /// <param name="key">键名</param>
        /// <returns>返回读取值</returns>
        public static string ReadValue(string section, string key)
        {

            // 每次从ini中读取多少字节

            System.Text.StringBuilder temp = new System.Text.StringBuilder(255);

            // section=配置节，key=键名，temp=上面，path=路径

            WindowsAPI.GetPrivateProfileString(section, key, "", temp, 255, INIsPath);

            return temp.ToString();

        }
        /// <summary>
        /// INI读取预设值比较返回true false
        /// </summary>
        /// <param name="section">配置节</param>
        /// <param name="key">键名</param>
        /// <param name="comparedvalue">预设值</param>
        /// <returns>返回bool</returns>
        public static bool ReadValue(string section, string key, string comparedvalue)
        {

            // 每次从ini中读取多少字节

            System.Text.StringBuilder temp = new System.Text.StringBuilder(255);

            // section=配置节，key=键名，temp=上面，path=路径

            WindowsAPI.GetPrivateProfileString(section, key, "", temp, 255, INIsPath);

            if (temp.ToString() != comparedvalue) return false;
            return true;

        }
        #endregion
    }
}
