using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DTTOOLS
{
    public abstract class GDIToolsBase : IGDITools
    {
        public GDIToolsBase()
        {
            isDisplsed = false;
        }
        public GDIToolsBase(string path)
            : this()
        {
            if (String.IsNullOrWhiteSpace(path))
                return;

            this.LoadBitmap(path);
        }
        public abstract void Inti() ;
        public Bitmap BitmapImage { get; set; }
        public void LoadBitmap(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                return;

            BitmapImage = GDITools.LoadBitmap(path);
        }

        public string FilePath { get; set; }

        public System.Windows.Media.ImageBrush BitmapImageBrush { set; get; }
        /// <summary>
        /// 加载图像
        /// </summary>
        /// <param name="path">图像路径</param>
        /// <param name="w">宽</param>
        /// <param name="h">高</param>
        public System.Windows.Media.ImageBrush LoadImageBrush(string path, int w, int h)
        {
            if (String.IsNullOrWhiteSpace(path))
                return null;
            return GDITools.LoadImageBrush(path, w, h);
        }

        #region Dispose

        protected bool isDisplsed { get; set; }

        ~GDIToolsBase()
        {
            Dispose();
        }

        public void Close()
        {
            Dispose();
        }
         
        public void Dispose()
        {
            Dispose(isDisplsed);
        }

        public virtual void Dispose(bool isDisplsed)
        {
            if (isDisplsed)
                return;

            if (BitmapImage != null)
            {
                BitmapImage.Dispose();
                BitmapImage = null;
            }

            GDITools.FlushMemory();

            isDisplsed = true;
        }

        #endregion
    }


    public class ImageWrapper : GDIToolsBase
    {
        public ImageWrapper() : base() { }
        //public ImageWrapper(string path) : base(path) { }

        public override void Inti() { }
    }

    public static  class ImageExtensions
    {
        public static  int GetLength(this IGDITools @object)
        {
            return 1;
        }
    }

}
