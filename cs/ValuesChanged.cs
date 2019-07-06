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
        //Todo There has to be a better way to coordinate all of these controls and values.
        // Update all of the control color related fields with the color passed to this method, then update the top rectangle.

        public void RectangleChangeControl(Color color)
        {
            Brush brush = new SolidColorBrush(color);
            int i = 0;
            if (hueSlider.IsFocused == true || saturationSlider.IsFocused == true || lightnesSlider.IsFocused == true)
            {
                HSLChangeText(color);
                RGBChangeSlider(color);
                RGBChangeText(color);
                HEXChangeText(color);
                i++;
            }

            if (hueTextBox.IsFocused == true || saturationTextBox.IsFocused == true || lightnessTextBox.IsFocused == true)
            {
                HSLChangeSlider(color);
                RGBChangeSlider(color);
                RGBChangeText(color);
                HEXChangeText(color);
                i++;
            }

            if (redSlider.IsFocused == true || greenSlider.IsFocused == true || blueSlider.IsFocused == true || alphaSlider.IsFocused == true)
            {
                HSLChangeText(color);
                HSLChangeSlider(color);
                RGBChangeText(color);
                HEXChangeText(color);
                i++;
            }

            if (redTextBox.IsFocused == true || greenTextBox.IsFocused == true || blueTextBox.IsFocused == true || alphaTextBox.IsFocused == true)
            {
                HSLChangeText(color);
                RGBChangeSlider(color);
                HSLChangeSlider(color);
                HEXChangeText(color);
                i++;
            }

            if (hexTextBox.IsFocused == true)
            {
                HSLChangeText(color);
                RGBChangeSlider(color);
                HSLChangeSlider(color);
                RGBChangeText(color);
                i++;
            }

            if (color.ToString() == "came from palette click")
            {
                HSLChangeText(color);
                RGBChangeSlider(color);
                HSLChangeSlider(color);
                HEXChangeText(color);
                RGBChangeText(color);
                i++;
            }

            if (color.ToString() == "came from eyedrop")
            {
                HSLChangeText(color);
                RGBChangeSlider(color);
                HSLChangeSlider(color);
                HEXChangeText(color);
                RGBChangeText(color);
                i++;
            }

            // if nothing else matches, the program most likely just started.
            if (i==0)
            {
                HSLChangeText(color);
                RGBChangeSlider(color);
                HSLChangeSlider(color);
                HEXChangeText(color);
                RGBChangeText(color);
            }
            TopRectangle.Fill = brush;
        }

        private void HSLChangeText(Color color)
        {
            RGBandHSL.RgbToHls(color.R, color.G, color.B, out double h, out double l, out double s);
            hueTextBox.Text = (h*100).ToString(); 
            saturationTextBox.Text = (s*100).ToString(); 
            lightnessTextBox.Text = (l*100).ToString(); 

        }

        private void HSLChangeSlider(Color color)
        {
            RGBandHSL.RgbToHls(color.R, color.G, color.B, out double h, out double l, out double s);
            hueSlider.Value = h;
            saturationSlider.Value = s;
            lightnesSlider.Value = l;
        }
        private void RGBChangeText(Color color)
        {
            redTextBox.Text= color.R.ToString();
            greenTextBox.Text = color.G.ToString();
            blueTextBox.Text = color.B.ToString();
            alphaTextBox.Text = color.A.ToString();
        }
        private void RGBChangeSlider(Color color)
        {
            redSlider.Value = color.R;
            greenSlider.Value = color.G;
            blueSlider.Value = color.B;
            alphaSlider.Value = color.A;
        }

        private void HEXChangeText(Color color)
        {
            hexTextBox.Text = color.ToString();
        }





        //! Old method of values change below this comment
        //public void RectangleChange(Color color)
        //{
        //    Brush brush = new SolidColorBrush(color);

        //    // update rgb sliders and textboxes
        //    redSlider.Value = (int)color.R; redTextBox.Text = color.R.ToString();
        //    greenSlider.Value = (int)color.G; greenTextBox.Text = color.G.ToString();
        //    blueSlider.Value = (int)color.B; blueTextBox.Text = color.B.ToString();
        //    alphaSlider.Value = (int)color.A; alphaTextBox.Text = color.A.ToString();

        //    // convert rgb to hsl
        //    RGBandHSL.RgbToHls((int)color.R, (int)color.G, (int)color.B, out double h, out double l, out double s);

        //    // update hsl sliders and textboxes
        //    hueTextBox.Text = h.ToString(); hueSlider.Value = h;
        //    saturationTextBox.Text = s.ToString(); saturationSlider.Value = s;
        //    lightnessTextBox.Text = l.ToString(); lightnesSlider.Value = l;

        //    // update the hexttextbox
        //    hexTextBox.Text = brush.ToString();

        //    // fill the top rectangle with the color passed to this method.
        //    TopRectangle.Fill = brush;
        //}

        // If the hsl slider's change update the color controls and top rectangle respectively.
        private void HSL_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Color color = new Color();
            int h = (int)hueSlider.Value;
            double s = (double)saturationSlider.Value;
            double l = lightnesSlider.Value;
            // Get the alpha slider value for compatability with Color.FromArgb
            int a = (int)alphaSlider.Value;
            if (hueSlider.IsFocused == true || saturationSlider.IsFocused == true || lightnesSlider.IsFocused == true)
            {



                RGBandHSL.HlsToRgb((double)h, l, s, out int r, out int g, out int b);

                color.R = (byte)r;
                color.G = (byte)g;
                color.B = (byte)b;
                color.A = (byte)a;
        

                RectangleChangeControl(color);
            }
                // Get the hsl slider values

            //! Prevent the recursion error between rgba sliders and hsl sliders
            //if (hueSlider.IsFocused == true || saturationSlider.IsFocused == true || lightnesSlider.IsFocused == true)
            //{
            //    hueTextBox.Text = h.ToString();
            //    saturationTextBox.Text = s.ToString();
            //    lightnessTextBox.Text = l.ToString();

            //    RGBandHSL.HlsToRgb((double)h, l, s, out int r, out int g, out int b);
            //    redSlider.Value = r; redTextBox.Text = r.ToString();
            //    greenSlider.Value = g; greenTextBox.Text = g.ToString();
            //    blueSlider.Value = b; blueTextBox.Text = b.ToString();

            //    argbColor = Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
            //    var x = new SolidColorBrush(argbColor);
            //    TopRectangle.Fill = x;
            //    hexTextBox.Text = x.ToString();

            //}

            // get the rightmost saturation slider gradientstop
            s = 1;
            RGBandHSL.HlsToRgb((double)h, l, s, out int rb, out int gb, out int bb);
            color = Color.FromArgb((byte)a, (byte)rb, (byte)gb, (byte)bb);
            SaturationRectangleGradientstop.GradientStops[1].Color = color;

            // get the leftmost saturation slider gradientstop
            s = 0;
            RGBandHSL.HlsToRgb((double)h, l, s, out int rc, out int gc, out int bc);
            color = Color.FromArgb((byte)a, (byte)rc, (byte)gc, (byte)bc);
            SaturationRectangleGradientstop.GradientStops[0].Color = color;

            // get the middle lightness slider gradientstop, the first and last stop are black and white.
            // the middle gradient should be the values of saturation and hue, where lightness is held at .5.
            s = (double)saturationSlider.Value;
            l = .5;
            RGBandHSL.HlsToRgb((double)h, l, s, out int rd, out int gd, out int bd);
            color = Color.FromArgb((byte)a, (byte)rd, (byte)gd, (byte)bd);
            LightnessRectangleGradientstop.GradientStops[2].Color = color;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            //! Prevent the recursion error between rgba sliders and hsl sliders
           // if (redSlider.IsFocused == true || greenSlider.IsFocused == true || blueSlider.IsFocused == true || alphaSlider.IsFocused == true)
           // {
                int cR = (int)redSlider.Value; redTextBox.Text = cR.ToString();
                int cG = (int)greenSlider.Value; greenTextBox.Text = cG.ToString();
                int cB = (int)blueSlider.Value; blueTextBox.Text = cB.ToString();
                int cA = (int)alphaSlider.Value; alphaTextBox.Text = cA.ToString();
                Color argbcolor = Color.FromArgb((byte)cA, (byte)cR, (byte)cG, (byte)cB);
                RectangleChangeControl(argbcolor);

            //if (hueSlider.IsFocused == false || saturationSlider.IsFocused == false || lightnesSlider.IsFocused == false)
            //{
            //    RGBandHSL.RgbToHls(cR, cG, cB, out double h, out double l, out double s);
            //    hueTextBox.Text = h.ToString(); hueSlider.Value = h;
            //    saturationTextBox.Text = s.ToString(); saturationSlider.Value = s;
            //    lightnessTextBox.Text = l.ToString(); lightnesSlider.Value = l;
            //}


            //Brush x = TopRectangle.Fill;

            //Brush r = redSlider.Foreground;
            //    string hexValueR = cR.ToString("X2");
            //    Color rC = (Color)ColorConverter.ConvertFromString(r.ToString());
            //    var rHex = "#" + hexValueR + rC.R.ToString("X2") + rC.G.ToString("X2") + rC.B.ToString("X2");
            //    Color rColor = (Color)ColorConverter.ConvertFromString(rHex);
            //    redSlider.Foreground = new SolidColorBrush(rColor);

            //    Brush g = greenSlider.Foreground;
            //    string hexValueG = cG.ToString("X2");
            //    Color gC = (Color)ColorConverter.ConvertFromString(g.ToString());
            //    var gHex = "#" + hexValueG + gC.R.ToString("X2") + gC.G.ToString("X2") + gC.B.ToString("X2");
            //    Color gColor = (Color)ColorConverter.ConvertFromString(gHex);
            //    greenSlider.Foreground = new SolidColorBrush(gColor);

            //    Brush b = blueSlider.Foreground;
            //    string hexValueB = cB.ToString("X2");
            //    Color bC = (Color)ColorConverter.ConvertFromString(b.ToString());
            //    var bHex = "#" + hexValueB + bC.R.ToString("X2") + bC.G.ToString("X2") + bC.B.ToString("X2");
            //    Color bColor = (Color)ColorConverter.ConvertFromString(bHex);
            //    blueSlider.Foreground = new SolidColorBrush(bColor);

            //    Brush a = alphaSlider.Foreground;
            //    string hexValueA = cA.ToString("X2");
            //    Color aC = (Color)ColorConverter.ConvertFromString(a.ToString());
            //    var aHex = "#" + hexValueA + aC.R.ToString("X2") + aC.G.ToString("X2") + aC.B.ToString("X2");
            //    Color aColor = (Color)ColorConverter.ConvertFromString(aHex);
            //    alphaSlider.Foreground = new SolidColorBrush(aColor);

        
                // x = new SolidColorBrush(argbColor);
                //TopRectangle.Fill = x;
                //hexTextBox.Text = x.ToString();

           // }
        }
        private void HexTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = hexTextBox.Text;
            if (hexTextBox.IsFocused == true)
            {


                if (Regex.Match(text, @"^((0x){0,1}|#{0,1})([0-9A-F]{8}|[0-9A-F]{6})$").Success)
                {
                    Color color = (Color)ColorConverter.ConvertFromString(text);
                    //RectangleChange(color);
                    RectangleChangeControl(color);
                }
            }
        }
    }
}
