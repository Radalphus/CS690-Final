namespace MealPlanner;

// UC1: Manage Pantry Inventory
// FR1: Add items to pantry inventory
// FR2: Update and remove pantry items
// FR9: Add expiration dates to pantry items
public static class PantryMenu
{
    public static void Show()
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            Helpers.PrintHeader("MANAGE PANTRY INVENTORY");
            DisplayPantry();
            Helpers.PrintDivider();
            Console.WriteLine("  1. Add Pantry Item");
            Console.WriteLine("  2. Update / Remove Item");
            Console.WriteLine("  3. Return to Main Menu");
            Helpers.PrintDivider();
            Console.Write("  Select an option: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": AddItem(); break;
                case "2": SelectAndManageItem(); break;
                case "3": back = true; break;
                default: Helpers.PressAnyKey("  Invalid option."); break;
            }
        }
    }

    public static void DisplayPantry()
    {
        var items = DataStore.Data.PantryItems;
        if (items.Count == 0)
        {
            Console.WriteLine("  (Pantry is empty)");
            return;
        }

        Console.WriteLine($"  {"#",-4} {"Name",-20} {"Qty",-7} {"Unit",-10} {"Expires",-12}");
        Helpers.PrintDivider();
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            string exp = item.ExpirationDate.HasValue
                ? item.ExpirationDate.Value.ToString("MM/dd/yyyy")
                : "N/A";
            Console.WriteLine($"  {i + 1,-4} {item.Name,-20} {item.Quantity,-7} {item.Unit,-10} {exp,-12}");
        }
    }

    private static void AddItem()
    {
        Console.Clear();
        Helpers.PrintHeader("ADD PANTRY ITEM");

        string name = Helpers.Prompt("Item Name");
        if (string.IsNullOrEmpty(name)) { Helpers.PressAnyKey("  Name cannot be empty."); return; }

        double qty = Helpers.PromptDouble("Quantity", 1);
        string unit = Helpers.Prompt("Unit (e.g. lbs, oz, count)");
        DateTime? expDate = Helpers.PromptDate("Expiration Date");

        DataStore.Data.PantryItems.Add(new PantryItem
        {
            Id = DataStore.Data.NextPantryId++,
            Name = name,
            Quantity = qty,
            Unit = unit,
            ExpirationDate = expDate
        });
        DataStore.Save();
        Helpers.PressAnyKey($"\n  \"{name}\" added to pantry!");
    }

    private static void SelectAndManageItem()
    {
        var items = DataStore.Data.PantryItems;
        if (items.Count == 0) { Helpers.PressAnyKey("  Pantry is empty."); return; }

        Console.Clear();
        Helpers.PrintHeader("SELECT PANTRY ITEM");
        DisplayPantry();
        Helpers.PrintDivider();

        int? index = Helpers.PickFromList("Select item number", items.Count);
        if (index == null) return;

        var item = items[index.Value];
        bool back = false;
        while (!back)
        {
            Console.Clear();
            Helpers.PrintHeader($"ITEM: {item.Name.ToUpper()}");
            Console.WriteLine($"  Name     : {item.Name}");
            Console.WriteLine($"  Quantity : {item.Quantity} {item.Unit}");
            Console.WriteLine($"  Expires  : {(item.ExpirationDate.HasValue ? item.ExpirationDate.Value.ToString("MM/dd/yyyy") : "N/A")}");
            Helpers.PrintDivider();
            Console.WriteLine("  1. Update Item");
            Console.WriteLine("  2. Delete Item");
            Console.WriteLine("  3. Return");
            Helpers.PrintDivider();
            Console.Write("  Select an option: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": UpdateItem(item); back = true; break;
                case "2": if (DeleteItem(item)) back = true; break;
                case "3": back = true; break;
                default: Helpers.PressAnyKey("  Invalid option."); break;
            }
        }
    }

    private static void UpdateItem(PantryItem item)
    {
        Console.Clear();
        Helpers.PrintHeader($"UPDATE: {item.Name}");
        Console.WriteLine("  (Press Enter to keep current value)\n");

        Console.Write($"  Name [{item.Name}]: ");
        string? name = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(name)) item.Name = name;

        Console.Write($"  Quantity [{item.Quantity}]: ");
        string? qtyStr = Console.ReadLine()?.Trim();
        if (double.TryParse(qtyStr, out double qty)) item.Quantity = qty;

        Console.Write($"  Unit [{item.Unit}]: ");
        string? unit = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(unit)) item.Unit = unit;

        string expDisplay = item.ExpirationDate.HasValue
            ? item.ExpirationDate.Value.ToString("MM/dd/yyyy") : "N/A";
        Console.Write($"  Expiration Date [{expDisplay}] (MM/DD/YYYY, blank to keep): ");
        string? expStr = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(expStr) && DateTime.TryParse(expStr, out DateTime expDate))
            item.ExpirationDate = expDate;

        DataStore.Save();
        Helpers.PressAnyKey($"\n  \"{item.Name}\" updated!");
    }

    private static bool DeleteItem(PantryItem item)
    {
        Console.Write($"\n  Delete \"{item.Name}\"? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            DataStore.Data.PantryItems.Remove(item);
            DataStore.Save();
            Helpers.PressAnyKey($"  \"{item.Name}\" removed from pantry.");
            return true;
        }
        return false;
    }
}
