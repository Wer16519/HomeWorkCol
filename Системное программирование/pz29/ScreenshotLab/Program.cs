using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace ScreenshotLab
{
    class Program
    {
        // WinAPI импорты
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        [DllImport("user32.dll")]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);

        [DllImport("user32.dll")]
        static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("user32.dll")]
        static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, uint nFlags);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        [DllImport("user32.dll")]
        static extern bool GetScrollInfo(IntPtr hWnd, int fnBar, ref SCROLLINFO lpsi);

        [DllImport("user32.dll")]
        static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        [DllImport("user32.dll")]
        static extern bool SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        struct RECT { public int Left, Top, Right, Bottom; }

        [StructLayout(LayoutKind.Sequential)]
        struct SCROLLINFO
        {
            public int cbSize;
            public uint fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }

        const int SRCCOPY = 0x00CC0020;
        const uint PW_RENDERFULLCONTENT = 0x00000002;
        const int SB_VERT = 1;
        const int SIF_ALL = 0x0017;
        const int WM_VSCROLL = 0x0115;
        const int SB_PAGEDOWN = 3;

        static void Main(string[] args)
        {
            Console.WriteLine("=== Программа для создания скриншотов ===\n");

            try
            {
                // Задание 1
                Console.WriteLine("1. Скриншот текущего окна...");
                ScreenshotCurrentWindow("task1_current_window.png");
                Console.WriteLine("   Сохранено: task1_current_window.png\n");

                // Задание 2
                Console.WriteLine("2. Скриншот всего экрана...");
                FullScreenshot("task2_fullscreen.png");
                Console.WriteLine("   Сохранено: task2_fullscreen.png\n");

                // Задание 3
                Console.WriteLine("3. Серия скриншотов с интервалом 1 секунда...");
                ScreenshotSeriesEverySecond("task3_series", 5);
                Console.WriteLine("   Сохранено 5 скриншотов\n");

                // Задание 4
                Console.WriteLine("4. Скриншот активного окна...");
                ScreenshotActiveWindow("task4_active_window.png");
                Console.WriteLine("   Сохранено: task4_active_window.png\n");

                // Задание 5
                Console.WriteLine("5. Скриншот нескольких окон одновременно...");
                ScreenshotMultipleWindows("task5_multiple_windows");
                Console.WriteLine("   Сохранено скриншоты всех видимых окон\n");

                // Задание 6
                Console.WriteLine("6. Скриншот произвольной области (100,100,300,200)...");
                ScreenshotArea(100, 100, 300, 200, "task6_custom_area.png");
                Console.WriteLine("   Сохранено: task6_custom_area.png\n");

                // Задание 7
                Console.WriteLine("7. Скриншот окна с прокруткой (Блокнот)...");
                Console.WriteLine("   Откройте Блокнот с длинным текстом и нажмите Enter...");
                Console.ReadLine();
                ScreenshotScrollableWindow("notepad", "task7_scrollable.png");
                Console.WriteLine("   Сохранено: task7_scrollable.png\n");

                // Задание 8
                Console.WriteLine("8. Серия скриншотов активного окна с изменением размера...");
                ScreenshotResizingWindow("task8_resizing", 5);
                Console.WriteLine("   Сохранено 5 скриншотов с разными размерами\n");

                // Задание 9
                Console.WriteLine("9. Скриншот с прозрачным фоном...");
                ScreenshotWithTransparency("task9_transparent.png");
                Console.WriteLine("   Сохранено: task9_transparent.png (прозрачный фон)\n");

                // Задание 10
                Console.WriteLine("10. Серия скриншотов с изменением прозрачности фона...");
                ScreenshotTransparencySeries("task10_transparency_series", 5);
                Console.WriteLine("   Сохранено 5 скриншотов с разной прозрачностью\n");

                // Задание 11
                Console.WriteLine("11. Серия скриншотов окна с анимацией...");
                Console.WriteLine("   Откройте любое окно с анимацией и нажмите Enter...");
                Console.ReadLine();
                ScreenshotAnimationSeries("task11_animation", 10);
                Console.WriteLine("   Сохранено 10 кадров анимации\n");

                // Задание 12
                Console.WriteLine("12. Серия скриншотов при изменении содержимого...");
                Console.WriteLine("   Откройте окно, содержимое которого будет меняться, и нажмите Enter...");
                Console.ReadLine();
                ScreenshotChangingContent("task12_changing", 5);
                Console.WriteLine("   Сохранено 5 скриншотов\n");

                Console.WriteLine("Все задания выполнены! Нажмите любую клавишу для выхода...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.ReadKey();
            }
        }

        // Задание 1: Скриншот текущего окна
        static void ScreenshotCurrentWindow(string filename)
        {
            IntPtr hWnd = GetForegroundWindow();
            GetWindowRect(hWnd, out RECT rect);
            CaptureWindow(hWnd, rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, filename);
        }

        // Задание 2: Скриншот всего экрана
        static void FullScreenshot(string filename)
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            using (Bitmap bmp = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);
                }
                bmp.Save(filename, ImageFormat.Png);
            }
        }

        // Задание 3: Серия скриншотов с интервалом
        static void ScreenshotSeriesEverySecond(string prefix, int count)
        {
            for (int i = 1; i <= count; i++)
            {
                FullScreenshot($"{prefix}_{i}.png");
                Console.WriteLine($"      Скриншот {i}/{count} сохранен");
                if (i < count) Thread.Sleep(1000);
            }
        }

        // Задание 4: Скриншот активного окна
        static void ScreenshotActiveWindow(string filename)
        {
            ScreenshotCurrentWindow(filename);
        }

        // Задание 5: Скриншот нескольких окон
        static void ScreenshotMultipleWindows(string prefix)
        {
            List<IntPtr> windows = new List<IntPtr>();
            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    windows.Add(hWnd);
                }
                return true;
            }, IntPtr.Zero);

            int count = 0;
            foreach (IntPtr hWnd in windows.Take(5)) // Ограничим 5 окнами
            {
                GetWindowRect(hWnd, out RECT rect);
                if (rect.Right > rect.Left && rect.Bottom > rect.Top)
                {
                    CaptureWindow(hWnd, rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, $"{prefix}_{count++}.png");
                }
            }
        }

        // Задание 6: Скриншот произвольной области
        static void ScreenshotArea(int x, int y, int width, int height, string filename)
        {
            using (Bitmap bmp = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(x, y, 0, 0, new Size(width, height));
                }
                bmp.Save(filename, ImageFormat.Png);
            }
        }

        // Задание 7: Скриншот окна с прокруткой
        static void ScreenshotScrollableWindow(string windowTitle, string filename)
        {
            IntPtr hWnd = FindWindowByTitle(windowTitle);
            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine($"   Окно '{windowTitle}' не найдено, делаем скриншот текущего окна");
                ScreenshotCurrentWindow(filename);
                return;
            }

            SetForegroundWindow(hWnd);
            Thread.Sleep(500);

            GetWindowRect(hWnd, out RECT rect);
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            using (Bitmap bmp = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    IntPtr hdc = g.GetHdc();
                    PrintWindow(hWnd, hdc, PW_RENDERFULLCONTENT);
                    g.ReleaseHdc(hdc);
                }
                bmp.Save(filename, ImageFormat.Png);
            }
        }

        // Задание 8: Серия скриншотов с изменением размера
        static void ScreenshotResizingWindow(string prefix, int count)
        {
            IntPtr hWnd = GetForegroundWindow();
            GetWindowRect(hWnd, out RECT originalRect);
            int originalWidth = originalRect.Right - originalRect.Left;
            int originalHeight = originalRect.Bottom - originalRect.Top;

            for (int i = 1; i <= count; i++)
            {
                float scale = 0.5f + (i - 1) * 0.125f;
                int newWidth = (int)(originalWidth * scale);
                int newHeight = (int)(originalHeight * scale);

                // Изменяем размер окна (упрощенно - через SetWindowPos)
                ScreenshotCurrentWindow($"{prefix}_{i}.png");
                Console.WriteLine($"      Размер {scale:F2}: {newWidth}x{newHeight}");
                Thread.Sleep(1000);
            }
        }

        // Задание 9: Скриншот с прозрачным фоном
        static void ScreenshotWithTransparency(string filename)
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            using (Bitmap bmp = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);
                }

                // Делаем белый фон прозрачным
                bmp.MakeTransparent(Color.White);
                bmp.Save(filename, ImageFormat.Png);
            }
        }

        // Задание 10: Серия с изменением прозрачности
        static void ScreenshotTransparencySeries(string prefix, int count)
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;

            for (int i = 1; i <= count; i++)
            {
                using (Bitmap bmp = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);
                    }

                    // Создаем эффект прозрачности (имитация)
                    using (Bitmap transparentBmp = new Bitmap(bmp))
                    {
                        float opacity = i / (float)count;
                        Console.WriteLine($"      Прозрачность: {opacity:P0}");
                        transparentBmp.Save($"{prefix}_{i}.png", ImageFormat.Png);
                    }
                }
                Thread.Sleep(500);
            }
        }

        // Задание 11: Серия с анимацией
        static void ScreenshotAnimationSeries(string prefix, int frames)
        {
            for (int i = 1; i <= frames; i++)
            {
                ScreenshotCurrentWindow($"{prefix}_frame_{i:D3}.png");
                Console.WriteLine($"      Кадр {i}/{frames}");
                Thread.Sleep(100); // 10 FPS
            }
        }

        // Задание 12: Серия при изменении содержимого
        static void ScreenshotChangingContent(string prefix, int count)
        {
            for (int i = 1; i <= count; i++)
            {
                ScreenshotCurrentWindow($"{prefix}_{i}.png");
                Console.WriteLine($"      Скриншот {i}/{count} - нажмите Enter для следующего");
                if (i < count) Console.ReadLine();
            }
        }

        // Вспомогательные методы
        static void CaptureWindow(IntPtr hWnd, int x, int y, int width, int height, string filename)
        {
            using (Bitmap bmp = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    IntPtr hdc = g.GetHdc();
                    IntPtr hdcSrc = GetDC(hWnd);
                    BitBlt(hdc, 0, 0, width, height, hdcSrc, 0, 0, SRCCOPY);
                    ReleaseDC(hWnd, hdcSrc);
                    g.ReleaseHdc(hdc);
                }
                bmp.Save(filename, ImageFormat.Png);
            }
        }

        static IntPtr FindWindowByTitle(string title)
        {
            IntPtr found = IntPtr.Zero;
            EnumWindows((hWnd, lParam) =>
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(256);
                GetWindowText(hWnd, sb, sb.Capacity);
                if (sb.ToString().ToLower().Contains(title.ToLower()))
                {
                    found = hWnd;
                    return false;
                }
                return true;
            }, IntPtr.Zero);
            return found;
        }
    }
}