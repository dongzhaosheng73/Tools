using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DTTOOLS.Print
{
    public interface IDTPrint
    {

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="image">图像流</param>
        /// <param name="isCut">是否裁切</param>
        void Print(Bitmap image, bool isCut);
        /// <summary>
        /// 打印预览
        /// </summary>
        void PrintView(Bitmap image, bool isCut);
        /// <summary>
        /// 释放资源
        /// </summary>
        void Dispose();
    }
}
