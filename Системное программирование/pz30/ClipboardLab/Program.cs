using Microsoft.VisualBasic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ClipboardLab;

class Program
{
    [STAThread]
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("=== Программа для работы с буфером обмена ===\n");

        try
        {
            // Задание 1: Копировать текст в буфер обмена
            Console.WriteLine("1. Копирование текста в буфер обмена...");
            CopyToClipboard("Hello, World! Это текст из программы.");
            Console.WriteLine("   Текст скопирован: \"Hello, World! Это текст из программы.\"\n");

            // Задание 2: Вставить текст из буфера обмена
            Console.WriteLine("2. Вставка текста из буфера обмена...");
            string pastedText = PasteFromClipboard();
            Console.WriteLine($"   Вставленный текст: \"{pastedText}\"\n");

            // Задание 3: Проверить, есть ли текст в буфере обмена
            Console.WriteLine("3. Проверка наличия текста в буфере обмена...");
            bool hasText = HasTextInClipboard();
            Console.WriteLine($"   Текст в буфере обмена: {(hasText ? "Да" : "Нет")}\n");

            // Задание 4: Очистить буфер обмена
            Console.WriteLine("4. Очистка буфера обмена...");
            ClearClipboard();
            Console.WriteLine($"   Буфер очищен. Текст есть: {HasTextInClipboard()}\n");

            // Восстанавливаем текст для следующих заданий
            CopyToClipboard("Пример форматированного текста");

            // Задание 5: Работа с форматированным текстом
            Console.WriteLine("5. Работа с форматированным текстом...");
            WorkWithFormattedText();
            Console.WriteLine();

            // Задание 6: Работа со строками (объединение, разбиение)
            Console.WriteLine("6. Работа со строками в буфере обмена...");
            WorkWithStrings();
            Console.WriteLine();

            // Задание 7: Обработка ошибок
            Console.WriteLine("7. Обработка ошибок при работе с буфером обмена...");
            HandleClipboardErrors();
            Console.WriteLine();

            // Задание 8: Кроссплатформенность (информация)
            Console.WriteLine("8. Кроссплатформенность...");
            ShowCrossPlatformInfo();
            Console.WriteLine();

            // Задание 9: Работа с изображениями
            Console.WriteLine("9. Работа с изображениями в буфере обмена...");
            WorkWithImages();
            Console.WriteLine();

            // Задание 10: Создание и редактирование форматированного текста
            Console.WriteLine("10. Создание и редактирование форматированного текста...");
            CreateAndEditRichText();
            Console.WriteLine();

            Console.WriteLine("\n✅ Все 10 заданий выполнены! Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            Console.ReadKey();
        }
    }

    // Задание 1: Копирование текста
    static void CopyToClipboard(string text)
    {
        try
        {
            Clipboard.SetText(text);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Ошибка копирования: {ex.Message}");
        }
    }

    // Задание 2: Вставка текста
    static string PasteFromClipboard()
    {
        try
        {
            if (Clipboard.ContainsText())
                return Clipboard.GetText();
            return "Буфер обмена не содержит текст";
        }
        catch (Exception ex)
        {
            return $"Ошибка вставки: {ex.Message}";
        }
    }

    // Задание 3: Проверка наличия текста
    static bool HasTextInClipboard()
    {
        try
        {
            return Clipboard.ContainsText();
        }
        catch
        {
            return false;
        }
    }

    // Задание 4: Очистка буфера обмена
    static void ClearClipboard()
    {
        try
        {
            Clipboard.Clear();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Ошибка очистки: {ex.Message}");
        }
    }

    // Задание 5: Работа с форматированным текстом
    static void WorkWithFormattedText()
    {
        // Создаем RichTextBox для форматирования
        using (RichTextBox rtb = new RichTextBox())
        {
            rtb.Text = "Форматированный текст";
            rtb.SelectAll();
            rtb.SelectionFont = new Font("Arial", 14, FontStyle.Bold | FontStyle.Italic);
            rtb.SelectionColor = Color.Blue;

            // Копируем форматированный текст в буфер
            Clipboard.SetData(DataFormats.Rtf, rtb.Rtf);
            Console.WriteLine("   Форматированный текст (RTF) скопирован в буфер обмена");

            // Проверяем, что в буфере есть RTF
            if (Clipboard.ContainsData(DataFormats.Rtf))
            {
                string rtfData = Clipboard.GetData(DataFormats.Rtf) as string ?? "";
                Console.WriteLine($"   RTF данные получены, длина: {rtfData.Length} символов");
            }

            // Получаем текст как обычный текст
            if (Clipboard.ContainsText())
            {
                string plainText = Clipboard.GetText();
                Console.WriteLine($"   Как обычный текст: \"{plainText}\"");
            }
        }
    }

    // Задание 6: Работа со строками
    static void WorkWithStrings()
    {
        // Исходный текст
        string originalText = "яблоко,банан,вишня,апельсин,манго";
        Console.WriteLine($"   Исходная строка: \"{originalText}\"");

        // Копируем в буфер
        CopyToClipboard(originalText);

        // Получаем и обрабатываем
        string text = PasteFromClipboard();

        // Разбиение на части
        string[] fruits = text.Split(',');
        Console.WriteLine($"   Разбиение на части: {string.Join(", ", fruits)}");

        // Объединение с другим разделителем
        string joined = string.Join(" | ", fruits);
        Console.WriteLine($"   Объединение с ' | ': \"{joined}\"");

        // Обратный порядок
        Array.Reverse(fruits);
        string reversed = string.Join(", ", fruits);
        Console.WriteLine($"   Обратный порядок: \"{reversed}\"");

        // Верхний регистр
        string upper = text.ToUpper();
        Console.WriteLine($"   Верхний регистр: \"{upper}\"");

        // Копируем обработанный текст обратно
        CopyToClipboard(joined);
        Console.WriteLine($"   Обработанный текст скопирован в буфер");
    }

