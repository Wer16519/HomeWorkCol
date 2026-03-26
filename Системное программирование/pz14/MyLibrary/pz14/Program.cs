using MyLibrary;

namespace pz14
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Тестирование библиотеки MyLibrary ===\n");

            Console.WriteLine("Вариант 1: Сложение чисел");
            Console.WriteLine($"15.5 + 25.3 = {Calculator.Add(15.5, 25.3)}\n");

            Console.WriteLine("Вариант 2: Квадратный корень");
            Console.WriteLine($"√144 = {MathFunctions.SquareRoot(144)}\n");

            Console.WriteLine("Вариант 3: Работа с матрицами");
            double[,] m1 = { { 1, 2 }, { 3, 4 } };
            double[,] m2 = { { 5, 6 }, { 7, 8 } };
            Console.WriteLine("Сумма матриц:");
            Console.WriteLine(MatrixOperations.PrintMatrix(MatrixOperations.AddMatrices(m1, m2)));

            Console.WriteLine("Вариант 4: Шифрование");
            string text = "Hello World";
            int key = 3;
            string enc = Encryption.Encrypt(text, key);
            Console.WriteLine($"Исходный: {text} -> Зашифрованный: {enc} -> Расшифрованный: {Encryption.Decrypt(enc, key)}\n");

            Console.WriteLine("Вариант 5: Случайные числа");
            Console.WriteLine($"Случайное число от 1 до 100: {RandomGenerator.GenerateRandomInt(1, 100)}\n");

            Console.WriteLine("Вариант 6: Работа со строками");
            Console.WriteLine($"Поиск 'World' в 'Hello World': {StringOperations.FindSubstring("Hello World", "World")}\n");

            Console.WriteLine("Вариант 7: Работа с файлами");
            string testFile = @"C:\test.txt";
            FileSystemOperations.WriteFile(testFile, "Тест");
            Console.WriteLine($"Файл создан: {testFile}\n");

            Console.WriteLine("Вариант 8: Обработка изображений");
            var img = ImageProcessing.CreateTestImage(40, 20);
            Console.WriteLine("Тестовое изображение:");
            Console.WriteLine(ImageProcessing.ImageToString(img, 40, 20));

            Console.WriteLine("Вариант 9: Работа с БД");
            var db = new DatabaseOperations("test_connection");
            db.ExecuteQuery("SELECT * FROM Users");
            Console.WriteLine();

            Console.WriteLine("Вариант 10: Метод Гаусса");
            double[,] sys = { { 2, 1, -1 }, { -3, -1, 2 }, { -2, 1, 2 } };
            double[] consts = { 8, -11, -3 };
            double[] sol = GaussianElimination.SolveSystem(sys, consts);
            Console.WriteLine($"x = {sol[0]:F2}, y = {sol[1]:F2}, z = {sol[2]:F2}\n");

            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}