using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Task45_ClipboardManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateClipboardInfo();
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = TxtInput.Text;
                if (string.IsNullOrEmpty(text))
                {
                    MessageBox.Show("Введите текст для копирования", "Предупреждение");
                    return;
                }

                Clipboard.SetText(text);
                LblStatus.Content = $"Текст скопирован в буфер обмена: {DateTime.Now:HH:mm:ss}";
                UpdateClipboardInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка копирования: {ex.Message}", "Ошибка");
            }
        }

        private void BtnPaste_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    string text = Clipboard.GetText();
                    TxtInput.Text = text;
                    LblStatus.Content = $"Текст вставлен из буфера обмена: {DateTime.Now:HH:mm:ss}";
                }
                else
                {
                    MessageBox.Show("Буфер обмена не содержит текста", "Предупреждение");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка вставки: {ex.Message}", "Ошибка");
            }
        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool hasText = Clipboard.ContainsText();
                string status = hasText ? "содержит текст" : "не содержит текст";
                LblStatus.Content = $"Буфер обмена {status}";

                if (hasText)
                {
                    string text = Clipboard.GetText();
                    int length = text.Length;
                    string preview = length > 50 ? text.Substring(0, 50) + "..." : text;
                    MessageBox.Show($"Буфер содержит текст ({length} символов):\n\n{preview}", "Информация");
                }
                else
                {
                    MessageBox.Show("Буфер обмена пуст", "Информация");
                }

                UpdateClipboardInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.Clear();
                LblStatus.Content = $"Буфер обмена очищен: {DateTime.Now:HH:mm:ss}";
                UpdateClipboardInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка очистки: {ex.Message}", "Ошибка");
            }
        }

        private void BtnFormat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = TxtInput.Text;
                if (string.IsNullOrEmpty(text))
                {
                    MessageBox.Show("Введите текст для форматирования", "Предупреждение");
                    return;
                }

                RichTextBox richTextBox = new RichTextBox();
                richTextBox.Document.Blocks.Clear();

                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run("=== Форматированный текст ===\n")
                {
                    FontWeight = FontWeights.Bold,
                    FontSize = 16,
                    Foreground = new SolidColorBrush(Colors.Blue)
                });

                paragraph.Inlines.Add(new Run($"\n{text}\n")
                {
                    FontSize = 14,
                    Foreground = new SolidColorBrush(Colors.Black)
                });

                paragraph.Inlines.Add(new Run($"\nСоздано: {DateTime.Now:dd.MM.yyyy HH:mm:ss}")
                {
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Colors.Gray)
                });

                richTextBox.Document.Blocks.Add(paragraph);

                TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                DataObject dataObject = new DataObject();
                dataObject.SetData(DataFormats.Rtf, range.Text);
                dataObject.SetData(DataFormats.Text, text);

                Clipboard.SetDataObject(dataObject, true);

                LblStatus.Content = $"Форматированный текст скопирован в буфер: {DateTime.Now:HH:mm:ss}";
                UpdateClipboardInfo();

                MessageBox.Show("Форматированный текст скопирован в буфер обмена!\n\nМожно вставить в Word или другой редактор.", "Готово");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void UpdateClipboardInfo()
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    string text = Clipboard.GetText();
                    int length = text.Length;
                    LblInfo.Content = $"Буфер: {length} символов";
                }
                else
                {
                    LblInfo.Content = "Буфер: пуст";
                }
            }
            catch
            {
                LblInfo.Content = "Буфер: недоступен";
            }
        }
    }
}