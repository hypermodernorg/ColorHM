using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SQLite;

namespace ColorHM
{
    public partial class MainWindow
    {
        //public System.Windows.Media.Color MyBrush { get; set; }
        private void HSL_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            int h = (int)hueSlider.Value;
            double s = (double)saturationSlider.Value;
            double l = lightnesSlider.Value;
            int a = (int)alphaSlider.Value;
            Color argbColor = new Color();
            //! Prevent the recursion error between rgba sliders and hsl sliders
            if (hueSlider.IsFocused == true || saturationSlider.IsFocused == true || lightnesSlider.IsFocused == true)
            {
                hueTextBox.Text = h.ToString();
                saturationTextBox.Text = s.ToString();
                lightnessTextBox.Text = l.ToString();


                RGBandHSL.HlsToRgb((double)h, l, s, out int r, out int g, out int b);
                redSlider.Value = r; redTextBox.Text = r.ToString();
                greenSlider.Value = g; greenTextBox.Text = g.ToString();
                blueSlider.Value = b; blueTextBox.Text = b.ToString();

                argbColor = Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
                var x = new SolidColorBrush(argbColor);
                TopRectangle.Fill = x;
                hexTextBox.Text = x.ToString();
                //MyBrush = argbColor;

            }


            s = 1;
            RGBandHSL.HlsToRgb((double)h, l, s, out int rb, out int gb, out int bb);
            argbColor = Color.FromArgb((byte)a, (byte)rb, (byte)gb, (byte)bb);
            SaturationRectangleGradientstop.GradientStops[1].Color = argbColor;

            s = 0;
            RGBandHSL.HlsToRgb((double)h, l, s, out int rc, out int gc, out int bc);
            argbColor = Color.FromArgb((byte)a, (byte)rc, (byte)gc, (byte)bc);
            SaturationRectangleGradientstop.GradientStops[0].Color = argbColor;

            s = (double)saturationSlider.Value;
            l = .5;
            RGBandHSL.HlsToRgb((double)h, l, s, out int rd, out int gd, out int bd);
            argbColor = Color.FromArgb((byte)a, (byte)rd, (byte)gd, (byte)bd);
            LightnessRectangleGradientstop.GradientStops[2].Color = argbColor;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            //! Prevent the recursion error between rgba sliders and hsl sliders
            if (redSlider.IsFocused == true || greenSlider.IsFocused == true || blueSlider.IsFocused == true || alphaSlider.IsFocused == true)
            {
                int cR = (int)redSlider.Value; redTextBox.Text = cR.ToString();
                int cG = (int)greenSlider.Value; greenTextBox.Text = cG.ToString();
                int cB = (int)blueSlider.Value; blueTextBox.Text = cB.ToString();
                int cA = (int)alphaSlider.Value; alphaTextBox.Text = cA.ToString();

                if (hueSlider.IsFocused == false || saturationSlider.IsFocused == false || lightnesSlider.IsFocused == false)
                {
                    RGBandHSL.RgbToHls(cR, cG, cB, out double h, out double l, out double s);
                    hueTextBox.Text = h.ToString(); hueSlider.Value = h;
                    saturationTextBox.Text = s.ToString(); saturationSlider.Value = s;
                    lightnessTextBox.Text = l.ToString(); lightnesSlider.Value = l;
                }


                Brush x = TopRectangle.Fill;

                Brush r = redSlider.Foreground;
                string hexValueR = cR.ToString("X2");
                Color rC = (Color)ColorConverter.ConvertFromString(r.ToString());
                var rHex = "#" + hexValueR + rC.R.ToString("X2") + rC.G.ToString("X2") + rC.B.ToString("X2");
                Color rColor = (Color)ColorConverter.ConvertFromString(rHex);
                redSlider.Foreground = new SolidColorBrush(rColor);

                Brush g = greenSlider.Foreground;
                string hexValueG = cG.ToString("X2");
                Color gC = (Color)ColorConverter.ConvertFromString(g.ToString());
                var gHex = "#" + hexValueG + gC.R.ToString("X2") + gC.G.ToString("X2") + gC.B.ToString("X2");
                Color gColor = (Color)ColorConverter.ConvertFromString(gHex);
                greenSlider.Foreground = new SolidColorBrush(gColor);

                Brush b = blueSlider.Foreground;
                string hexValueB = cB.ToString("X2");
                Color bC = (Color)ColorConverter.ConvertFromString(b.ToString());
                var bHex = "#" + hexValueB + bC.R.ToString("X2") + bC.G.ToString("X2") + bC.B.ToString("X2");
                Color bColor = (Color)ColorConverter.ConvertFromString(bHex);
                blueSlider.Foreground = new SolidColorBrush(bColor);

                Brush a = alphaSlider.Foreground;
                string hexValueA = cA.ToString("X2");
                Color aC = (Color)ColorConverter.ConvertFromString(a.ToString());
                var aHex = "#" + hexValueA + aC.R.ToString("X2") + aC.G.ToString("X2") + aC.B.ToString("X2");
                Color aColor = (Color)ColorConverter.ConvertFromString(aHex);
                alphaSlider.Foreground = new SolidColorBrush(aColor);

                Color argbColor = Color.FromArgb((byte)cA, (byte)cR, (byte)cG, (byte)cB);
                x = new SolidColorBrush(argbColor);
                TopRectangle.Fill = x;
                hexTextBox.Text = x.ToString();

            }
        }
        private void HexTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = hexTextBox.Text;
            if (hexTextBox.IsFocused == true)
            {


                if (Regex.Match(text, @"^((0x){0,1}|#{0,1})([0-9A-F]{8}|[0-9A-F]{6})$").Success)
                {
                    Color color = (Color)ColorConverter.ConvertFromString(text);
                    RectangleChange(color);
                }
            }
        }
    }
}
