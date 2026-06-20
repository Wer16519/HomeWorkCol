namespace task4
{
    public class Triangle
    {
        private double _a;
        private double _b;
        private double _c;

        public Triangle(double a, double b, double c)
        {
            _a = a;
            _b = b;
            _c = c;
        }

        public double A
        {
            get => _a;
            set => _a = value;
        }

        public double B
        {
            get => _b;
            set => _b = value;
        }

        public double C
        {
            get => _c;
            set => _c = value;
        }

        public bool Exists =>
            _a > 0 && _b > 0 && _c > 0 &&
            _a + _b > _c &&
            _a + _c > _b &&
            _b + _c > _a;

        public void PrintSides()
        {
            Console.WriteLine($"Стороны треугольника: a = {_a}, b = {_b}, c = {_c}");
        }

        public double Perimeter() => _a + _b + _c;

        public double Area()
        {
            if (!Exists)
                throw new InvalidOperationException("Треугольник с такими сторонами не существует.");

            double p = Perimeter() / 2;
            return Math.Sqrt(p * (p - _a) * (p - _b) * (p - _c));
        }
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Демонстрация класса Triangle ===\n");

            Triangle t = new Triangle(3, 4, 5);
            t.PrintSides();

            Console.WriteLine($"Существует ли треугольник: {(t.Exists ? "да" : "нет")}");

            if (t.Exists)
            {
                Console.WriteLine($"Периметр: {t.Perimeter()}");
                Console.WriteLine($"Площадь:  {t.Area():F2}");
            }

            Console.WriteLine("\nИзменим стороны: a=10, b=1, c=1");
            t.A = 10; t.B = 1; t.C = 1;
            t.PrintSides();
            Console.WriteLine($"Существует ли треугольник: {(t.Exists ? "да" : "нет")}");

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}