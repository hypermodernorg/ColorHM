using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ColorHM
{
    public partial class MainWindow
    {
        private void CopyHex(object sender, RoutedEventArgs e)
        {
            MenuItem x = sender as MenuItem;
            ContextMenu y = x.Parent as ContextMenu;
            Rectangle rec = y.PlacementTarget as Rectangle;
            SolidColorBrush brush = rec.Fill as SolidColorBrush;
            Color color = brush.Color;

            var hexString = color.ToString().Remove(1, 2);
            Clipboard.SetText(hexString);
        }
        private void CopyHexA(object sender, RoutedEventArgs e)
        {
            MenuItem x = sender as MenuItem;
            ContextMenu y = x.Parent as ContextMenu;
            Rectangle rec = y.PlacementTarget as Rectangle;
            SolidColorBrush brush = rec.Fill as SolidColorBrush;
            Color color = brush.Color;

            Clipboard.SetText(hexTextBox.Text);
        }
        private void CopyRGB(object sender, RoutedEventArgs e)
        {
            MenuItem x = sender as MenuItem;
            ContextMenu y = x.Parent as ContextMenu;
            Rectangle rec = y.PlacementTarget as Rectangle;
            SolidColorBrush brush = rec.Fill as SolidColorBrush;
            Color color = brush.Color;

            Clipboard.SetText("rgb (" + color.R.ToString() + ", " + color.G.ToString() + ", " + color.B.ToString() + ")");
        }
        private void CopyHSL(object sender, RoutedEventArgs e)
        {
            MenuItem x = sender as MenuItem;
            ContextMenu y = x.Parent as ContextMenu;
            Rectangle rec = y.PlacementTarget as Rectangle;
            SolidColorBrush brush = rec.Fill as SolidColorBrush;
            Color color = brush.Color;

            RGBandHSL.RgbToHls((int)color.R, (int)color.G, (int)color.B, out double h, out double l, out double s);
            Clipboard.SetText("hsl (" + Math.Round(h).ToString() + ", " + "%" + (Convert.ToInt32(s * 100)).ToString() + ", " + "%" + (Convert.ToInt32(l * 100)).ToString() + ")");
        }
    }
}
