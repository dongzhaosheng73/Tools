using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace DTTOOLS.Print
{
    /// <summary>
    /// 
    /// </summary>
    public class DTPrint : IDTPrint
    {
        public delegate void PrintDelegate(object image);

        public event PrintDelegate event_printend;
        public System.Drawing.Printing.PrintDocument DocumentPrint { set; get; }

        public  struct PDPI
        {
            public int X;
            public int Y;
        }
        private PDPI _DPI;
        public PDPI DPI
        {
            get
            {

                _DPI.X = DocumentPrint.DefaultPageSettings.PrinterResolution.X;
                _DPI.Y = DocumentPrint.DefaultPageSettings.PrinterResolution.Y;

                return _DPI;
            }
        }

        private bool _IsTp;

        public bool IsTp
        {
            set { _IsTp = value; }
            get { return _IsTp; }
        }
        private bool _ImageCut;
        /// <summary>
        /// 是否裁切
        /// </summary>
        public bool ImageCut
        {
            set { _ImageCut = value; }
            get { return _ImageCut; }
        }
        private Bitmap PrintBitmap { set; get; }
        /// <summary>
        /// 初始化
        /// </summary>
        public DTPrint()
        {
            DocumentPrint = new System.Drawing.Printing.PrintDocument();
            
            DocumentPrint.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(Pd_BeginPrint);
            DocumentPrint.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(Pd_PrintPage);        
        }
       /// <summary>
       /// 初始化
       /// </summary>
       /// <param name="defaultPrinter">默认打印机</param>
        public DTPrint(string defaultPrinter)
        {
            DocumentPrint = new System.Drawing.Printing.PrintDocument();
            DocumentPrint.PrinterSettings.PrinterName = defaultPrinter;
            DocumentPrint.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(Pd_BeginPrint);
            DocumentPrint.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(Pd_PrintPage);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="defaultPrinter">默认打印机</param>
        /// <param name="Landscape">是否横向打印</param>
        /// <param name="margin">打印边距</param>
        public DTPrint(string defaultPrinter, bool Landscape, System.Drawing.Printing.Margins margin)
        {
            DocumentPrint = new System.Drawing.Printing.PrintDocument();
            DocumentPrint.PrinterSettings.PrinterName = defaultPrinter;
            DocumentPrint.PrinterSettings.DefaultPageSettings.Margins = margin;
            DocumentPrint.DefaultPageSettings.Landscape = Landscape;
            DocumentPrint.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(Pd_BeginPrint);
            DocumentPrint.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(Pd_PrintPage);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="defaultPrinter">默认打印机</param>
        /// <param name="page">纸张大小</param>
        /// <param name="Landscape">纸张横竖</param>
        /// <param name="margin">打印边距</param>
        public DTPrint(string defaultPrinter, string page, bool Landscape,System.Drawing.Printing.Margins margin)
        {
            DocumentPrint = new System.Drawing.Printing.PrintDocument {PrinterSettings = {PrinterName = defaultPrinter}};
            DocumentPrint.PrintController = new StandardPrintController();
            var papers = new List<System.Drawing.Printing.PaperSize>(DTTOOLS.Print.PrintTools.GetPrintPageType(DocumentPrint));
            DocumentPrint.DefaultPageSettings.PaperSize = papers.Find(x => x.PaperName == page);
            DocumentPrint.DefaultPageSettings.Margins = margin;
            DocumentPrint.DefaultPageSettings.Landscape = Landscape;         
            DocumentPrint.PrintPage += Pd_BeginPrint;
            DocumentPrint.PrintPage += Pd_PrintPage;
        }
        /// <summary>
        /// 打印
        /// </summary>
        public void Print(Bitmap image, bool isCut)
        {
            if (image == null)
                return;
            ImageCut = isCut;
            PrintBitmap = image;
            DocumentPrint.Print();
            DocumentPrint.Dispose();
            GC.Collect();
        }
      
        /// <summary>
        /// 打印
        /// </summary>
        public void Print(Bitmap image, bool isCut,bool tp)
        {
            if (image == null)
                return;
            ImageCut = isCut;
            IsTp = tp;
            PrintBitmap = image;
            DocumentPrint.Print();
            DocumentPrint.Dispose();
            GC.Collect();
        }
       
        /// <summary>
        /// 打印预览
        /// </summary>
        public void PrintView(Bitmap image, bool isCut)
        {
            if (image == null)
                return;
            ImageCut = isCut;
            PrintBitmap = image;
            var pv = new PrintPreviewDialog {Document = DocumentPrint};
            pv.ShowDialog();
            pv.Dispose();
            GC.Collect();
        }
        /// <summary>
        /// 打印预览
        /// </summary>
        public void PrintView(Bitmap image, bool isCut,bool tp)
        {
            if (image == null)
                return;
            ImageCut = isCut;
            IsTp = tp;
            PrintBitmap = image;
            var pv = new PrintPreviewDialog { Document = DocumentPrint };
            pv.ShowDialog();
            pv.Dispose();
            GC.Collect();
        }
        /// <summary>
        /// 打印前事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pd_BeginPrint(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {   
          
        }
        /// <summary>
        /// 打印事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pd_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) 
        {

            try
            {
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                e.Graphics.InterpolationMode = InterpolationMode.High;
                e.PageSettings.PrinterResolution.X = DPI.X;
                e.PageSettings.PrinterResolution.Y = DPI.Y;

                Bitmap TBitmap;

                if (IsTp)
                {
                    var Tbh = (PrintBitmap.Height*(e.PageBounds.Width*DPI.X/100))/PrintBitmap.Width;
                    TBitmap = new Bitmap(e.PageBounds.Width * DPI.X / 100, Tbh); 
                }
                else
                {
                    TBitmap = new Bitmap(e.PageBounds.Width * DPI.X / 100, e.PageBounds.Size.Height * DPI.Y / 100); 
                }

                
                if (ImageCut == false)
                {                  
                    TBitmap.SetResolution(DPI.X, DPI.Y);
                    GDITools.Picsize = GDITools.FitSize(PrintBitmap.Width, PrintBitmap.Height, TBitmap.Width, TBitmap.Height);
                    var tempXY = GDITools.PointXY(GDITools.Picsize, TBitmap.Width, TBitmap.Height);
                  
                    var g = Graphics.FromImage(TBitmap);

                    g.DrawImage(PrintBitmap,
                        new Rectangle(tempXY.X, tempXY.Y, (int) GDITools.Picsize.fitw, (int) GDITools.Picsize.fith));
                        
                    g.Dispose();
                    e.Graphics.Clear(Color.White);
                    e.HasMorePages = false;
                    e.Graphics.DrawImage(TBitmap, 0, 0);
                    if (event_printend != null) event_printend(TBitmap.Clone());
                    PrintBitmap.Dispose();
                    TBitmap.Dispose();
                }
                else
                {
                   
           
                    TBitmap.SetResolution(DPI.X, DPI.Y);
                    GDITools.Picsize = GDITools.FitSizeOutSide(PrintBitmap.Width, PrintBitmap.Height, TBitmap.Width, TBitmap.Height);
                    var TempXY = GDITools.PointXY(GDITools.Picsize, TBitmap.Width, TBitmap.Height);
                   
                    Graphics g = Graphics.FromImage(TBitmap);
                    g.DrawImage(PrintBitmap,
                        new Rectangle(TempXY.X, TempXY.Y, (int) GDITools.Picsize.fitw, (int) GDITools.Picsize.fith));
                          
                    g.Dispose();
                    e.Graphics.Clear(Color.White);
                    e.HasMorePages = false;
                    e.Graphics.DrawImage(TBitmap, 0, 0);
                    if (event_printend != null) event_printend(TBitmap.Clone());
                    PrintBitmap.Dispose();
                    TBitmap.Dispose();
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }      
        #region Dispose

        protected bool isDisplsed { get; set; }

        ~DTPrint()
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

            if(DocumentPrint!=null) DocumentPrint.Dispose();

            if (PrintBitmap != null) PrintBitmap.Dispose();

            GDITools.FlushMemory();

            isDisplsed = true;
        }

        #endregion
    }
}
