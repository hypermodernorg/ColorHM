using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace ColorHM.Properties
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        private void RectangleUC_MouseDown(object sender, MouseButtonEventArgs e)
        { 

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                dynamic x = Application.Current.MainWindow;
                Rectangle rec = sender as Rectangle;
                dynamic fill = rec.Fill;
                dynamic color = fill.Color;
         
                Color c = (Color)ColorConverter.ConvertFromString(fill.ToString());

                x.TopRectangle.Fill = fill;
                x.redSlider.Value = c.R; x.redTextBox.Text = c.R.ToString();
                x.greenSlider.Value = c.G; x.greenTextBox.Text = c.G.ToString();
                x.blueSlider.Value = c.B; x.blueTextBox.Text = c.B.ToString(); 
                x.alphaSlider.Value = c.A; x.alphaTextBox.Text = c.A.ToString();
                x.hexTextBox.Text = color.ToString();
                ColorHM.RGBandHSL.RgbToHls(c.R, c.G, c.B, out double h, out double l, out double s);
                x.hueSlider.Value = h; x.hueTextBox.Text = h.ToString();
                x.lightnesSlider.Value = l; x.lightnessTextBox.Text = (l*100).ToString();
                x.saturationSlider.Value = s; x.saturationTextBox.Text = (s*100).ToString();
              
            }  
        }

        private void RectangleUC_MouseEnter(object sender, MouseEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush();

            Color color = new Color
            {
                R = 255,
                G = 255,
                B = 255, 
                A = 255
            };
            brush.Color = color;
            rectangleUC.StrokeThickness = 4;
            rectangleUC.Stroke = brush;
        }

        private void RectangleUC_MouseLeave(object sender, MouseEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush();

            Color color = new Color
            {
                R = 0,
                G = 0,
                B = 0,
                A = 255
            };
            brush.Color = color;
            rectangleUC.StrokeThickness = 1;
            rectangleUC.Stroke = brush;
     
        }
    }
}
