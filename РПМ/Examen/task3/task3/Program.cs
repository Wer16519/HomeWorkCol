namespace task3
{
    public class Point
    {
        private double _x;
        private double _y;

        public Point() : this(0, 0) { }

        public Point(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public double X
        {
            get => _x;
            set => _x = value;
        }

        public double Y
        {
            get => _y;
            set => _y = value;
        }

        public double Scale
        {
            set
            {
                _x *= value;
                _y *= value;
            }
        }

        public void Print()
        {
            Console.WriteLine($"Точка: ({_x}; {_y})");
        }

        public double DistanceFromOrigin()
        {
            return Math.Sqrt(_x * _x + _y * _y);
        }

        public void Move(double a, double b)
        {
            _x += a;
            _y += b;
        }
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Демонстрация класса Point ===\n");

            Point p1 = new Point();
            Console.Write("p1 (по умолчанию): ");
            p1.Print();

            Point p2 = new Point(3, 4);
            Console.Write("p2 (заданные координаты): ");
            p2.Print();

            Console.WriteLine($"Расстояние от (0;0) до p2: {p2.DistanceFromOrigin():F2}");

            p2.Move(1, 2);
            Console.Write("p2 после перемещения на (1; 2): ");
            p2.Print();

            p1.X = 5;
            p1.Y = 5;
            Console.Write("p1 после установки координат через свойства: ");
            p1.Print();

            p1.Scale = 2;
            Console.Write("p1 после умножения координат на 2: ");
            p1.Print();

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}