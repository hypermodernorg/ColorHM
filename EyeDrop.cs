using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing;

using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ColorHM
{
    class EyeDrop
    {
        public void TakeScreenShot()
        {

            MainWindow newRec = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            //newRec.WindowState = System.Windows.WindowState.Minimized;
            newRec.Hide();
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Size screenSize = new Size();

            Random r = new Random();
            int rInt = r.Next(0, 100); //for ints
            string screenshotFile = "screenshot" + rInt.ToString() + ".bmp";

            using (Graphics g = Graphics.FromImage(bitmap))
            {

                g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
                bitmap.Save(screenshotFile);  // saves the image
                screenSize = Screen.PrimaryScreen.Bounds.Size;
                g.Dispose();
                
            }
            bitmap.Dispose();

            Screenshot screenshot = new Screenshot();
            screenshot.screenshotImage.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + screenshotFile, UriKind.Absolute));
            
            screenshot.Show();
            

        }
    }

}
    