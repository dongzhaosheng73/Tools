using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using DTTOOLS.Print;

namespace DTTOOLS.WinAIP
{
    public static class WindowsAPI
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        private static extern int SendMessage(IntPtr hwnd, int WM_COPYDATA, IntPtr wParam, ref Rectangle res);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="winName">窗体名</param>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public static int SendMessage(string winName,int msg)
        {
            var Iptr = FindWindow(null, winName);
            var rect = new Rectangle();
            return  SendMessage(Iptr, msg, (IntPtr)0, ref rect);
        }
    }
}
