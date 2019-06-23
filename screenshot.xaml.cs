using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace ColorHM
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Screenshot : Window
    {
        public Screenshot()
        {
            InitializeComponent();
        }

        private void ScreenshotImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dynamic x = sender;

            var xy = System.Windows.Forms.Cursor.Position;
            var hdc = GetWindowDC(0);
            var intColor = GetPixel(hdc, xy.X, xy.Y);
            byte b = (byte)((intColor >> 0x10) & 0xffL);
            byte ga = (byte)((intColor >> 8) & 0xffL);
            byte r = (byte)(intColor & 0xffL);
            System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb(r, ga, b);
            //System.Windows.MessageBox.Show(color.ToString());
       
         

            MainWindow newRec = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            newRec.Show();
            newRec.RectangleChange(color);
            x.Parent.Parent.Close();
            
            //var mainWindow = MainWindow.

        }



        [DllImport("gdi32")]
        private static extern int GetPixel(int hdc, int nXPos, int nYPos);
        [DllImport("user32")]
        private static extern int GetWindowDC(int hwnd);

        private void ScreenshotImage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            double factor = System.Windows.PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11;

            dynamic x = sender;
            
            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
   
   
            var xy = System.Windows.Forms.Cursor.Position;
 
            var hdc = GetWindowDC(0);
            var intColor = GetPixel(hdc, xy.X, xy.Y);
            byte b = (byte)((intColor >> 0x10) & 0xffL);
            byte ga = (byte)((intColor >> 8) & 0xffL);
            byte r = (byte)(intColor & 0xffL);
            System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb(r, ga, b);
            SolidColorBrush brush = new SolidColorBrush(color);
            shRec.Width = 60;
            shRec.Height = 60;
            shRec.Fill = brush;
            Thickness position = new Thickness();
            position.Top = (int)factor * (int)xy.Y +5 ;
            position.Left = (int)factor * (int)xy.X +5;

            shRec.Margin = position;

           
           

        }
    }
}
