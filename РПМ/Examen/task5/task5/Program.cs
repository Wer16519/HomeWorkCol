namespace task5
{
    public abstract class Figure
    {
        public abstract double Area();
        public abstract double Perimeter();

        public virtual void PrintInfo()
        {
            Console.WriteLine($"Фигура: {GetType().Name}");
            Console.WriteLine($"  Периметр: {Perimeter():F2}");
            Console.WriteLine($"  Площадь:  {Area():F2}");
        }
    }

    public class Triangle : Figure
    {
        private readonly double _a, _b, _c;

        public Triangle(double a, double b, double c)
        {
            if (!IsValid(a, b, c))
                throw new ArgumentException("Треугольник с такими сторонами не существует.");
            _a = a; _b = b; _c = c;
        }

        private static bool IsValid(double a, double b, double c) =>
            a > 0 && b > 0 && c > 0 &&
            a + b > c && a + c > b && b + c > a;

        public override double Perimeter() => _a + _b + _c;

        public override double Area()
        {
            double p = Perimeter() / 2;
            return Math.Sqrt(p * (p - _a) * (p - _b) * (p - _c));
        }

        public override void PrintInfo()
        {
            Console.WriteLine($"Треугольник со сторонами: a={_a}, b={_b}, c={_c}");
            Console.WriteLine($"  Периметр: {Perimeter():F2}");
            Console.WriteLine($"  Площадь:  {Area():F2}");
        }
    }

    public class Circle : Figure
    {
        private readonly double _radius;

        public Circle(double radius)
        {
            if (radius <= 0)
                throw new ArgumentException("Радиус должен быть положительным.");
            _radius = radius;
        }

        public override double Perimeter() => 2 * Math.PI * _radius;

        public override double Area() => Math.PI * _radius * _radius;

        public override void PrintInfo()
        {
            Console.WriteLine($"Круг с радиусом: r={_radius}");
            Console.WriteLine($"  Длина окружности: {Perimeter():F2}");
            Console.WriteLine($"  Площадь:          {Area():F2}");
        }
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Figure[] figures =
            {
                new Triangle(3, 4, 5),
                new Circle(5),
                new Triangle(7, 8, 9),
                new Circle(2.5),
                new Triangle(6, 6, 6)
            };

            Console.WriteLine("=== Информация о фигурах ===\n");

            for (int i = 0; i < figures.Length; i++)
            {
                Console.WriteLine($"--- Фигура №{i + 1} ---");
                figures[i].PrintInfo();
                Console.WriteLine();
            }

            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}