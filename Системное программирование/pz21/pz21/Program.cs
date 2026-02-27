using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Collections.Generic;
using System.IO.Compression;
using Microsoft.VisualBasic.FileIO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Data.Sqlite;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Path = System.IO.Path;

namespace pz21
{
    class Program
    {
        static string textFilePath = "test.txt";
        static string binaryFilePath = "test.bin";
        static string jsonFilePath = "test.json";
        static string xmlFilePath = "test.xml";
        static string csvFilePath = "test.csv";
        static string zipFilePath = "archive.zip";
        static string tarFilePath = "archive.tar";
        static string isoFilePath = "disk.iso";
        static string pdfFilePath = "test.pdf";
        static string audioFilePath = "test.mp3";
        static string videoFilePath = "test.mp4";
        static string fontFilePath = "test.ttf";
        static string imageFilePath = "test.png";
        static string dbFilePath = "test.db";
        static string iniFilePath = "config.ini";
        static string yamlFilePath = "config.yaml";

        static void Main()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("Практическое занятие 21: Файловый ввод/вывод");
            Console.WriteLine("==========================================\n");

            try
            {
                Task1_ReadTextFile();
                Task2_WriteTextFile();
                Task3_BinaryData();
                Task4_Json();
                Task5_Xml();
                Task6_Csv();
                Task7_Zip();
                Task8_Tar();
                Task9_Iso();
                Task10_Pdf();
                Task11_AudioVideo();
                Task12_Fonts();
                Task13_Images();
                Task14_Database();
                Task15_Configs();

                Console.WriteLine("\n==========================================");
                Console.WriteLine("ВСЕ ЗАДАНИЯ УСПЕШНО ВЫПОЛНЕНЫ!");
                Console.WriteLine("==========================================");

                Console.WriteLine("\nСозданные файлы:");
                foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.*"))
                {
                    FileInfo fi = new FileInfo(file);
                    Console.WriteLine($"- {fi.Name} ({fi.Length} байт)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОШИБКА: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void Task1_ReadTextFile()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 1: Чтение текстового файла ---");
            if (File.Exists(textFilePath))
            {
                string content = File.ReadAllText(textFilePath);
                Console.WriteLine("Содержимое файла:");
                Console.WriteLine(content);
            }
            else
            {
                Console.WriteLine("Файл не найден. Пропускаем чтение.");
            }
        }

        static void Task2_WriteTextFile()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 2: Запись в текстовый файл ---");
            string textContent = "Это тестовый текст для практического занятия.\n";
            textContent += "Строка 2: Файловый ввод/вывод в C#\n";
            textContent += "Строка 3: " + DateTime.Now.ToString();

            File.WriteAllText(textFilePath, textContent);
            Console.WriteLine($"Текст записан в {textFilePath}");
        }

        static void Task3_BinaryData()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 3: Двоичные данные ---");
            byte[] dataToWrite = new byte[32];
            for (int i = 0; i < 32; i++)
                dataToWrite[i] = (byte)i;

            File.WriteAllBytes(binaryFilePath, dataToWrite);
            Console.WriteLine($"Бинарные данные записаны в {binaryFilePath}");

            byte[] dataRead = File.ReadAllBytes(binaryFilePath);
            Console.WriteLine("Прочитанные данные (первые 16 байт):");
            for (int i = 0; i < 16; i++)
                Console.Write($"{dataRead[i]:X2} ");
            Console.WriteLine();
        }

        static void Task4_Json()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 4: JSON ---");
            var data = new
            {
                Name = "Тестовые данные",
                Version = 1.0,
                Items = new[] { "item1", "item2", "item3" },
                Timestamp = DateTime.Now
            };

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(jsonFilePath, json);
            Console.WriteLine($"JSON записан в {jsonFilePath}");
            Console.WriteLine("Содержимое JSON:");
            Console.WriteLine(json);
        }

        static void Task5_Xml()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 5: XML ---");
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(xmlDeclaration);

            XmlElement root = doc.CreateElement("Root");
            doc.AppendChild(root);

            XmlElement element = doc.CreateElement("Element");
            element.SetAttribute("id", "1");
            element.InnerText = "Значение элемента";
            root.AppendChild(element);

            doc.Save(xmlFilePath);
            Console.WriteLine($"XML записан в {xmlFilePath}");
            Console.WriteLine("Содержимое XML:");
            Console.WriteLine(doc.OuterXml);
        }

        static void Task6_Csv()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 6: CSV ---");
            using (StreamWriter sw = new StreamWriter(csvFilePath))
            {
                sw.WriteLine("ID,Имя,Возраст,Город");
                sw.WriteLine("1,Иван,25,Москва");
                sw.WriteLine("2,Мария,30,Санкт-Петербург");
                sw.WriteLine("3,Петр,28,Казань");
            }
            Console.WriteLine($"CSV записан в {csvFilePath}");

            Console.WriteLine("Содержимое CSV:");
            string[] lines = File.ReadAllLines(csvFilePath);
            foreach (string line in lines)
            {
                string[] fields = line.Split(',');
                Console.WriteLine(string.Join(" | ", fields));
            }
        }

        static void Task7_Zip()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 7: ZIP ---");
            string[] filesToZip = { textFilePath, jsonFilePath, xmlFilePath, csvFilePath };

            if (File.Exists(zipFilePath)) File.Delete(zipFilePath);

            using (ZipArchive archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (string file in filesToZip)
                {
                    if (File.Exists(file))
                    {
                        archive.CreateEntryFromFile(file, Path.GetFileName(file));
                        Console.WriteLine($"Добавлен в архив: {file}");
                    }
                }
            }
            Console.WriteLine($"ZIP архив создан: {zipFilePath}");
        }

        static void Task8_Tar()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 8: TAR ---");
            string tarContent = "TAR Archive Emulation\n";
            tarContent += $"Created: {DateTime.Now}\n";
            tarContent += "Files:\n";

            foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.txt"))
            {
                FileInfo fi = new FileInfo(file);
                tarContent += $"- {fi.Name} ({fi.Length} bytes)\n";
            }

            File.WriteAllText(tarFilePath, tarContent);
            Console.WriteLine($"TAR файл создан: {tarFilePath}");
        }

        static void Task9_Iso()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 9: ISO/IMG ---");
            using (FileStream fs = new FileStream(isoFilePath, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write(Encoding.ASCII.GetBytes("DISK IMAGE"));
                writer.Write(DateTime.Now.Ticks);
                writer.Write(Directory.GetFiles(Directory.GetCurrentDirectory()).Length);
            }
            Console.WriteLine($"Образ диска создан: {isoFilePath}");
        }

        static void Task10_Pdf()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 10: PDF ---");
            string pdfContent = @"%PDF-1.4
