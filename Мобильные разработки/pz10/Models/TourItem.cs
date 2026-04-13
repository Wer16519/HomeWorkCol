namespace pz10.Models;

public class TourItem
{
    private static readonly Random random = new Random();

    public string Title { get; set; }
    public string Location { get; set; }
    public string Date { get; set; }
    public string ImageSource { get; set; }
    public string Price { get; set; }
    public List<string> AlternativeDates { get; set; }

    public TourItem()
    {
        Price = $"{random.Next(500, 1000)},{random.Next(0, 99):D2}";
        Date = GenerateRandomDate();
        AlternativeDates = GenerateAlternativeDates();
    }

    private string GenerateRandomDate()
    {
        int year = random.Next(2026, 2027);
        int month = random.Next(1, 13);
        int day = random.Next(1, DateTime.DaysInMonth(year, month) + 1);

        return $"{year}-{month:D2}-{day:D2}";
    }

    private List<string> GenerateAlternativeDates()
    {
        List<string> dates = new List<string>();
        DateTime baseDate = DateTime.Parse(Date);

        for (int i = 0; i < 3; i++)
        {
            DateTime newDate = baseDate.AddDays(random.Next(7, 30));
            dates.Add($"{newDate.Year}-{newDate.Month:D2}-{newDate.Day:D2}");
        }

        return dates;
    }
}