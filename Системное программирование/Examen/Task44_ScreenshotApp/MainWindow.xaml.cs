using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Task44_ScreenshotApp
{
    public partial class MainWindow : Window
    {
        private int screenshotCount = 0;
        private bool isSeriesRunning = false;
        private Bitmap? currentBitmap = null;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                currentBitmap = CaptureFullScreen();
                if (currentBitmap != null)
                {
                    ScreenshotImage.Source = BitmapToImageSource(currentBitmap);
                    LblStatus.Content = $"Сделан скриншот всего экрана: {DateTime.Now:HH:mm:ss}";
                    UpdateCount();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void BtnActiveWindow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                currentBitmap = CaptureActiveWindow();
                if (currentBitmap != null)
                {
                    ScreenshotImage.Source = BitmapToImageSource(currentBitmap);
                    LblStatus.Content = $"Сделан скриншот активного окна: {DateTime.Now:HH:mm:ss}";
                    UpdateCount();
                }
                else
                {
                    MessageBox.Show("Не удалось получить активное окно", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void BtnCurrentApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                currentBitmap = CaptureCurrentApp();
                if (currentBitmap != null)
                {
                    ScreenshotImage.Source = BitmapToImageSource(currentBitmap);
                    LblStatus.Content = $"Сделан скриншот текущего приложения: {DateTime.Now:HH:mm:ss}";
                    UpdateCount();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void BtnSeries_Click(object sender, RoutedEventArgs e)
        {
            if (isSeriesRunning)
            {
                isSeriesRunning = false;
                BtnSeries.Content = "Серия (1 сек)";
                LblStatus.Content = "Серия остановлена";
                return;
            }

            isSeriesRunning = true;
            BtnSeries.Content = "Остановить";
            LblStatus.Content = "Запуск серии...";

            Thread thread = new Thread(StartSeries);
            thread.IsBackground = true;
            thread.Start();
        }

        private void StartSeries()
        {
            int count = 0;
            while (isSeriesRunning && count < 50)
            {
                try
                {
                    Bitmap? screenshot = CaptureFullScreen();
                    if (screenshot != null)
                    {
                        string fileName = $"screenshot_{DateTime.Now:HHmmss}.png";
                        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
                        screenshot.Save(filePath, ImageFormat.Png);

                        Dispatcher.Invoke(() =>
                        {
                            currentBitmap = screenshot;
                            ScreenshotImage.Source = BitmapToImageSource(screenshot);
                            LblStatus.Content = $"Серия: скриншот #{++count} сохранен на рабочий стол";
                            UpdateCount();
                        });
                    }

                    Thread.Sleep(1000);
                }
                catch
                {
                    break;
                }
            }

            Dispatcher.Invoke(() =>
            {
                isSeriesRunning = false;
                BtnSeries.Content = "Серия (1 сек)";
                LblStatus.Content = $"Серия завершена. Сделано {count} скриншотов";
            });
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (currentBitmap == null)
            {
                MessageBox.Show("Нет изображения для сохранения", "Ошибка");
                return;
            }

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|BMP Image|*.bmp";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    ImageFormat format = ImageFormat.Png;
                    if (saveFileDialog.FilterIndex == 2) format = ImageFormat.Jpeg;
                    if (saveFileDialog.FilterIndex == 3) format = ImageFormat.Bmp;

                    currentBitmap.Save(saveFileDialog.FileName, format);
                    LblStatus.Content = $"Скриншот сохранен: {saveFileDialog.FileName}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка");
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ScreenshotImage.Source = null;
            if (currentBitmap != null)
            {
                currentBitmap.Dispose();
                currentBitmap = null;
            }
            LblStatus.Content = "Изображение очищено";
        }

        private Bitmap? CaptureFullScreen()
        {
            try
            {
                int width = GetSystemMetrics(SM_CXSCREEN);
                int height = GetSystemMetrics(SM_CYSCREEN);

                Bitmap bitmap = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(width, height));
                }
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        private Bitmap? CaptureActiveWindow()
        {
            try
            {
                IntPtr hWnd = GetForegroundWindow();
                if (hWnd == IntPtr.Zero) return null;

                Rect rect = new Rect();
                GetWindowRect(hWnd, ref rect);

                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;

                if (width <= 0 || height <= 0) return null;

                Bitmap bitmap = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(rect.Left, rect.Top, 0, 0, new System.Drawing.Size(width, height));
                }
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        private Bitmap? CaptureCurrentApp()
        {
            try
            {
                int left = (int)this.Left;
                int top = (int)this.Top;
                int width = (int)this.Width;
                int height = (int)this.Height;

                Bitmap bitmap = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(left, top, 0, 0, new System.Drawing.Size(width, height));
                }
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        private BitmapImage? BitmapToImageSource(Bitmap bitmap)
        {
            try
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, ImageFormat.Png);
                    memory.Position = 0;

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    return bitmapImage;
                }
            }
            catch
            {
                return null;
            }
        }

        private void UpdateCount()
        {
            screenshotCount++;
            LblCount.Content = $"Скриншотов: {screenshotCount}";
        }
    }
}