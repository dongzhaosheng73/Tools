using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows;
using System.Media;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Security;
using System.Management;
using System.Windows.Media.Imaging;
using DTTOOLS.Tools;
using ThoughtWorks.QRCode.Codec;


namespace DTTOOLS
{

    public  class GDITools
    {
        #region API
        [DllImport("kernel32.dll")]
        public static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);
        #endregion
        #region 各种变量
        //public static System.Drawing.Point TempXY;
        public struct FitSizeTable// 适配结构
        {
            public long fitw;
            public long fith;
            public float fitsize;
        }
        public struct FitSizeTableD// 适配结构
        {
            public double fitw;
            public double fith;
            public double fitsize;
        }
        public static FitSizeTable Picsize;

        public struct AdaptationSize
        { 
           public   FitSizeTableD size;
           public   PointF xy;

        }
        #endregion
        public GDITools()
        {


        }

        public static AdaptationSize GetAdaptationSize(double w, double h, double dw, double dh, bool outsize)
        {
            AdaptationSize bitsize;
           
            FitSizeTableD vPicsize;

            PointF vTempXY;

            if (outsize == false)
            {
                  vPicsize = FitSize(w, h, dw, dh);

                  vTempXY = PointXY(vPicsize, dw, dh);
            }
            else
            {
                 vPicsize = FitSizeOutSide(w, h, dw, dh);

                 vTempXY = PointXY(vPicsize, dw, dh);
            }

            bitsize.size = vPicsize;
            bitsize.xy = vTempXY;

            return bitsize;

        }
        /// <summary>
        /// 图像加载
        /// </summary>
        /// <param name="path">加载路径</param>
        /// <returns>位图</returns>
        public static Bitmap LoadBitmap(string path)
        {

            MemoryStream ms = null;
            try
            {
                FileStream fStream = new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read);

                byte[] bytes = new byte[fStream.Length];

                fStream.Read(bytes, 0, bytes.Length);

                ms = new MemoryStream(bytes);

                fStream.Dispose();
                fStream.Close();
                             
                Bitmap  TempBitmap = (Bitmap)Bitmap.FromStream(ms,false, false);

                Bitmap rBitmap = new Bitmap(TempBitmap.Width, TempBitmap.Height);

                Graphics g = Graphics.FromImage(rBitmap);

                g.DrawImage(TempBitmap, 0, 0, TempBitmap.Width,TempBitmap.Height);
             
                g.Dispose();           
               
                TempBitmap.Dispose();

                return rBitmap;
                
            }
            catch(Exception ex)
            {
                var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                ms = null;
                return null;
            }
            finally
            {
                ms.Dispose();
            }
        }
        /// <summary>
        /// 返回ImageBrush
        /// </summary>
        /// <param name="path">图像路径</param>
        /// <param name="w">显示宽度0为默认</param>
        /// <param name="h">显示高度0为默认</param>
        /// <returns></returns>
        public static ImageBrush LoadImageBrush(string path, int w, int h)
        {
            try
            {
                ImageBrush b = new ImageBrush();
                FileStream fs = new System.IO.FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Dispose();
                fs.Close();
                MemoryStream ms = new MemoryStream(bytes);
                System.Windows.Media.Imaging.BitmapImage bit = new System.Windows.Media.Imaging.BitmapImage();
                bit.BeginInit();
                if (w > 0) bit.DecodePixelWidth = w;
                if (h > 0) bit.DecodePixelHeight = h;
                bit.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.None;
                bit.StreamSource = ms;
                bit.EndInit();
                b.ImageSource = bit;
                b.Stretch = Stretch.Uniform;
                return b;
            }
            catch (Exception ex)
            {
                var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                return null;
            }
            finally
            {
                FlushMemory();
            }
        }

        /// <summary>
        /// 返回ImageBrush
        /// </summary>
        /// <param name="path">图像路径</param>
        /// <param name="w">显示宽度0为默认</param>
        /// <param name="h">显示高度0为默认</param>
        /// <param name="stretch">适配方式</param>
        /// <returns></returns>
        public static ImageBrush LoadImageBrush(string path, int w, int h,Stretch stretch )
        {
            try
            {
                ImageBrush b = new ImageBrush();
                FileStream fs = new System.IO.FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Dispose();
                fs.Close();
                MemoryStream ms = new MemoryStream(bytes);
                BitmapImage bit = new BitmapImage();
                bit.BeginInit();
                if (w > 0) bit.DecodePixelWidth = w;
                if (h > 0) bit.DecodePixelHeight = h;
                bit.CacheOption = BitmapCacheOption.None;
                bit.StreamSource = ms;
                bit.EndInit();
                b.ImageSource = bit;
                b.Stretch = stretch;
                return b;
            }
            catch (Exception ex)
            {
                var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                return null;
            }
            finally
            {
                FlushMemory();
            }
        }

        public static MemoryStream LoadBitmapStream(string path, int w, int h, bool Isfit)
        {
            try
            {
                var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Dispose();
                fs.Close();
                var ms = new MemoryStream(bytes);
                if (w > 0 & h > 0)
                {
                    DateTime t1 = DateTime.Now;
                    var sBitmap = new Bitmap(ms);
                    var rBitmap = GetThumBitmap(sBitmap, w, h, Isfit);
                    var rStream = new MemoryStream();
                    rBitmap.Save(rStream, sBitmap.RawFormat);
                    rBitmap.Dispose();
                    sBitmap.Dispose();
                    ms.Dispose();
                    rStream.Seek(0, SeekOrigin.Begin);
                    DateTime t2 = DateTime.Now;
                    TimeSpan ts = t2.Subtract(t1);
                    return rStream;
                }
                else
                {
                    return ms;
                }
            }
            catch (Exception ex)
            {
                var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                return null;
            }
            finally
            {
                FlushMemory();
            }
        }
        /// <summary>
        /// 返回BitmapImage
        /// </summary>
        /// <param name="path">图像路径</param>
        /// <param name="w">显示宽度0为默认</param>
        /// <param name="h">显示高度0为默认</param>
        /// <returns></returns>
        public static BitmapImage LoadBitmapImage( string path, int w, int h,bool Isfit)
        {
            try
            {

                var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Dispose();
                fs.Close();
                var ms = new MemoryStream(bytes);
                if (w > 0 & h > 0)
                {
                    var sBitmap = new Bitmap(ms);
                    var rBitmap = GetThumBitmap(sBitmap, w, h, Isfit);
                    var rStream = new MemoryStream();
                    rBitmap.Save(rStream, sBitmap.RawFormat);
                    rBitmap.Dispose();
                    sBitmap.Dispose();
                    ms.Dispose();
                    rStream.Seek(0, SeekOrigin.Begin);
                    var bit = new BitmapImage();
                    bit.BeginInit();
                    bit.CacheOption = BitmapCacheOption.None;
                    bit.StreamSource = rStream;
                    bit.EndInit();
                    return bit;
                }
                else
                {
                    var bit = new BitmapImage();
                    bit.BeginInit();
                    bit.CacheOption = BitmapCacheOption.None;
                    bit.StreamSource = ms;
                    bit.EndInit();
                    return bit;
                }           
            }
            catch (Exception ex)
            {
                var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                return null;
            }
            finally
            {
                FlushMemory();
            }
        }
        /// <summary>
        /// 内存释放
        /// </summary>
        public static void FlushMemory()
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FlushMemory" + ex.Message);
            }
        }
        /// <summary>
        /// 获取适配坐标
        /// </summary>
        /// <param name="picsize">适配比例</param>
        /// <param name="w">容器宽度</param>
        /// <param name="h">容器高度</param>
        /// <returns>TempXY</returns>
        public static System.Drawing.Point PointXY(FitSizeTable picsize, int w, int h)
        {

            var TempXY = new System.Drawing.Point();

            if (w > picsize.fitw)
            {
                TempXY.X = (int)((w - picsize.fitw) / 2);
            }
            else if (w < picsize.fitw)
            {
                TempXY.X = (int)((w - picsize.fitw) / 2);
            }
            else
            {
                TempXY.X = 0;
            }

            if (h > picsize.fith)
            {
                TempXY.Y = (int)((h - picsize.fith) / 2);

            }
            else if (h < picsize.fith)
            {
                TempXY.Y = (int)((h - picsize.fith) / 2);
            }
            else
            {
                TempXY.Y = 0;
            }

            return TempXY;

        }
        /// <summary>
        /// 获取适配坐标
        /// </summary>
        /// <param name="picsize">适配比例</param>
        /// <param name="w">容器宽度</param>
        /// <param name="h">容器高度</param>
        /// <returns>TempXY</returns>
        public static PointF PointXY(FitSizeTableD picsize, double w, double h)
        {
            PointF dxy = new PointF();
           
            if (w > picsize.fitw)
            {
                dxy.X = (float)((w - picsize.fitw) / 2);
            }
            else if (w < picsize.fitw)
            {
                dxy.X = (float)((w - picsize.fitw) / 2);
            }
            else
            {
                dxy.X = 0;
            }

            if (h > picsize.fith)
            {
                dxy.Y = (float)((h - picsize.fith) / 2);

            }
            else if (h < picsize.fith)
            {
                dxy.Y = (float)((h - picsize.fith) / 2);
            }
            else
            {
                dxy.Y = 0;
            }

            return dxy;

        }
        /// <summary>
        /// 获取适配比例
        /// </summary>
        /// <param name="Srw">原图宽度</param>
        /// <param name="Srh">原图高度</param>
        /// <param name="Dsw">容器宽度</param>
        /// <param name="Dsh">容器宽度</param>
        /// <returns>FitSizeTable</returns>
        public static FitSizeTable FitSize(long Srw, long Srh, long Dsw, long Dsh) //适配原图像与容器
        {
            float SrBL = (float)Srh / Srw;
            float DsBL = (float)Dsh / Dsw;

            FitSizeTable DsetFitsize;

            if (SrBL > DsBL)
            {
                DsetFitsize.fith = Dsh;


                DsetFitsize.fitw = ((long)Math.Round((float)Srw / (float)Srh * Dsh, MidpointRounding.AwayFromZero));
            }
            else
            {
                if (SrBL < DsBL)
                {
                    DsetFitsize.fitw = Dsw;
                    DsetFitsize.fith = ((long)Math.Round((float)Srh / (float)Srw * Dsw, MidpointRounding.AwayFromZero));
                }
                else
                {
                    DsetFitsize.fith = Dsh;
                    DsetFitsize.fitw = Dsw;
                }
            }

            DsetFitsize.fitsize = (float)Math.Round((float)DsetFitsize.fith / Srh, 5, MidpointRounding.AwayFromZero);
            return DsetFitsize;
        }
        /// <summary>
        /// 获取适配比例
        /// </summary>
        /// <param name="Srw">原图宽度</param>
        /// <param name="Srh">原图高度</param>
        /// <param name="Dsw">容器宽度</param>
        /// <param name="Dsh">容器宽度</param>
        /// <returns>FitSizeTable</returns>
        public static FitSizeTableD FitSize(double Srw, double Srh, double Dsw, double Dsh) //适配原图像与容器
        {
            double SrBL = Srh / Srw;
            double DsBL = Dsh / Dsw;

            FitSizeTableD DsetFitsize;

            if (SrBL > DsBL)
            {
                DsetFitsize.fith = Dsh;


                DsetFitsize.fitw = (Math.Round(Srw /Srh * Dsh, MidpointRounding.AwayFromZero));
            }
            else
            {
                if (SrBL < DsBL)
                {
                    DsetFitsize.fitw = Dsw;
                    DsetFitsize.fith = (Math.Round(Srh / Srw * Dsw, MidpointRounding.AwayFromZero));
                }
                else
                {
                    DsetFitsize.fith = Dsh;
                    DsetFitsize.fitw = Dsw;
                }
            }

            DsetFitsize.fitsize = Math.Round(DsetFitsize.fith / Srh, 5, MidpointRounding.AwayFromZero);
            return DsetFitsize;
        }
        /// <summary>
        /// 获取从外往里适配比例
        /// </summary>
        /// <param name="Srw">原图宽度</param>
        /// <param name="Srh">原图高度</param>
        /// <param name="Dsw">容器宽度</param>
        /// <param name="Dsh">容器宽度</param>
        /// <returns></returns>
        public static FitSizeTableD FitSizeOutSide(double Srw, double Srh, double Dsw, double Dsh)
        {
            double SrBL =Srh / Srw;
            double DsBL =Dsh / Dsw;

            FitSizeTableD DsetFitsize;

            if (SrBL < DsBL)
            {
                DsetFitsize.fith = Dsh;
                DsetFitsize.fitw = (Math.Round(Srw /Srh * Dsh));
            }
            else
            {
                if (SrBL > DsBL)
                {
                    DsetFitsize.fitw = Dsw;
                    DsetFitsize.fith = (Math.Round(Srh / Srw * Dsw));
                }
                else
                {
                    DsetFitsize.fith = Dsh;
                    DsetFitsize.fitw = Dsw;
                }
            }

            DsetFitsize.fitsize = Math.Round(DsetFitsize.fith / Srh, 5);
            return DsetFitsize;
        }
        /// <summary>
        /// 获取从外往里适配比例
        /// </summary>
        /// <param name="Srw">原图宽度</param>
        /// <param name="Srh">原图高度</param>
        /// <param name="Dsw">容器宽度</param>
        /// <param name="Dsh">容器宽度</param>
        /// <returns></returns>
        public static FitSizeTable FitSizeOutSide(long Srw, long Srh, long Dsw, long Dsh)
        {
            float SrBL = (float)Srh / Srw;
            float DsBL = (float)Dsh / Dsw;

            FitSizeTable DsetFitsize;

            if (SrBL < DsBL)
            {
                DsetFitsize.fith = Dsh;
                DsetFitsize.fitw = ((long)Math.Round((float)Srw / (float)Srh * Dsh));
            }
            else
            {
                if (SrBL > DsBL)
                {
                    DsetFitsize.fitw = Dsw;
                    DsetFitsize.fith = ((long)Math.Round((float)Srh / (float)Srw * Dsw));
                }
                else
                {
                    DsetFitsize.fith = Dsh;
                    DsetFitsize.fitw = Dsw;
                }
            }

            DsetFitsize.fitsize = (float)Math.Round((float)DsetFitsize.fith / Srh, 5);
            return DsetFitsize;
        }
        ///   <summary>   
        ///   获取缩略图  
        ///   </summary> 
        ///   <param name="Path">图像路径</param>
        ///   <param name="Dsw">指定要创建图像宽度</param> 
        ///   <param name="Dsh">指定要创建图像高度</param> 
        /// <returns>Bitmap</returns>
        public static Bitmap GetThumBitmap(string Path, int Dsw, int Dsh, bool outsize)
        {
            try
            {
                var fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                var bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Dispose();
                fs.Close();

                using (var ms = new MemoryStream(bytes))
                {
                    var TempBitmap = (Bitmap)Bitmap.FromStream(ms, false, false);

                    System.Drawing.Point TempXY;

                    if (outsize == false)
                    {
                        Picsize = FitSize(TempBitmap.Width, TempBitmap.Height, Dsw, Dsh);

                        TempXY = PointXY(Picsize, Dsw, Dsh);
                    }
                    else
                    {
                        Picsize = FitSizeOutSide(TempBitmap.Width, TempBitmap.Height, Dsw, Dsh);

                        TempXY = PointXY(Picsize, Dsw, Dsh);
                    }

                    Bitmap RtBitmap = new Bitmap(Dsw, Dsh);

                    Graphics g = Graphics.FromImage(RtBitmap);

                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    g.DrawImage(TempBitmap, TempXY.X, TempXY.Y, (int)Math.Round((TempBitmap.Width * Picsize.fitsize), MidpointRounding.ToEven), (int)Math.Round((TempBitmap.Height * Picsize.fitsize), MidpointRounding.ToEven));

                    g.Dispose();

                    TempBitmap.Dispose();

                    ms.Close();

                    ms.Dispose();

                    return RtBitmap;
                }
                           
            }
            catch(Exception ex)
            {
                var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                return null;
            }

        }
        ///   <summary>   
        ///   获取缩略图  
        ///   </summary> 
        ///   <param name="Path">位图</param>
        ///   <param name="Dsw">指定要创建图像宽度</param> 
        ///   <param name="Dsh">指定要创建图像高度</param> 
        /// <returns>Bitmap</returns>
        public static Bitmap GetThumBitmap(Bitmap temp, int Dsw, int Dsh, bool outsize)
        {
            try
            {
                var TempXY = new System.Drawing.Point();

                Bitmap TempBitmap = (Bitmap)temp.Clone();

                if (outsize == false)
                {
                    Picsize = FitSize(TempBitmap.Width, TempBitmap.Height, Dsw, Dsh);

                    TempXY = PointXY(Picsize, Dsw, Dsh);
                }
                else
                {
                    Picsize = FitSizeOutSide(TempBitmap.Width, TempBitmap.Height, Dsw, Dsh);

                    TempXY = PointXY(Picsize, Dsw, Dsh);
                }

                Bitmap RtBitmap = new Bitmap(Dsw, Dsh);

                RtBitmap.SetResolution(300, 300);

                Graphics g = Graphics.FromImage(RtBitmap);

                g.InterpolationMode = InterpolationMode.Bicubic;

                g.PixelOffsetMode = PixelOffsetMode.Default;

                g.DrawImage(TempBitmap, TempXY.X, TempXY.Y, (int)Math.Round((TempBitmap.Width * Picsize.fitsize), MidpointRounding.ToEven), (int)Math.Round((TempBitmap.Height * Picsize.fitsize), MidpointRounding.ToEven));

                g.Dispose();

                TempBitmap.Dispose();

                return RtBitmap;

            }
            catch(Exception ex)
            {
                var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                return null;
            }

        }
        /// <summary>
        /// 旋转图像
        /// </summary>
        /// <param name="temp">要旋转的位图</param>
        /// <param name="angle">要旋转的角度</param>
        /// <returns>Bitmap</returns>
        public static Bitmap Angle(Bitmap temp, float angle) //位图旋转
        {

            angle = angle % 360; //弧度转换

            double radian = angle * Math.PI / 180.0;
            double cos = Math.Cos(radian);
            double sin = Math.Sin(radian);

            int w = temp.Width;
            int h = temp.Height;
            int W = (int)(Math.Max(Math.Abs(w * cos - h * sin), Math.Abs(w * cos + h * sin)));
            int H = (int)(Math.Max(Math.Abs(w * sin - h * cos), Math.Abs(w * sin + h * cos)));


            Bitmap tempp = new Bitmap(W, H);

            Graphics g = Graphics.FromImage(tempp);

            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            System.Drawing.Point Offset = new System.Drawing.Point((W - w) / 2, (H - h) / 2);
            Rectangle rect = new Rectangle((int)Offset.X, (int)Offset.Y, w, h);

            System.Drawing.Point center = new System.Drawing.Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform((float)(360 - angle));
            g.TranslateTransform(-center.X, -center.Y);
            Rectangle recta = new Rectangle(0, 0, temp.Width, temp.Height);
            g.DrawImage(temp, rect);
            g.ResetTransform();
            //g.Save();
            g.Dispose();
            return tempp;

        }
        /// <summary>
        /// 缩放图像
        /// </summary>
        /// <param name="temp">位图</param>
        /// <param name="size">缩放大小</param>
        /// <returns>Bitmap</returns>
        public static Bitmap BitSize(Bitmap temp, float size, bool enlarge, int Dsw, int Dsh, bool outsize)
        {
            float tempsize = 1;
            if (size > 0.9)
                if (enlarge == true)
                {
                    tempsize = tempsize + size;
                }
                else
                {
                    tempsize = tempsize - size;
                }
            System.Drawing.Point TempXY;
            if (outsize == false)
            {
                Picsize = FitSize(temp.Width, temp.Height, Dsw, Dsh);

                Picsize.fitsize = Picsize.fitsize * tempsize;
                Picsize.fith = (long)Math.Round((Picsize.fith * tempsize), MidpointRounding.AwayFromZero);
                Picsize.fitw = (long)Math.Round((Picsize.fitw * tempsize), MidpointRounding.AwayFromZero);

                TempXY = PointXY(Picsize, Dsw, Dsh);
            }
            else
            {
                Picsize = FitSizeOutSide(temp.Width, temp.Height, Dsw, Dsh);

                Picsize.fitsize = Picsize.fitsize * tempsize;
                Picsize.fith = (long)Math.Round((Picsize.fith * tempsize), MidpointRounding.AwayFromZero);
                Picsize.fitw = (long)Math.Round((Picsize.fitw * tempsize), MidpointRounding.AwayFromZero);

                TempXY = PointXY(Picsize, Dsw, Dsh);
            }


            Bitmap RtBitmap = new Bitmap(Dsw, Dsh);

            Graphics g = Graphics.FromImage(RtBitmap);

            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            g.DrawImage(temp, TempXY.X, TempXY.Y, (int)Math.Round((temp.Width * Picsize.fitsize), MidpointRounding.AwayFromZero), (int)Math.Round((temp.Height * Picsize.fitsize), MidpointRounding.ToEven));

            g.Dispose();

            return RtBitmap;

        }
        /// <summary>
        /// RGB颜色调整
        /// </summary>
        /// <param name="bmp">位图</param>
        /// <param name="rVal">R红色量</param>
        /// <param name="gVal">G绿色量</param>
        /// <param name="bVal">B蓝色量</param>
        /// <returns>Bitmap</returns>
        public static Bitmap KiColorBalance(Bitmap bmp, int rVal, int gVal, int bVal)
        {

            if (bmp == null)
            {
                return null;
            }


            int h = bmp.Height;
            int w = bmp.Width;

            try
            {
                if (rVal > 255 || rVal < -255 || gVal > 255 || gVal < -255 || bVal > 255 || bVal < -255)
                {
                    return null;
                }
                Bitmap temp = (Bitmap)bmp.Clone();

                BitmapData srcData = temp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                unsafe
                {
                    byte* p = (byte*)srcData.Scan0.ToPointer();

                    int nOffset = srcData.Stride - w * 3;
                    int r, g, b;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {

                            b = p[0] + bVal;
                            if (bVal >= 0)
                            {
                                if (b > 255) b = 255;
                            }
                            else
                            {
                                if (b < 0) b = 0;
                            }

                            g = p[1] + gVal;
                            if (gVal >= 0)
                            {
                                if (g > 255) g = 255;
                            }
                            else
                            {
                                if (g < 0) g = 0;
                            }

                            r = p[2] + rVal;
                            if (rVal >= 0)
                            {
                                if (r > 255) r = 255;
                            }
                            else
                            {
                                if (r < 0) r = 0;
                            }

                            p[0] = (byte)b;
                            p[1] = (byte)g;
                            p[2] = (byte)r;

                            p += 3;
                        }

                        p += nOffset;


                    }
                } // end of unsafe

                temp.UnlockBits(srcData);

                return temp;
            }
            catch(Exception ex)
            {
                var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                return null;
            }

        }
        /// <summary>
        /// 亮度处理
        /// </summary>
        /// <param name="b">位图</param>
        /// <param name="degree">亮度</param>
        /// <returns>Bitmap</returns> 
        public static Bitmap KiLighten(Bitmap b, int degree)
        {
            int BPP = 4;

            if (degree < -255) degree = -255;
            if (degree > 255) degree = 255;

            int width = b.Width;
            int height = b.Height;

            BitmapData data = b.LockBits(new Rectangle(0, 0, width, height),
              ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* p = (byte*)data.Scan0;
                int offset = data.Stride - width * BPP;

                int pixel = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // 处理像素 B, G, R 亮度三分量
                        for (int i = 0; i < 3; i++)
                        {
                            pixel = p[i] + degree;

                            if (pixel < 0) pixel = 0;
                            if (pixel > 255) pixel = 255;

                            p[i] = (byte)pixel;
                        } // i

                        p += BPP;
                    }  // x
                    p += offset;
                } // y
            }

            b.UnlockBits(data);

            return b;
        } // end of Brightness
        /// <summary>
        /// 蓝背景抠像
        /// </summary>
        /// <param name="low"></param>
        /// <param name="hig"></param>
        /// <param name="metting"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static Bitmap BlueScreenMatting(int low, int hig, int metting, Bitmap bit)
        {
            bit.MakeTransparent(System.Drawing.Color.Blue);

            try
            {
                Rectangle ret = new Rectangle(0, 0, bit.Width, bit.Height);
                var vHeight = bit.Height;
                var vWidth = bit.Width;
                var bitmapData = bit.LockBits(ret, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                unsafe
                {
                    byte* pixels = (byte*)bitmapData.Scan0;

                    int nOffset = bitmapData.Stride - vWidth * 4;

                    for (int row = 0; row < vHeight; ++row)
                    {
                        for (int col = 0; col < vWidth; ++col)
                        {
                            var B = pixels[0];
                            var G = pixels[1];
                            var R = pixels[2];

                            int vAlpha =B - Math.Max(R, G);

                            if (vAlpha < 0) vAlpha = 0;

                            if ((vAlpha <= hig) && (vAlpha >= low))
                            {
                                vAlpha = 255;
                            }

                            if (vAlpha < metting)
                                vAlpha = 0;

                            vAlpha = 255 - vAlpha;

                            if (B > Math.Max(R, G))
                            {
                                pixels[1] = Math.Max(R, G);

                            }

                            pixels[3] = (byte)vAlpha;

                            pixels += 4;
                        }
                        pixels += nOffset;
                    }
                }

                bit.UnlockBits(bitmapData);

            }
            catch (Exception)
            {
                return null;
            }

            return bit;

        }
        /// <summary>
        /// 绿背景抠像
        /// </summary>
        /// <param name="low"></param>
        /// <param name="hig"></param>
        /// <param name="metting"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static Bitmap GreenScreenMatting(int low, int hig, int metting, Bitmap bit)
        {
            bit.MakeTransparent(System.Drawing.Color.Green);

            try
            {
                Rectangle ret = new Rectangle(0, 0, bit.Width, bit.Height);
                var vHeight = bit.Height;
                var vWidth = bit.Width;
                var bitmapData = bit.LockBits(ret, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                unsafe
                {
                    byte* pixels = (byte*)bitmapData.Scan0;

                    int nOffset = bitmapData.Stride - vWidth * 4;

                    for (int row = 0; row < vHeight; ++row)
                    {
                        for (int col = 0; col < vWidth; ++col)
                        {
                            var B = pixels[0];
                            var G = pixels[1];
                            var R = pixels[2];

                            int vAlpha = G - Math.Max(R, B);

                            if (vAlpha < 0) vAlpha = 0;

                            if ((vAlpha <= hig) && (vAlpha >= low))
                            {
                                vAlpha = 255;
                            }

                            if (vAlpha < metting)
                                vAlpha = 0;

                            vAlpha = 255 - vAlpha;

                            if (G > Math.Max(R, B))
                            {
                                pixels[1] = Math.Max(R, B);

                            }

                            pixels[3] = (byte)vAlpha;

                            pixels += 4;
                        }
                        pixels += nOffset;
                    }
                }

                bit.UnlockBits(bitmapData);

            }
            catch (Exception)
            {
                return null;
            }

            return bit;

        }
        /// <summary>
        /// BitmapImage  TO Bitmap
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapSource BitmapToBitmapImage(Bitmap bitmap)
        {
            using (System.Drawing.Bitmap source = bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap

                return bs;
            }
        }
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);
        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
        /// <summary>
        /// 对比处理
        /// </summary>
        /// <param name="b">位图</param>
        /// <param name="degree">对比</param>
        /// <returns>Bitmap</returns>
        public static Bitmap KiContrast( Bitmap b, int degree)
        {
            if (b == null)
            {
                return null;
            }
            try
            {

                if (degree != 0)
                {
                    if (degree < -100) degree = -100;
                    if (degree > 100) degree = 100;
                    double pixel = 0;
                    double contrast = (100.0 + degree) / 100.0;
                    contrast *= contrast;
                    int width = b.Width;
                    int height = b.Height;

                    BitmapData data = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    unsafe
                    {
                        byte* p = (byte*)data.Scan0;
                        int offset = data.Stride - width * 3;
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                // 处理指定位置像素的对比度
                                for (int i = 0; i < 3; i++)
                                {
                                    pixel = ((p[i] / 255.0 - 0.5) * contrast + 0.5) * 255;
                                    if (pixel < 0) pixel = 0;
                                    if (pixel > 255) pixel = 255;
                                    p[i] = (byte)pixel;
                                } // i
                                p += 3;
                            } // x
                            p += offset;
                        } // y
                    }
                    b.UnlockBits(data);
                    return b;
                }
                else
                {
                    return b;
                }
            }
            catch(Exception ex)
            {
                var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                return null;
            }
        }
        /// <summary>
        /// 锐化处理
        /// </summary>
        /// <param name="b">位图</param>
        /// <param name="val">锐化</param>
        /// <returns>Bitmap</returns>
        public static Bitmap KiSharpen(Bitmap b, float val)
        {
            if (b == null)
            {
                return null;
            }

            int w = b.Width;
            int h = b.Height;

            try
            {

                Bitmap bmpRtn = new Bitmap(w, h, b.PixelFormat);

                BitmapData srcData = b.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                BitmapData dstData = bmpRtn.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                unsafe
                {
                    byte* pIn = (byte*)srcData.Scan0.ToPointer();
                    byte* pOut = (byte*)dstData.Scan0.ToPointer();
                    int stride = srcData.Stride;
                    byte* p;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            //取周围9点的值。位于边缘上的点不做改变。
                            if (x == 0 || x == w - 1 || y == 0 || y == h - 1)
                            {
                                //不做
                                pOut[0] = pIn[0];
                                pOut[1] = pIn[1];
                                pOut[2] = pIn[2];
                            }
                            else
                            {
                                int r1, r2, r3, r4, r5, r6, r7, r8, r0;
                                int g1, g2, g3, g4, g5, g6, g7, g8, g0;
                                int b1, b2, b3, b4, b5, b6, b7, b8, b0;

                                float vR, vG, vB;

                                //左上
                                p = pIn - stride - 3;
                                r1 = p[2];
                                g1 = p[1];
                                b1 = p[0];

                                //正上
                                p = pIn - stride;
                                r2 = p[2];
                                g2 = p[1];
                                b2 = p[0];

                                //右上
                                p = pIn - stride + 3;
                                r3 = p[2];
                                g3 = p[1];
                                b3 = p[0];

                                //左侧
                                p = pIn - 3;
                                r4 = p[2];
                                g4 = p[1];
                                b4 = p[0];

                                //右侧
                                p = pIn + 3;
                                r5 = p[2];
                                g5 = p[1];
                                b5 = p[0];

                                //右下
                                p = pIn + stride - 3;
                                r6 = p[2];
                                g6 = p[1];
                                b6 = p[0];

                                //正下
                                p = pIn + stride;
                                r7 = p[2];
                                g7 = p[1];
                                b7 = p[0];

                                //右下
                                p = pIn + stride + 3;
                                r8 = p[2];
                                g8 = p[1];
                                b8 = p[0];

                                //自己
                                p = pIn;
                                r0 = p[2];
                                g0 = p[1];
                                b0 = p[0];

                                vR = (float)r0 - (float)(r1 + r2 + r3 + r4 + r5 + r6 + r7 + r8) / 8;
                                vG = (float)g0 - (float)(g1 + g2 + g3 + g4 + g5 + g6 + g7 + g8) / 8;
                                vB = (float)b0 - (float)(b1 + b2 + b3 + b4 + b5 + b6 + b7 + b8) / 8;

                                vR = r0 + vR * val;
                                vG = g0 + vG * val;
                                vB = b0 + vB * val;

                                if (vR > 0)
                                {
                                    vR = Math.Min(255, vR);
                                }
                                else
                                {
                                    vR = Math.Max(0, vR);
                                }

                                if (vG > 0)
                                {
                                    vG = Math.Min(255, vG);
                                }
                                else
                                {
                                    vG = Math.Max(0, vG);
                                }

                                if (vB > 0)
                                {
                                    vB = Math.Min(255, vB);
                                }
                                else
                                {
                                    vB = Math.Max(0, vB);
                                }

                                pOut[0] = (byte)vB;
                                pOut[1] = (byte)vG;
                                pOut[2] = (byte)vR;

                            }

                            pIn += 3;
                            pOut += 3;
                        }// end of x

                        pIn += srcData.Stride - w * 3;
                        pOut += srcData.Stride - w * 3;
                    } // end of y
                }

                b.UnlockBits(srcData);
                bmpRtn.UnlockBits(dstData);

                return bmpRtn;
            }
            catch(Exception ex)
            {
                var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                return null;
            }

        }

        /// <summary>
        /// 获取所有图像文件
        /// </summary>
        /// <param name="path"></param>
        public static List<string> GetFile(string path) //获取子文件夹及其文件
        {
            List<string> files = new List<string>();

            DirectoryInfo DriFile = new DirectoryInfo(path);

            foreach (FileSystemInfo DF in DriFile.GetFileSystemInfos())
            {
                if (DF is DirectoryInfo)
                {
                    GetFile(DriFile + DF.ToString() + '\\');
                }

            }
            foreach (FileInfo File in DriFile.GetFiles())
            {
                if (File.Extension.ToLower() == ".jpg")
                {
                    files.Add(DriFile + File.ToString());

                }
            }
            return files;
        }
        /// <summary>
        /// 合成图像
        /// </summary>
        /// <param name="bmp">合成原图</param>
        /// <param name="synpath">合成边路径</param>
        /// <param name="tx">合成起点x坐标</param>
        /// <param name="ty">合成起点y坐标</param> 
        /// <param name="pt">原图是否压在边框上</param>
        /// <returns></returns>
        public static Bitmap Synthesis(Bitmap bmp, string synpath,bool pt)
        {
            var fs = new FileStream(synpath,FileMode.Open,FileAccess.ReadWrite,FileShare.ReadWrite);

            byte[] filebytes = new byte[fs.Length];

            fs.Read(filebytes, 0, filebytes.Length);

            fs.Close();

            var backpicStream = new MemoryStream(filebytes);

            var tempBack = (Bitmap) new Bitmap(backpicStream).Clone();

            backpicStream.Close();

            Bitmap PrintT = new Bitmap(bmp.Width, bmp.Height);

            return (Bitmap)Synthesis(bmp, tempBack, 0, 0,pt);
            
        }
        /// <summary>
        /// 合成图像
        /// </summary>
        /// <param name="mpic">合成原图</param>
        /// <param name="backpic">边框</param>
        /// <param name="x">合成起点x坐标</param>
        /// <param name="y">合成起点y坐标</param>
        /// <param name="pt">原图是否压在边框上</param>
        /// <returns></returns>
        public static Bitmap Synthesis(Bitmap mpic, Bitmap backpic, int x, int y,bool pt)
        {
            Bitmap b = new Bitmap(backpic.Width, backpic.Height);
            Graphics g = Graphics.FromImage(b);
            if (pt)
            {
                g.DrawImage(backpic, 0, 0, backpic.Width, backpic.Height);
                g.DrawImage(mpic, x, y, mpic.Width, mpic.Height);
            }
            else
            {             
                g.DrawImage(mpic, x, y, mpic.Width, mpic.Height);
                g.DrawImage(backpic, 0, 0, backpic.Width, backpic.Height);
              
            }
        
            g.Dispose();
            mpic.Dispose();
            backpic.Dispose();
          
            return b;
        }
        /// <summary>
        /// 合成图像
        /// </summary>
        /// <param name="p1">合成原图</param>
        /// <param name="p2">边框路径</param>
        /// <param name="x">合成起点x坐标</param>
        /// <param name="y">合成起点y坐标</param>
        /// <returns></returns>
        public static MemoryStream Synthesis(Bitmap p1, string p2, int x, int y)
        {

            Bitmap mpic = (Bitmap)p1.Clone();

            MemoryStream backpicStream = new MemoryStream(File.ReadAllBytes(p2));

            Bitmap backpic = new Bitmap(backpicStream);

            MemoryStream returnms = new MemoryStream();

            Bitmap b = new Bitmap(backpic.Width, backpic.Height);
            Graphics g = Graphics.FromImage(b);
            g.DrawImage(mpic, x, y, backpic.Width, backpic.Height);
            g.DrawImage(backpic, 0, 0, backpic.Width, backpic.Height);
            g.Dispose();
            mpic.Dispose();
            backpic.Dispose();
            backpicStream.Dispose();
            b.Save(returnms, ImageFormat.Png);
            b.Dispose();
            return returnms;
        }
        /// <summary>
        /// 合成图像
        /// </summary>
        /// <param name="p1">合成原图</param>
        /// <param name="p2">边框路径</param>
        /// <param name="x">合成起点x坐标</param>
        /// <param name="y">合成起点y坐标</param>
        /// <returns></returns>
        public static Bitmap Synthesis(Bitmap p1, string p2, int x, int y,int x1,int y1)
        {
            Bitmap returnBitmap = null;

            MemoryStream backpicStream = null;
            try
            {
                var mpic = (Bitmap)p1.Clone();

                var fStream = new FileStream(p2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

                var bytes = new byte[fStream.Length];

                fStream.Read(bytes, 0, bytes.Length);

                fStream.Close();

                backpicStream = new MemoryStream(bytes);

                var backpic = new Bitmap(backpicStream);

                returnBitmap = new Bitmap(backpic.Width, backpic.Height);

                Graphics g = Graphics.FromImage(returnBitmap);             
                g.DrawImage(backpic, 0, 0, backpic.Width, backpic.Height);
                g.DrawImage(mpic, x, y, x1 - x, y1 - y);
                g.Dispose();
                mpic.Dispose();
                backpic.Dispose();
                backpicStream.Dispose();             
             
                return (Bitmap) returnBitmap.Clone();
            }
            catch (Exception ex)
            {
                var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                return null;
            }
            finally
            {
                if (returnBitmap != null) returnBitmap.Dispose();
                if (backpicStream != null) backpicStream.Close();
                if (p1 != null) p1.Dispose();
            }
        }

      
        /// <summary>
        ///图像合成
        /// </summary>
        /// <param name="p1">原图</param>
        /// <param name="p2">边框</param>
        /// <param name="x">起点坐标x</param>
        /// <param name="y">起点坐标y</param>
        /// <param name="w">宽</param>
        /// <param name="h">高</param>
        /// <param name="pt">原图是否压在边框上</param>
        /// <returns></returns>
        public static BitmapSource SynthesisBitmapSource(Bitmap p1, Bitmap p2, double x, double y, double w, double h, bool pt)
        {
            Bitmap returnBitmap = null;

            try
            {
                var mpic = (Bitmap)p1.Clone();

                var backpic = (Bitmap)p2.Clone();

                returnBitmap = new Bitmap(backpic.Width, backpic.Height);

                Graphics g = Graphics.FromImage(returnBitmap);
                if (pt)
                {
                    g.DrawImage(backpic, 0, 0, backpic.Width, backpic.Height);
                    g.DrawImage(mpic, (int)x, (int)y, (int)w, (int)h);
                }
                else
                {
                    g.DrawImage(mpic, (int)x, (int)y, (int)w, (int)h);
                    g.DrawImage(backpic, 0, 0, backpic.Width, backpic.Height);    
                }
                        
                g.Dispose();
                mpic.Dispose();
                backpic.Dispose();
                return BitmapToBitmapImage((Bitmap)returnBitmap.Clone());
            }
            catch (Exception ex)
            {
                var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                return null;
            }
            finally
            {
                if (returnBitmap != null) returnBitmap.Dispose();
                if (p1 != null) p1.Dispose();
            }
        }
        /// <summary>
        ///图像合成
        /// </summary>
        /// <param name="p1">原图</param>
        /// <param name="p2">边框</param>
        /// <param name="x">起点坐标x</param>
        /// <param name="y">起点坐标y</param>
        /// <param name="w">宽</param>
        /// <param name="h">高</param>
        /// <param name="pt">原图是否压在边框上</param>
        /// <returns></returns>
        public static Bitmap SynthesisBitmap(Bitmap p1, Bitmap p2, double x, double y, double w, double h, bool pt)
        {
            Bitmap returnBitmap = null;

            try
            {
                var mpic = (Bitmap)p1.Clone();

                var backpic = (Bitmap)p2.Clone();

                returnBitmap = new Bitmap(backpic.Width, backpic.Height);

                Graphics g = Graphics.FromImage(returnBitmap);
                if (pt)
                {
                    g.DrawImage(backpic, 0, 0, backpic.Width, backpic.Height);
                    g.DrawImage(mpic, (int)x, (int)y, (int)w, (int)h);
                }
                else
                {
                    g.DrawImage(mpic, (int)x, (int)y, (int)w, (int)h);
                    g.DrawImage(backpic, 0, 0, backpic.Width, backpic.Height);
                }

                g.Dispose();
                mpic.Dispose();
                backpic.Dispose();
                return (Bitmap)returnBitmap.Clone();
            }
            catch (Exception ex)
            {
                var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                return null;
            }
            finally
            {
                if (returnBitmap != null) returnBitmap.Dispose();
                if (p1 != null) p1.Dispose();
            }
        }
        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片</param>
        /// <param name="dFile">压缩后保存位置</param>
        /// <param name="tWidth">横图宽度</param>
        /// <param name="tWidth">竖图宽度</param>
        /// <param name="flag">压缩质量 1-100</param>
        /// <returns></returns>
        public static bool CompressImage(Bitmap sFile, string dFile, int tWidth, int vWidth, int flag)
        {
            Bitmap ob;

            Bitmap iSource = sFile;

            System.Drawing.Point TempXY;

            if (iSource.Width > iSource.Height)
            {
                double p = (double)iSource.Width / tWidth;
                int newtHeight = (int)Math.Round((iSource.Height / p), MidpointRounding.ToEven);
                Picsize = FitSize(iSource.Width, iSource.Height, tWidth, newtHeight);
                TempXY = PointXY(Picsize, tWidth, newtHeight);
                ob = new Bitmap(tWidth, newtHeight);
            }
            else
            {
                double p = (double)iSource.Width / vWidth;
                int newtHeight = (int)Math.Round((iSource.Height / p), MidpointRounding.ToEven);
                Picsize = FitSize(iSource.Width, iSource.Height, vWidth, newtHeight);
                TempXY = PointXY(Picsize, vWidth, newtHeight);
                ob = new Bitmap(vWidth, newtHeight);
            }

            ImageFormat tFormat = iSource.RawFormat;

            //按比例缩放            
            Graphics g = Graphics.FromImage(ob);
            g.Clear(System.Drawing.Color.WhiteSmoke);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(iSource, TempXY.X, TempXY.Y, (int)Math.Round((iSource.Width * Picsize.fitsize), MidpointRounding.AwayFromZero), (int)Math.Round((iSource.Height * Picsize.fitsize), MidpointRounding.ToEven));
            g.Dispose();
            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    if (File.Exists(dFile))
                    {
                        File.Delete(dFile);
                    }    
                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                }
                else
                {
                    if (File.Exists(dFile))
                    {
                        File.Delete(dFile);
                    } 
                    ob.Save(dFile, tFormat);
                }
                return true;
            }
            catch(Exception ex)
            {
                var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                return false;
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
            }

        }
        /// <summary>
        /// 二维码生成
        /// </summary>
        /// <param name="qRstring"></param>
        /// <returns></returns>
        public static Image photoQR(string qRstring)
        {
            var qrCodeEncoder = new QRCodeEncoder
            {
                QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE,
                QRCodeScale = 4,
                QRCodeVersion = 5,
                QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M
            };
            String data = qRstring;
            Image image = qrCodeEncoder.Encode(data);
            return image;
        }
        /// <summary>
        /// 二维码生成
        /// </summary>
        /// <param name="qRstring">字符串</param>
        /// <param name="qrEncoding">三种尺寸：BYTE ，ALPHA_NUMERIC，NUMERIC</param>
        /// <param name="level">大小：L M Q H</param>
        /// <param name="version">版本4</param>
        /// <param name="scale">比例</param>
        /// <returns></returns>
        public static Image photoQR(string qRstring, string qrEncoding, string level, int version, int scale)
        {

            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            string encoding = qrEncoding;
            switch (encoding)
            {
                case "Byte":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                    break;
                case "AlphaNumeric":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
                    break;
                case "Numeric":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
                    break;
                default:
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                    break;
            }

            qrCodeEncoder.QRCodeScale = scale;
            qrCodeEncoder.QRCodeVersion = version;
            switch (level)
            {
                case "L":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
                    break;
                case "M":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                    break;
                case "Q":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
                    break;
                default:
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
                    break;
            }

            return qrCodeEncoder.Encode(qRstring);
        }

        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片</param>
        /// <param name="dFile">压缩后保存位置</param>
        /// <param name="tWidth">横图宽度</param>
        /// <param name="flag">压缩质量 1-100</param>
        /// <param name="IsDelect">是否删除原始照片</param>
        /// <returns></returns>
        public static bool CompressImage(string sFile, string dFile, int tWidth, int vWidth, int flag,bool IsDelect)
        {

          
            Bitmap ob;

            FileStream fstream = new FileStream(sFile,FileMode.Open,FileAccess.ReadWrite,FileShare.ReadWrite);
            
            byte[] bytes = new byte[fstream.Length];

            fstream.Read(bytes, 0, bytes.Length);

            fstream.Dispose();

            fstream.Close();

            MemoryStream ms = new MemoryStream(bytes);

            Bitmap iSource = (Bitmap)Bitmap.FromStream(ms, false, false);

            System.Drawing.Point TempXY;

            if (iSource.Width > iSource.Height)
            {
                double p = (double)iSource.Width / tWidth;
                int newtHeight = (int)Math.Round((iSource.Height / p), MidpointRounding.ToEven);
                Picsize = FitSize(iSource.Width, iSource.Height, tWidth, newtHeight);
                TempXY = PointXY(Picsize, tWidth, newtHeight);
                ob = new Bitmap(tWidth, newtHeight);
            }
            else
            {
                double p = (double)iSource.Width / vWidth;
                int newtHeight = (int)Math.Round((iSource.Height / p), MidpointRounding.ToEven);
                Picsize = FitSize(iSource.Width, iSource.Height, vWidth, newtHeight);
                TempXY = PointXY(Picsize, vWidth, newtHeight);
                ob = new Bitmap(vWidth, newtHeight);
            }

            ImageFormat tFormat = iSource.RawFormat;

            //按比例缩放            
            Graphics g = Graphics.FromImage(ob);
            g.Clear(System.Drawing.Color.WhiteSmoke);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(iSource, TempXY.X, TempXY.Y, (int)Math.Round((iSource.Width * Picsize.fitsize), MidpointRounding.AwayFromZero), (int)Math.Round((iSource.Height * Picsize.fitsize), MidpointRounding.ToEven));
            g.Dispose();
            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                }
                else
                {
                    ob.Save(dFile, tFormat);
                }

                try
                {
                    if (IsDelect) File.Delete(sFile);
                }
                catch (Exception ex)
                {
               
                }
               
                return true;
            }
            catch(Exception ex)
            {
                var errorlog = new Errorlog(Directory.GetCurrentDirectory() + "\\error\\");
                errorlog.WriteError(ex);
                return false;
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
                ms.Dispose();
            }

        }

        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片</param>
        /// <param name="dFile">原图片</param>
        /// <param name="flag">压缩质量 1-100</param>
        /// <param name="IsDelect">是否删除原始照片</param>
        /// <returns></returns>
        public static bool CompressImage(string sFile, string dFile, int flag, bool IsDelect)
        {
            FileStream fstream = new FileStream(sFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

            byte[] bytes = new byte[fstream.Length];

            fstream.Read(bytes, 0, bytes.Length);

            fstream.Dispose();

            fstream.Close();

            MemoryStream ms = new MemoryStream(bytes);

            Bitmap iSource = (Bitmap)Bitmap.FromStream(ms, false, false);

            ImageFormat tFormat = iSource.RawFormat;

         
            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    iSource.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                }
                else
                {
                    iSource.Save(dFile, tFormat);
                }

                try
                {
                    if (IsDelect) File.Delete(sFile);
                }
                catch (Exception ex)
                {

                }

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();
                ms.Dispose();
            }

        }
        /// <summary>
        /// 将文本分行
        /// </summary>
        /// <param name="graphic">绘图图面</param>
        /// <param name="font">字体</param>
        /// <param name="text">文本</param>
        /// <param name="width">行宽</param>
        /// <returns></returns>
        public static List<string> GetStringRows(Graphics graphic, Font font, string text, int width)
        {
            int RowBeginIndex = 0;
            int rowEndIndex = 0;
            int textLength = text.Length;
            List<string> textRows = new List<string>();

            for (int index = 0; index < textLength; index++)
            {
                rowEndIndex = index;

                if (index == textLength - 1)
                {
                    textRows.Add(text.Substring(RowBeginIndex));
                }
                else if (rowEndIndex + 1 < text.Length && text.Substring(rowEndIndex, 2) == "rn")
                {
                    textRows.Add(text.Substring(RowBeginIndex, rowEndIndex - RowBeginIndex));
                    rowEndIndex = index += 2;
                    RowBeginIndex = rowEndIndex;
                }
                else if (graphic.MeasureString(text.Substring(RowBeginIndex, rowEndIndex - RowBeginIndex + 1), font).Width > width)
                {
                    textRows.Add(text.Substring(RowBeginIndex, rowEndIndex - RowBeginIndex));
                    RowBeginIndex = rowEndIndex;
                }
            }
            return textRows;
        }

        /// <summary>
        /// 快速读取图像分辨率大小
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static System.Drawing.Size GetImageSize(FileInfo file)
        {

            using (FileStream stm = file.OpenRead())
            {
                return GetImageSize(stm);
            }

        }

        /// <summary>
        /// 快速读取图像分辨率大小
        /// </summary>
        /// <param name="ImageStream"></param>
        /// <returns></returns>
        public static System.Drawing.Size GetImageSize(Stream ImageStream)
        {
            System.Drawing.Size imgSize;
            long position = ImageStream.Position;


            byte[] bin = new byte[2];
            ImageStream.Read(bin, 0, 2);
            ImageStream.Seek(-2, SeekOrigin.Current);

            int Fmt = bin[0] << 8 | bin[1];


            switch (Fmt)
            {
                case 0xFFd8://jpg
                    if (GetJpegSize(ImageStream, out imgSize))
                        return imgSize;
                    break;
                case 0x8950://png
                    if (GetPngSize(ImageStream, out imgSize))
                        return imgSize;
                    break;
                case 0x4749://gif
                    if (GetGifSize(ImageStream, out imgSize))
                        return imgSize;
                    break;
                case 0x424D://bmp
                    if (GetBmpSize(ImageStream, out imgSize))
                        return imgSize;
                    break;
            }


            ImageStream.Position = position;
            Image img = null;
            try
            {
                img = Image.FromStream(ImageStream);
                return img.Size;
            }
            catch (Exception)
            {
                return new System.Drawing.Size();
            }
            finally
            {
                if (img != null)
                    img.Dispose();
            }
        }


        /// <summary>
        /// 快速读取jpg图像分辨率大小
        /// </summary>
        /// <param name="JpegStream"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static bool GetJpegSize(Stream JpegStream, out System.Drawing.Size size)
        {
            size = new System.Drawing.Size();
            byte[] bin = new byte[2];
            JpegStream.Read(bin, 0, 2);

            if (bin[0] != 0xff && bin[1] != 0xd8)//SOI，Start of Image，图像开始
                return false;
            int flag;
            int DataLen = 0;
            while (JpegStream.CanRead)
            {
                int c = JpegStream.Read(bin, 0, 2);
                if (c != 2)//end of file;
                    break;
                if (bin[0] != 0xfF)//Error File!
                    break;
                flag = bin[1];

                if (flag == 0xD9 || flag == 0xDA)//图像结整或图像数据开始
                    break;
                switch (flag)
                {
                    case 0xC0://SOF0，Start of Frame，帧图像开始
                        JpegStream.Read(bin, 0, 2);
                        DataLen = bin[0] << 8 | bin[1];
                        DataLen -= 2;
                        byte[] data = new byte[DataLen];
                        JpegStream.Read(data, 0, DataLen);
                        DataLen = 0;
                        size.Height = data[1] << 8 | data[2];
                        size.Width = data[3] << 8 | data[4];
                        return true;//无需读取其它数据

                    //case 0xD9://EOI，End of Image，图像结束 2字节
                    //case 0xDA://Start of Scan，扫描开始 12字节 图像数据，通常，到文件结束,遇到EOI标记
                    case 0xC4://DHT，Difine Huffman Table，定义哈夫曼表
                    case 0xDD:// DRI，Define Restart Interval，定义差分编码累计复位的间隔
                    case 0xDB:// DQT，Define Quantization Table，定义量化表
                    case 0xE0://APP0，Application，应用程序保留标记0。版本，DPI等信息
                    case 0xE1://APPn，Application，应用程序保留标记n，其中n=1～15(任选)
                        JpegStream.Read(bin, 0, 2);
                        DataLen = bin[0] << 8 | bin[1];
                        DataLen -= 2;
                        break;
                    default:
                        if (flag > 0xE1 && flag < 0xEF)//APPx
                            goto case 0xE1;
                        //格式错误？？
                        break;
                }
                if (DataLen != 0)
                {
                    JpegStream.Seek(DataLen, System.IO.SeekOrigin.Current);
                    DataLen = 0;
                }
            }
            return !size.IsEmpty;
        }

        /// <summary>
        /// 快速读取png图像分辨率大小
        /// </summary>
        /// <param name="JpegStream"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static bool GetPngSize(Stream PngStm, out System.Drawing.Size size)
        {
            size = new System.Drawing.Size();
            const uint PNG_HEAD = 0x89504e47;
            const uint PNG_HEAD_2 = 0x0d0a1a0a;// PNG标识签名 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A;

            byte[] bin = new byte[64];
            int c = PngStm.Read(bin, 0, 8);
            if (c != 8)
                return false;

            if ((uint)Bytes2Int(bin, 0) != PNG_HEAD || (uint)Bytes2Int(bin, 4) != PNG_HEAD_2) //其它格式
                return false;

            while (PngStm.CanRead)
            {

                c = PngStm.Read(bin, 0, 8);
                if (c != 8)
                    return false;
                int dataLen = Bytes2Int(bin, 0) + 4;
                string Field = System.Text.ASCIIEncoding.ASCII.GetString(bin, 4, 4);
                switch (Field)
                {
                    case "IHDR"://文件头数据块 
                        c = PngStm.Read(bin, 0, dataLen);
                        if (c != dataLen)
                            return false;
                        size.Width = Bytes2Int(bin, 0);
                        size.Height = Bytes2Int(bin, 4);
                        dataLen = 0;
                        return true;
                    case "sBIT":
                    case "pHYs":
                    case "tEXt":
                    case "IDAT"://LZ77图片数据
                    case "IEND"://文件结尾
                    default:
                        break;
                }
                if (dataLen != 0)
                {
                    PngStm.Seek(dataLen, System.IO.SeekOrigin.Current);
                }

            }
            return !size.IsEmpty;
        }

        /// <summary>
        /// 快速读取gif图像分辨率大小
        /// </summary>
        /// <param name="PngStm"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static bool GetGifSize(Stream PngStm, out System.Drawing.Size size)
        {
            size = new System.Drawing.Size();


            byte[] bin = new byte[32];
            int c = PngStm.Read(bin, 0, 32);
            if (c != 32)
                return false;

            if (bin[0] != 'G' || bin[1] != 'I' || bin[2] != 'F') //其它格式
                return false;

            size.Width = bin[6] | bin[7] << 8;
            size.Height = bin[8] | bin[9] << 8;

            return !size.IsEmpty;
        }

        /// <summary>
        /// 快速读取bmp图像分辨率大小
        /// </summary>
        /// <param name="PngStm"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static bool GetBmpSize(Stream PngStm, out System.Drawing.Size size)
        {
            size = new System.Drawing.Size();

            byte[] bin = new byte[32];
            int c = PngStm.Read(bin, 0, 32);
            if (c != 32)
                return false;

            if (bin[0] != 'B' || bin[1] != 'M') //其它格式
                return false;

            size.Width = bin[18] | bin[19] << 8 | bin[20] << 16 | bin[21] << 24;
            size.Height = bin[22] | bin[23] << 8 | bin[24] << 16 | bin[25] << 24;

            return !size.IsEmpty;
        }


        static int Bytes2Int(byte[] bin, int offset)
        {
            return bin[offset + 0] << 24 | bin[offset + 1] << 16 | bin[offset + 2] << 8 | bin[offset + 3];
        }
    }



}
