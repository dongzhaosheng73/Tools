using System;
using System.Drawing;
using DTTOOLS.WinAIP;

namespace DTTOOLS.CommunicationHepler
{
    public class WinMessage
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="winName">窗体名</param>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public  int SendMessage(string winName, int msg)
        {
            var Iptr = WindowsAPI.FindWindow(null, winName);
            var rect = new Rectangle();
            return WindowsAPI.SendMessage(Iptr, msg, (IntPtr)0, ref rect);
        }
    }
}
