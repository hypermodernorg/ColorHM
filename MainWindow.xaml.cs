using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            CleanScreenshots();
            alphaSlider.Value = 255;
            GetPalettes();
            Color c = new Color();
            c.R = 82;
            c.G = 254;
            c.B = 129;

            RectangleChange(c);


        }

        public void CleanScreenshots()
        {
            string screenShotDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\screenshots";
            Directory.CreateDirectory(screenShotDirectory);
            string[] filePaths = Directory.GetFiles(screenShotDirectory);
            foreach (string filePath in filePaths)
            {
                File.Delete(filePath);
            }
        }

        //System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public void RectangleChange(Color color)
        {

            Brush brush = new SolidColorBrush(color);
            //TopRectangle.Fill = brush;
            redSlider.Value = (int)color.R; redTextBox.Text = color.R.ToString();
            greenSlider.Value = (int)color.G; greenTextBox.Text = color.G.ToString();
            blueSlider.Value = (int)color.B; blueTextBox.Text = color.B.ToString();
            RgbToHls((int)color.R, (int)color.G, (int)color.B, out double h, out double l, out double s);
            hueTextBox.Text = h.ToString(); hueSlider.Value = h;
            saturationTextBox.Text = s.ToString(); saturationSlider.Value = s;
            lightnessTextBox.Text = l.ToString(); lightnesSlider.Value = l;

        }


        // Gets the wrap pannel of the selected TabControl TabItem
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject rootObject) where T : DependencyObject
        {
            if (rootObject != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(rootObject); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(rootObject, i);

                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }

        public SQLiteConnection Connect()
        {
            SQLiteConnection conn;
            string dbDirectory = AppDomain.CurrentDomain.BaseDirectory + "db\\colors.db";
            conn = new SQLiteConnection("Data Source=" + dbDirectory + "; Version=3;New=True;Compress=True;");
            conn.Open();
            return conn;

        }

        // The tab with "+" in the header to create a new palette.
        public void NewPalette()
        {
            string paletteName = "+";
            var ti = new TabItem();

            Label label = new Label();
            label.Content = paletteName;
            label.MouseLeftButtonDown += Label_MouseLeftButtonDown;
            ti.Header = label;
            TabControl1.Items.Add(ti);
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dynamic label = sender;
            dynamic tabItem = label.Parent;
            var wcp = new WrapPanel();

            Thickness thickness = new Thickness
            {
                Bottom = 0,
                Top = 0,
                Left = 0,
                Right = 0
            };

            TextBox newPaletteTextBox = new TextBox
            {
                BorderThickness = thickness,

                Background = Brushes.Transparent,
                Text = "Rename"
            };

            tabItem.Header = newPaletteTextBox;
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            contextMenu.Items.Add(menuItem);
            menuItem.Header = "Save Palette";
            menuItem.Click += new RoutedEventHandler(SavePalette);
            tabItem.ContextMenu = contextMenu;
            tabItem.Content = wcp;
            NewPalette();
        }



        // Get all palettes, create new if none exist.
        public void GetPalettes()
        {
            SQLiteConnection conn = Connect();
            SQLiteCommand sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT id, palette_name, colors from palettes";
            SQLiteDataAdapter dt = new SQLiteDataAdapter(sqlite_cmd);
            DataTable palettesDT = new DataTable();
            dt.Fill(palettesDT);

            // If there are no palettes, create one and rerun the query.
            if (palettesDT.Rows.Count == 0)
            {
                sqlite_cmd.CommandText = "INSERT INTO palettes (palette_name) VALUES ('Palette 1')";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "SELECT id, palette_name, colors from palettes";
                dt = new SQLiteDataAdapter(sqlite_cmd);
                palettesDT = new DataTable();
                dt.Fill(palettesDT);
            }

            foreach (DataRow row in palettesDT.Rows)
            {
                var ti = new TabItem();
                var wcp = new WrapPanel();
                ContextMenu contextMenu = new ContextMenu();
                MenuItem menuItem = new MenuItem();
                contextMenu.Items.Add(menuItem);
                menuItem.Header = "Save Palette";
                menuItem.Click += new RoutedEventHandler(SavePalette);
                //ti.Content = wcp;

                Thickness thickness = new Thickness
                {
                    Bottom = 0,
                    Top = 0,
                    Left = 0,
                    Right = 0
                };
                TextBox newPaletteTextBox = new TextBox
                {
                    BorderThickness = thickness,

                    Background = Brushes.Transparent,
                    Text = row["palette_name"].ToString(),

                };

                string colors = row["colors"].ToString();
                List<string> colorList = colors.Split(null).ToList();

                // Create a rectangle for each color per palette
                foreach (string color in colorList)
                {

                    // make sure color is in fact a color before processing 
                    // because whitespace may exist at the end of the color string in the db
                    if (color.Contains('#'))
                    {
                        //MessageBox.Show(color);
                        Color newColor = (Color)ColorConverter.ConvertFromString(color);


                        Brush newBrush = new SolidColorBrush(newColor);
                        var rec = new ColorHM.Properties.UserControl1();
                        rec.rectangleUC.Fill = newBrush;
                        //rec.MouseDown += new RoutedEventArgs(RecMouseDown);
                        wcp.Children.Add(rec);
                    }

                }
                ti.Content = wcp;
                // MessageBox.Show(colors);
                ti.Header = newPaletteTextBox;
                ti.ContextMenu = contextMenu;
                ti.Tag = row["id"];
                TabControl1.Items.Add(ti);
            }
            conn.Close();
            NewPalette();
        }


        public void SavePalette(object sender, RoutedEventArgs e)
        {

            dynamic selectedTab = TabControl1.SelectedContent;
            dynamic selectedTabHeader = TabControl1.SelectedItem;
            string selectedTabHeaderText = selectedTabHeader.Header.Text.ToString();


            dynamic wrapChildren = selectedTab.Children;
            StringBuilder paletteColors = new StringBuilder();
            foreach (dynamic rec in wrapChildren)
            {
                dynamic recColor = rec.rectangleUC.Fill;
                string colorListString = recColor.ToString();
                paletteColors.Append(recColor + " ");
            }
            //MessageBox.Show(paletteColors.ToString());
            SQLiteConnection conn = Connect();
            SQLiteCommand sqlite_cmd = conn.CreateCommand();
            SQLiteCommand sqlite_cmdB = conn.CreateCommand();


            // If there is no tag in the tabitem.
            // Since when a new palette is created, the palette is not yet assigned a tag, give it a tag.
            // The tag holds the database ID of the palette.
            if (selectedTab.Parent.Tag == null)
            {

                sqlite_cmd.CommandText = $"INSERT INTO palettes (palette_name, colors) VALUES ('{selectedTabHeaderText}', '{paletteColors.ToString()}') ";
                sqlite_cmd.ExecuteNonQuery();

                sqlite_cmdB.CommandText = "SELECT last_insert_rowid()";
                dynamic dyn = sqlite_cmdB.ExecuteScalar();

                selectedTab.Parent.Tag = dyn.ToString();

            }

            // If there is a tag in the tabitem.
            else
            {
                sqlite_cmd.CommandText = $"UPDATE palettes SET palette_name = '{selectedTabHeaderText}', colors = '{paletteColors.ToString()}' WHERE id = {selectedTab.Parent.Tag}";
                sqlite_cmd.ExecuteNonQuery();
            }

            conn.Close();

        }

        private void TopRectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void HSL_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (hueSlider.IsFocused == true || saturationSlider.IsFocused == true || lightnesSlider.IsFocused == true)
            {
                int h = (int)hueSlider.Value; hueTextBox.Text = h.ToString();
                double s = (double)saturationSlider.Value; saturationTextBox.Text = s.ToString();
                double l = lightnesSlider.Value; lightnessTextBox.Text = l.ToString();
                HlsToRgb((double)h, l, s, out int r, out int g, out int b);
                redSlider.Value = r; redTextBox.Text = r.ToString();
                greenSlider.Value = g; greenTextBox.Text = g.ToString();
                blueSlider.Value = b; blueTextBox.Text = b.ToString();
                int a = (int)alphaSlider.Value;
                Color argbColor = Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
                var x = new SolidColorBrush(argbColor);
                TopRectangle.Fill = x;
            }

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (redSlider.IsFocused == true || greenSlider.IsFocused == true || blueSlider.IsFocused == true || alphaSlider.IsFocused == true)
            {
                int cR = (int)redSlider.Value; redTextBox.Text = cR.ToString();
                int cG = (int)greenSlider.Value; greenTextBox.Text = cG.ToString();
                int cB = (int)blueSlider.Value; blueTextBox.Text = cB.ToString();
                int cA = (int)alphaSlider.Value; alphaTextBox.Text = cA.ToString();

                if (hueSlider.IsFocused == false || saturationSlider.IsFocused == false || lightnesSlider.IsFocused == false)
                {
                    RgbToHls(cR, cG, cB, out double h, out double l, out double s);
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
                //redSlider.Background = new SolidColorBrush(rColor);
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


                Color c = (Color)ColorConverter.ConvertFromString(x.ToString());
                //var cHex = "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
                Color argbColor = Color.FromArgb((byte)cA, (byte)cR, (byte)cG, (byte)cB);
                x = new SolidColorBrush(argbColor);
                TopRectangle.Fill = x;
                hexTextBox.Text = x.ToString();
                //var cRgb = "RGB(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
                //MessageBox.Show(cRgb);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            EyeDrop ss = new EyeDrop();
            ss.TakeScreenShot();


        }

        private void AddToPalette_Click(object sender, RoutedEventArgs e)
        {
            var FirstWrapPanelInTabControl = FindVisualChildren<WrapPanel>(TabControl1).FirstOrDefault();
            Brush newBrush = TopRectangle.Fill;
            //var newColor = dialog.Color;
            //Brush newBrush = new SolidColorBrush(newColor);
            var x = new ColorHM.Properties.UserControl1(); //new instance of rectangle user control.
            x.rectangleUC.Fill = newBrush;
            x.ToolTip = newBrush.ToString();
            FirstWrapPanelInTabControl.Children.Add(x); // Add the rectangle to the selected TabItem.
        }


        // Convert an RGB value into an HLS value.
        public static void RgbToHls(int r, int g, int b,
            out double h, out double l, out double s)
        {
            // Convert RGB to a 0.0 to 1.0 range.
            double double_r = r / 255.0;
            double double_g = g / 255.0;
            double double_b = b / 255.0;

            // Get the maximum and minimum RGB components.
            double max = double_r;
            if (max < double_g) max = double_g;
            if (max < double_b) max = double_b;

            double min = double_r;
            if (min > double_g) min = double_g;
            if (min > double_b) min = double_b;

            double diff = max - min;
            l = (max + min) / 2;
            if (Math.Abs(diff) < 0.00001)
            {
                s = 0;
                h = 0;  // H is really undefined.
            }
            else
            {
                if (l <= 0.5) s = diff / (max + min);
                else s = diff / (2 - max - min);

                double r_dist = (max - double_r) / diff;
                double g_dist = (max - double_g) / diff;
                double b_dist = (max - double_b) / diff;

                if (double_r == max) h = b_dist - g_dist;
                else if (double_g == max) h = 2 + r_dist - b_dist;
                else h = 4 + g_dist - r_dist;

                h = h * 60;
                if (h < 0) h += 360;
            }
        }
        // Convert an HLS value into an RGB value.
        public static void HlsToRgb(double h, double l, double s,
            out int r, out int g, out int b)
        {
            double p2;
            if (l <= 0.5) p2 = l * (1 + s);
            else p2 = l + s - l * s;

            double p1 = 2 * l - p2;
            double double_r, double_g, double_b;
            if (s == 0)
            {
                double_r = l;
                double_g = l;
                double_b = l;
            }
            else
            {
                double_r = QqhToRgb(p1, p2, h + 120);
                double_g = QqhToRgb(p1, p2, h);
                double_b = QqhToRgb(p1, p2, h - 120);
            }

            // Convert RGB to the 0 to 255 range.
            r = (int)(double_r * 255.0);
            g = (int)(double_g * 255.0);
            b = (int)(double_b * 255.0);
        }

        private static double QqhToRgb(double q1, double q2, double hue)
        {
            if (hue > 360) hue -= 360;
            else if (hue < 0) hue += 360;

            if (hue < 60) return q1 + (q2 - q1) * hue / 60;
            if (hue < 180) return q2;
            if (hue < 240) return q1 + (q2 - q1) * (240 - hue) / 60;
            return q1;
        }
    }
}
