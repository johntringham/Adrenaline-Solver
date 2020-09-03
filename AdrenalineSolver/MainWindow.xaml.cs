using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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

namespace AdrenalineSolver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool continualSolve;

        public MainWindow()
        {
            InitializeComponent();
        }

        public static int Delay { get; set; }

        private async void GoButtonPress(object sender, RoutedEventArgs e)
        {
            var runner = new Runner();
            UpdateBitmap(runner);
            await Task.Delay(100);
            await runner.Run();

            // show the next one after the run finished
            await Task.Delay(200);
            runner = new Runner();
            UpdateBitmap(runner);
        }

        private async void GoContinualButtonPress(object sender, RoutedEventArgs e)
        {
            continualSolve = true;

            var runner = new Runner();
            UpdateBitmap(runner);

            while (continualSolve)
            {
                await Task.Delay(300);
                await runner.Run();
                await Task.Delay(200);
                UpdateBitmap(runner);
            }
        }

        private void UpdateBitmap(Runner runner)
        {
            try
            {
                var bitmap = runner.AnalyseBitmap();
                var imageSource = BitmapToImageSource(bitmap);
                this.DisplayImage.Source = imageSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception caught:" + ex.Message);
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private async void UpButtonPress(object sender, RoutedEventArgs e)
        {
            await PinvokeHelpers.SendKeyPress(WindowsVirtualKey.Up);
            await Task.Delay(500);
            this.UpdateBitmap(new Runner());
        }

        private async void DownButtonPress(object sender, RoutedEventArgs e)
        {
            await PinvokeHelpers.SendKeyPress(WindowsVirtualKey.Down);
            await Task.Delay(500);
            this.UpdateBitmap(new Runner());
        }

        private async void LeftButtonPress(object sender, RoutedEventArgs e)
        {
            await PinvokeHelpers.SendKeyPress(WindowsVirtualKey.Left);
            await Task.Delay(500);
            this.UpdateBitmap(new Runner());

        }

        private async void RightButtonPress(object sender, RoutedEventArgs e)
        {
            await PinvokeHelpers.SendKeyPress(WindowsVirtualKey.Right);
            await Task.Delay(500);
            this.UpdateBitmap(new Runner());
        }

        private void SpeedSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Delay = (int)this.SpeedSlider.Value;
        }

        private void StopButtonPress(object sender, RoutedEventArgs e)
        {
            this.continualSolve = false;
        }

        private void AnalyseButtonPress(object sender, RoutedEventArgs e)
        {
            this.UpdateBitmap(new Runner());
        }
    }
}
