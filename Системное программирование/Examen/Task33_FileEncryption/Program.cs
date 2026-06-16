using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Task33_FileEncryption
{
    public class FileEncryptor
    {
        private const int KeySize = 256;
        private const int BlockSize = 128;
        private const int SaltSize = 16;
        private const int Iterations = 10000;

        public void EncryptFile(string inputFile, string outputFile, string password)
        {
            try
            {
                byte[] salt = new byte[SaltSize];
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                using (FileStream inputStream = new FileStream(inputFile, FileMode.Open))
                using (FileStream outputStream = new FileStream(outputFile, FileMode.Create))
                {
                    outputStream.Write(salt, 0, salt.Length);

                    using (Rfc2898DeriveBytes keyDerivation = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                    {
                        byte[] key = keyDerivation.GetBytes(KeySize / 8);
                        byte[] iv = keyDerivation.GetBytes(BlockSize / 8);

                        using (Aes aes = Aes.Create())
                        {
                            aes.Key = key;
                            aes.IV = iv;
                            aes.Mode = CipherMode.CBC;
                            aes.Padding = PaddingMode.PKCS7;

                            using (CryptoStream cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                inputStream.CopyTo(cryptoStream);
                            }
                        }
                    }
                }

                Console.WriteLine($"[Шифрование] Файл зашифрован: {inputFile} -> {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка шифрования] {ex.Message}");
            }
        }

        public void DecryptFile(string inputFile, string outputFile, string password)
        {
            try
            {
                byte[] salt = new byte[SaltSize];

                using (FileStream inputStream = new FileStream(inputFile, FileMode.Open))
                {
                    inputStream.Read(salt, 0, salt.Length);

                    using (Rfc2898DeriveBytes keyDerivation = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                    {
                        byte[] key = keyDerivation.GetBytes(KeySize / 8);
                        byte[] iv = keyDerivation.GetBytes(BlockSize / 8);

                        using (Aes aes = Aes.Create())
                        {
                            aes.Key = key;
                            aes.IV = iv;
                            aes.Mode = CipherMode.CBC;
                            aes.Padding = PaddingMode.PKCS7;

                            using (FileStream outputStream = new FileStream(outputFile, FileMode.Create))
                            using (CryptoStream cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                            {
                                cryptoStream.CopyTo(outputStream);
                            }
                        }
                    }
                }

                Console.WriteLine($"[Дешифрование] Файл расшифрован: {inputFile} -> {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка дешифрования] {ex.Message}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Шифрование и дешифрование файлов ===\n");

            string originalFile = "original.txt";
            string encryptedFile = "encrypted.dat";
            string decryptedFile = "decrypted.txt";

            string content = "Это секретные данные для шифрования.\n" +
                           "Пароль должен быть надежным.\n" +
                           "Алгоритм AES с 256-битным ключом.";

            File.WriteAllText(originalFile, content, Encoding.UTF8);
            Console.WriteLine($"[Создан] Исходный файл: {originalFile}");
            Console.WriteLine($"Содержимое:\n{content}\n");

            FileEncryptor encryptor = new FileEncryptor();

            string password = "MySuperSecretPassword123!";

            encryptor.EncryptFile(originalFile, encryptedFile, password);

            FileInfo encryptedInfo = new FileInfo(encryptedFile);
            Console.WriteLine($"Размер зашифрованного файла: {encryptedInfo.Length} байт");

            encryptor.DecryptFile(encryptedFile, decryptedFile, password);

            string decryptedContent = File.ReadAllText(decryptedFile, Encoding.UTF8);
            Console.WriteLine($"\n[Проверка] Расшифрованное содержимое:");
            Console.WriteLine(decryptedContent);

            bool isEqual = content == decryptedContent;
            Console.WriteLine($"\n[Результат] Данные совпадают: {isEqual}");

            Console.WriteLine("\n[Завершение] Программа завершена");
            Console.ReadKey();
        }
    }
}