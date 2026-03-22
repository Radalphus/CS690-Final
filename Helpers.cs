namespace MealPlanner;

public static class Helpers
{
    private const int Width = 52;

    public static void PrintHeader(string title)
    {
        string line = new string('=', Width);
        Console.WriteLine(line);
        Console.WriteLine($"  {title}");
        Console.WriteLine(line);
    }

    public static void PrintDivider()
    {
        Console.WriteLine(new string('-', Width));
    }

    public static void PressAnyKey(string? message = null)
    {
        if (message != null) Console.WriteLine(message);
        Console.Write("\nPress any key to continue...");
        Console.ReadKey();
    }

    public static string Prompt(string label)
    {
        Console.Write($"  {label}: ");
        return Console.ReadLine()?.Trim() ?? "";
    }

    public static double PromptDouble(string label, double defaultVal = 1)
    {
        Console.Write($"  {label}: ");
        string? input = Console.ReadLine()?.Trim();
        return double.TryParse(input, out double result) ? result : defaultVal;
    }

    public static DateTime? PromptDate(string label)
    {
        Console.Write($"  {label} (MM/DD/YYYY, blank to skip): ");
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(input)) return null;
        if (DateTime.TryParse(input, out DateTime date)) return date;
        Console.WriteLine("  Invalid date, skipping.");
        return null;
    }

    public static int? PickFromList(string prompt, int count)
    {
        Console.Write($"  {prompt} (1-{count}, 0 to cancel): ");
        if (int.TryParse(Console.ReadLine()?.Trim(), out int choice))
        {
            if (choice == 0) return null;
            if (choice >= 1 && choice <= count) return choice - 1;
        }
        Console.WriteLine("  Invalid selection.");
        return null;
    }
}
