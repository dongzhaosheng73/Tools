using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DTTOOLS
{
    public interface IGDITools : IDisposable
    {
        void LoadBitmap(string path);

        System.Windows.Media.ImageBrush LoadImageBrush(string path, int w, int h);

        void Dispose();
    }
}
