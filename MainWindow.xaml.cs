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
       
            GetPalettes();
            Color c = new Color();
            c.R = 82;
            c.G = 212;
            c.B = 192;
            c.A = 255;

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
            redSlider.Value = (int)color.R; redTextBox.Text = color.R.ToString();
            greenSlider.Value = (int)color.G; greenTextBox.Text = color.G.ToString();
            blueSlider.Value = (int)color.B; blueTextBox.Text = color.B.ToString();
            alphaSlider.Value = (int)color.A; alphaTextBox.Text = color.A.ToString();
            RGBandHSL.RgbToHls((int)color.R, (int)color.G, (int)color.B, out double h, out double l, out double s);
            hueTextBox.Text = h.ToString(); hueSlider.Value = h;
            saturationTextBox.Text = s.ToString(); saturationSlider.Value = s;
            lightnessTextBox.Text = l.ToString(); lightnesSlider.Value = l;
            TopRectangle.Fill = brush;
            hexTextBox.Text = brush.ToString();

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
            TabItem tabItem = label.Parent as TabItem;
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
                Text = "New Palette",
                
            };
           

            tabItem.Header = newPaletteTextBox;
            ContextMenu contextMenu = new ContextMenu();
            MenuItem savePaletteMenuItem = new MenuItem();
            contextMenu.Items.Add(savePaletteMenuItem);
            savePaletteMenuItem.Header = "Save Palette";
            savePaletteMenuItem.Click += new RoutedEventHandler(SavePaletteEvent);
            tabItem.ContextMenu = contextMenu;
            tabItem.Content = wcp;
            tabItem.IsSelected = true;
            NewPalette();
            SavePalette();
        }



        public void DeleteColor(object sender, RoutedEventArgs e)
        {
            // Get the color, and the palette db row id.
            MenuItem x = sender as MenuItem;
            ContextMenu y = x.Parent as ContextMenu;
            Rectangle rec = y.PlacementTarget as Rectangle; //! PlacementTarget gets the container where the context menu was clicked.
            Grid grid = rec.Parent as Grid;
            UserControl uc = grid.Parent as UserControl;
            WrapPanel wp = uc.Parent as WrapPanel;
            TabItem tab = wp.Parent as TabItem;
            string id = tab.Tag.ToString(); //! All of the above to get the id of the palette db row.
            SolidColorBrush brush = rec.Fill as SolidColorBrush;
            Color color = brush.Color;

            // Connect to db
            SQLiteConnection conn = Connect();
            SQLiteCommand sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = $"SELECT id, colors from palettes where id = '{id}'";
            SQLiteDataAdapter dt = new SQLiteDataAdapter(sqlite_cmd);
            DataTable palettesDT = new DataTable();
            dt.Fill(palettesDT);
            palettesDT = new DataTable();
            dt.Fill(palettesDT);

            StringBuilder paletteColors = new StringBuilder();
            foreach (DataRow row in palettesDT.Rows)
            {
                string colorsString = row[1].ToString();
                //MessageBox.Show(colorsString.ToString());
                List<string> colorsList = new List<string>();
                colorsList = colorsString.Split(null).ToList();
                int i = 0;
                int iremove = -1;
                foreach (string individualColorString in colorsList)
                {
                    if (individualColorString.Trim() == color.ToString())
                    {
                        iremove = i;
                    }
                    i++;
                }
                colorsList.RemoveAt(iremove);
                
                foreach (string paletteColor in colorsList)
                {
               
                    paletteColors.Append(paletteColor + " ");
                }  
                    
            }
            sqlite_cmd.CommandText = $"UPDATE palettes SET colors = '{paletteColors.ToString()}' WHERE id = {id}";
            sqlite_cmd.ExecuteNonQuery();
            conn.Close();
            grid.Children.Remove(rec);
        }

        public ContextMenu RecContextMenu()
        {
            ContextMenu recContextMenu = new ContextMenu();
            MenuItem deleteRec = new MenuItem();
            deleteRec.Header = "Delete Color";
            deleteRec.Click += new RoutedEventHandler(DeleteColor);

            recContextMenu.Items.Add(deleteRec);

            return recContextMenu;
        }

        //! Get all palettes, create new if none exist.
        public void GetPalettes()
        {
            SQLiteConnection conn = Connect();
            SQLiteCommand sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT id, palette_name, colors from palettes";
            SQLiteDataAdapter dt = new SQLiteDataAdapter(sqlite_cmd);
            DataTable palettesDT = new DataTable();
            dt.Fill(palettesDT);

            //! If there are no palettes, create one and rerun the query.
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
                MenuItem savePaletteMenuItem = new MenuItem();
                MenuItem deletePaletteMenuItem = new MenuItem();
                contextMenu.Items.Add(savePaletteMenuItem);
                contextMenu.Items.Add(deletePaletteMenuItem);
                savePaletteMenuItem.Header = "Save Palette";
                savePaletteMenuItem.Click += new RoutedEventHandler(SavePaletteEvent);
                deletePaletteMenuItem.Header = "Delete Palette";
                deletePaletteMenuItem.Click += new RoutedEventHandler(DeletePalette);


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
                string paletteID = row["id"].ToString();
                string colors = row["colors"].ToString();
                List<string> colorList = colors.Split(null).ToList();

                //! Create a rectangle for each color per palette
                foreach (string color in colorList)
                {

                    //! make sure color is in fact a color before processing 
                    //! because whitespace may exist at the end of the color string in the db
                    if (color.Contains('#'))
                    {
                        Color newColor = (Color)ColorConverter.ConvertFromString(color);
                        Brush newBrush = new SolidColorBrush(newColor);
                        ColorHM.Properties.UserControl1 rec = CreateRectangle(newBrush);
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

        // Delete palette.
        public void DeletePalette(object sender, RoutedEventArgs e)
        {
            TabItem tab = TabControl1.SelectedItem as TabItem;

            MessageBox.Show(tab.Tag.ToString());
            SQLiteConnection conn = Connect();
            SQLiteCommand sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = $"DELETE FROM palettes WHERE id={tab.Tag}";
            sqlite_cmd.ExecuteNonQuery();
            conn.Close();
            TabControl1.Items.Clear();
            GetPalettes();

        }
        public void SavePaletteEvent(object sender, RoutedEventArgs e)
        {
            SavePalette();
        }

        public void SavePalette()
        {

            dynamic selectedTab = TabControl1.SelectedContent;
            dynamic selectedTabHeader = TabControl1.SelectedItem;
            string selectedTabHeaderText = selectedTabHeader.Header.Text.ToString();

            ContextMenu contextMenu = new ContextMenu();
            MenuItem savePaletteMenuItem = new MenuItem();
            MenuItem deletePaletteMenuItem = new MenuItem();
            contextMenu.Items.Add(savePaletteMenuItem);
            contextMenu.Items.Add(deletePaletteMenuItem);
            savePaletteMenuItem.Header = "Save Palette";
            savePaletteMenuItem.Click += new RoutedEventHandler(SavePaletteEvent);
            deletePaletteMenuItem.Header = "Delete Palette";
            deletePaletteMenuItem.Click += new RoutedEventHandler(DeletePalette);

            selectedTabHeader.ContextMenu = contextMenu;

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


            //! If there is no tag in the tabitem.
            //! Since when a new palette is created, the palette is not yet assigned a tag, give it a tag.
            //! The tag holds the database ID of the palette.
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

        //! Maybe move to class later.
        public ColorHM.Properties.UserControl1 CreateRectangle(Brush newBrush)
        {
            ContextMenu recContextMenu = RecContextMenu();
            var rec = new ColorHM.Properties.UserControl1();
            rec.rectangleUC.Fill = newBrush;
            rec.ToolTip = newBrush.ToString();
            rec.rectangleUC.ContextMenu = recContextMenu;
            return rec;
        }


        private void AddToPalette_Click(object sender, RoutedEventArgs e)
        { 
            var FirstWrapPanelInTabControl = FindVisualChildren<WrapPanel>(TabControl1).FirstOrDefault();
            Brush newBrush = TopRectangle.Fill;
            ColorHM.Properties.UserControl1 rec = CreateRectangle(newBrush); 
            FirstWrapPanelInTabControl.Children.Add(rec);
            SavePalette(); // New or old palette
        }


    }
}
