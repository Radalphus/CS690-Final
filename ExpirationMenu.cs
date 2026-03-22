namespace MealPlanner;

// UC5: Track Food Expiration Dates
// FR9: Add expiration dates to pantry items (entered via PantryMenu)
// FR10: Display items expiring soon
public static class ExpirationMenu
{
    private const int WarningDays = 7;

    public static void Show()
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            Helpers.PrintHeader("TRACK FOOD EXPIRATION DATES");
            Console.WriteLine("  1. View Items Expiring Soon (within 7 days)");
            Console.WriteLine("  2. View All Items with Expiration Dates");
            Console.WriteLine("  3. Return to Main Menu");
            Helpers.PrintDivider();
            Console.Write("  Select an option: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": ViewExpiringSoon(); break;
                case "2": ViewAllWithDates(); break;
                case "3": back = true; break;
                default: Helpers.PressAnyKey("  Invalid option."); break;
            }
        }
    }

    private static void ViewExpiringSoon()
    {
        Console.Clear();
        Helpers.PrintHeader($"EXPIRING WITHIN {WarningDays} DAYS");

        var today = DateTime.Today;
        var expiring = DataStore.Data.PantryItems
            .Where(p => p.ExpirationDate.HasValue &&
                        (p.ExpirationDate.Value.Date - today).TotalDays <= WarningDays)
            .OrderBy(p => p.ExpirationDate)
            .ToList();

        if (expiring.Count == 0)
        {
            Console.WriteLine("  No items expiring within 7 days.");
        }
        else
        {
            Console.WriteLine($"  {"Item",-20} {"Qty",-7} {"Unit",-10} {"Expires",-12} Status");
            Helpers.PrintDivider();
            foreach (var item in expiring)
            {
                int daysLeft = (int)(item.ExpirationDate!.Value.Date - today).TotalDays;
                string status = daysLeft < 0
                    ? "!! EXPIRED !!"
                    : daysLeft == 0 ? "!! TODAY !!"
                    : $"{daysLeft} day(s) left";
                Console.WriteLine($"  {item.Name,-20} {item.Quantity,-7} {item.Unit,-10} {item.ExpirationDate.Value:MM/dd/yyyy}  {status}");
            }
        }

        Helpers.PressAnyKey();
    }

    private static void ViewAllWithDates()
    {
        Console.Clear();
        Helpers.PrintHeader("ALL ITEMS WITH EXPIRATION DATES");

        var items = DataStore.Data.PantryItems
            .Where(p => p.ExpirationDate.HasValue)
            .OrderBy(p => p.ExpirationDate)
            .ToList();

        if (items.Count == 0)
        {
            Console.WriteLine("  No items have expiration dates set.");
            Console.WriteLine("  Add expiration dates via Manage Pantry Inventory.");
        }
        else
        {
            var today = DateTime.Today;
            Console.WriteLine($"  {"Item",-20} {"Qty",-7} {"Unit",-10} {"Expires",-12} Days Left");
            Helpers.PrintDivider();
            foreach (var item in items)
            {
                int daysLeft = (int)(item.ExpirationDate!.Value.Date - today).TotalDays;
                string dayStr = daysLeft < 0 ? "EXPIRED"
                    : daysLeft == 0 ? "Today"
                    : $"{daysLeft}";
                Console.WriteLine($"  {item.Name,-20} {item.Quantity,-7} {item.Unit,-10} {item.ExpirationDate.Value:MM/dd/yyyy}  {dayStr}");
            }
        }

        Helpers.PressAnyKey();
    }
}
