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
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GoButtonPress(object sender, RoutedEventArgs e)
        {
            var runner = new Runner();
            await UpdateBitmap(runner);

            await runner.Run();
        }

        private async void GoContinualButtonPress(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                var runner = new Runner();
                await UpdateBitmap(runner);
                await runner.Run();

                await Task.Delay(300);
            }
        }

        private async Task UpdateBitmap(Runner runner)
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
            await this.UpdateBitmap(new Runner());
        }

        private async void DownButtonPress(object sender, RoutedEventArgs e)
        {
            await PinvokeHelpers.SendKeyPress(WindowsVirtualKey.Down);
            await Task.Delay(500);
            await this.UpdateBitmap(new Runner());
        }

        private async void LeftButtonPress(object sender, RoutedEventArgs e)
        {
            await PinvokeHelpers.SendKeyPress(WindowsVirtualKey.Left);
            await Task.Delay(500);
            await this.UpdateBitmap(new Runner());

        }

        private async void RightButtonPress(object sender, RoutedEventArgs e)
        {
            await PinvokeHelpers.SendKeyPress(WindowsVirtualKey.Right);
            await Task.Delay(500);
            await this.UpdateBitmap(new Runner());
        }
    }
}
