using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


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
                dynamic rec = sender;
                dynamic fill = rec.Fill;
                dynamic color = fill.Color;

              
                Color c = (Color)ColorConverter.ConvertFromString(fill.ToString());
                //var cHex = "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
                //var cRgb = "RGB(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";



                x.TopRectangle.Fill = fill;
                x.redSlider.Value = c.R; x.redTextBox.Text = c.R.ToString();
                x.greenSlider.Value = c.G; x.greenTextBox.Text = c.G.ToString();
                x.blueSlider.Value = c.B; x.blueTextBox.Text = c.B.ToString(); 
                x.alphaSlider.Value = c.A; x.alphaTextBox.Text = c.A.ToString();

            }

            
        }
    }

}