1 0 obj
<< /Type /Catalog /Pages 2 0 R >>
endobj
2 0 obj
<< /Type /Pages /Kids [3 0 R] /Count 1 >>
endobj
3 0 obj
<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R >>
endobj
4 0 obj
<< /Length 51 >>
stream
BT /F1 24 Tf 100 700 Td (Тестовый PDF документ) Tj ET
endstream
endobj
xref
0 5
0000000000 65535 f 
0000000015 00000 n 
0000000060 00000 n 
0000000115 00000 n 
0000000210 00000 n 
trailer
<< /Size 5 /Root 1 0 R >>
startxref
300
%%EOF";

            File.WriteAllText(pdfFilePath, pdfContent);
            Console.WriteLine($"PDF файл создан: {pdfFilePath}");
        }

        static void Task11_AudioVideo()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 11: Аудио/видео ---");
            byte[] audioHeader = Encoding.ASCII.GetBytes("ID3");
            File.WriteAllBytes(audioFilePath, audioHeader);
            Console.WriteLine($"Аудио файл создан: {audioFilePath}");

            byte[] videoHeader = Encoding.ASCII.GetBytes("ftypmp4");
            File.WriteAllBytes(videoFilePath, videoHeader);
            Console.WriteLine($"Видео файл создан: {videoFilePath}");
        }

        static void Task12_Fonts()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 12: Шрифты ---");
            byte[] fontData = new byte[1024];
            Encoding.ASCII.GetBytes("TrueType Font").CopyTo(fontData, 0);
            File.WriteAllBytes(fontFilePath, fontData);
            Console.WriteLine($"Файл шрифта создан: {fontFilePath}");
        }

        static void Task13_Images()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 13: Изображения ---");
            using (SixLabors.ImageSharp.Image<Rgba32> image = new SixLabors.ImageSharp.Image<Rgba32>(100, 100))
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        image[x, y] = new Rgba32((byte)x, (byte)y, (byte)((x + y) / 2));
                    }
                }
                image.Save(imageFilePath);
            }
            Console.WriteLine($"Изображение создано: {imageFilePath}");
        }

        static void Task14_Database()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 14: Базы данных ---");
            if (File.Exists(dbFilePath)) File.Delete(dbFilePath);

            using (var connection = new SqliteConnection($"Data Source={dbFilePath}"))
            {
                connection.Open();

                var createCmd = connection.CreateCommand();
                createCmd.CommandText = "CREATE TABLE users (id INTEGER PRIMARY KEY, name TEXT, age INTEGER)";
                createCmd.ExecuteNonQuery();

                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = "INSERT INTO users (name, age) VALUES ('Alice', 30), ('Bob', 25)";
                insertCmd.ExecuteNonQuery();

                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT * FROM users";

                using (var reader = selectCmd.ExecuteReader())
                {
                    Console.WriteLine("Данные из базы:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader["id"]}, Имя: {reader["name"]}, Возраст: {reader["age"]}");
                    }
                }
            }
            Console.WriteLine($"База данных сохранена: {dbFilePath}");
        }

        static void Task15_Configs()
        {
            Console.WriteLine("\n--- ЗАДАНИЕ 15: INI/YAML ---");

            string iniContent = @"[AppSettings]
Name=FileIO Practice
Version=1.0
Language=ru";

            File.WriteAllText(iniFilePath, iniContent);
            Console.WriteLine($"INI файл создан: {iniFilePath}");

            var config = new
            {
                app = new
                {
                    name = "FileIO Practice",
                    version = "1.0"
                },
                user = new
                {
                    theme = "dark"
                }
            };

            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            string yaml = serializer.Serialize(config);
            File.WriteAllText(yamlFilePath, yaml);
            Console.WriteLine($"YAML файл создан: {yamlFilePath}");

            Console.WriteLine("INI содержимое:");
            Console.WriteLine(File.ReadAllText(iniFilePath));
            Console.WriteLine("YAML содержимое:");
            Console.WriteLine(File.ReadAllText(yamlFilePath));
        }
    }
}