    // Задание 7: Обработка ошибок
    static void HandleClipboardErrors()
    {
        // Ошибка 1: Попытка получить несуществующий формат
        try
        {
            if (Clipboard.ContainsData("NonExistentFormat"))
            {
                Clipboard.GetData("NonExistentFormat");
            }
            else
            {
                Console.WriteLine("   OK: Формат 'NonExistentFormat' не найден (ожидаемое поведение)");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Ошибка при проверке формата: {ex.Message}");
        }

        // Ошибка 2: Попытка установить null
        try
        {
            Clipboard.SetText(null!);
        }
        catch (ArgumentNullException)
        {
            Console.WriteLine("   OK: Поймано исключение ArgumentNullException при установке null");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Другая ошибка: {ex.Message}");
        }

        // Ошибка 3: Безопасная работа с изображением
        try
        {
            if (!Clipboard.ContainsImage())
            {
                Console.WriteLine("   OK: Изображение не обнаружено (можно безопасно продолжать)");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Ошибка при проверке изображения: {ex.Message}");
        }

        Console.WriteLine("   Обработка ошибок завершена");
    }

    // Задание 8: Кроссплатформенность
    static void ShowCrossPlatformInfo()
    {
        Console.WriteLine("   Информация о кроссплатформенности:");
        Console.WriteLine("   - Windows: Полная поддержка через System.Windows.Forms");
        Console.WriteLine("   - Linux/macOS: Требуется установка пакетов:");
        Console.WriteLine("     * dotnet add package System.Drawing.Common");
        Console.WriteLine("     * Установка libgdiplus (sudo apt install libgdiplus)");
        Console.WriteLine("   - Альтернативы для кроссплатформенности:");
        Console.WriteLine("     * Avalonia UI");
        Console.WriteLine("     * TextCopy (NuGet пакет для кроссплатформенного буфера обмена)");

        // Демонстрация работы с TextCopy (если установлен)
        // try
        // {
        //     TextCopy.ClipboardService.SetText("Кроссплатформенный текст");
        //     string text = TextCopy.ClipboardService.GetText();
        //     Console.WriteLine($"   TextCopy работает: \"{text}\"");
        // }
        // catch { }
    }

    // Задание 9: Работа с изображениями
    static void WorkWithImages()
    {
        // Создаем простое изображение
        using (Bitmap bmp = new Bitmap(200, 100))
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.DrawString("Скриншот из программы",
                    new Font("Arial", 12, FontStyle.Bold),
                    Brushes.Blue,
                    new PointF(10, 40));
                g.DrawRectangle(Pens.Red, 5, 5, 190, 90);
            }

            // Копируем изображение в буфер обмена
            Clipboard.SetImage(bmp);
            Console.WriteLine("   Изображение скопировано в буфер обмена");

            // Проверяем наличие изображения
            if (Clipboard.ContainsImage())
            {
                Image? retrievedImage = Clipboard.GetImage();
                Console.WriteLine($"   Изображение получено из буфера, размер: {retrievedImage?.Width}x{retrievedImage?.Height}");
                retrievedImage?.Dispose();
            }

            // Сохраняем изображение на диск (опционально)
            bmp.Save("clipboard_image.png", System.Drawing.Imaging.ImageFormat.Png);
            Console.WriteLine("   Изображение сохранено как 'clipboard_image.png'");
        }
    }

    // Задание 10: Создание и редактирование форматированного текста
    static void CreateAndEditRichText()
    {
        using (RichTextBox rtb = new RichTextBox())
        {
            // Создаем форматированный текст
            rtb.Text = "Это пример форматированного текста.\n";

            // Добавляем форматирование
            rtb.Select(0, 3);
            rtb.SelectionFont = new Font("Arial", 16, FontStyle.Bold);
            rtb.SelectionColor = Color.Red;

            rtb.Select(4, 7);
            rtb.SelectionFont = new Font("Times New Roman", 12, FontStyle.Italic);
            rtb.SelectionColor = Color.Green;

            rtb.Select(12, 8);
            rtb.SelectionFont = new Font("Courier New", 14, FontStyle.Underline);
            rtb.SelectionColor = Color.Blue;

            // Копируем в буфер обмена
            Clipboard.SetData(DataFormats.Rtf, rtb.Rtf);
            Console.WriteLine("   Форматированный текст создан и скопирован в буфер");

            // Редактируем текст через буфер
            if (Clipboard.ContainsData(DataFormats.Rtf))
            {
                string rtfContent = Clipboard.GetData(DataFormats.Rtf) as string ?? "";

                // Редактируем RTF (добавляем текст в конец)
                using (RichTextBox rtbEdit = new RichTextBox())
                {
                    rtbEdit.Rtf = rtfContent;
                    rtbEdit.AppendText("\n\n[Добавлено редактирование]");
                    rtbEdit.SelectAll();
                    rtbEdit.SelectionColor = Color.Purple;

                    // Копируем отредактированный текст обратно
                    Clipboard.SetData(DataFormats.Rtf, rtbEdit.Rtf);
                    Console.WriteLine("   Форматированный текст отредактирован и скопирован обратно");
                }
            }

            // Показываем как обычный текст
            if (Clipboard.ContainsText())
            {
                string plainText = Clipboard.GetText();
                Console.WriteLine($"   Как обычный текст: \"{plainText}\"");
            }
        }
    }
}