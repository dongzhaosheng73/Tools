using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using CsharpHttpHelper;

namespace DTTOOLS.WebTools
{
    public class WebTools
    {
        public static bool DownloadImage(string url, string savedir)
        {
            var uerphoto = savedir;
            var http = new HttpHelper();
            var itemm = new HttpItem { URL = url, Method = "GET", ResultType = CsharpHttpHelper.Enum.ResultType.Byte };
            var rt = http.GetHtml(itemm);
            if (rt.ResultByte == null) return false;
            try
            {
                HttpHelper.GetImage(rt.ResultByte).Save(uerphoto, ImageFormat.Jpeg);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
