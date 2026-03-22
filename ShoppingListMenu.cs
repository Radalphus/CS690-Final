namespace MealPlanner;

// UC3: Generate Shopping List
// FR5: Automatically generate shopping list from meal plan
// FR6: Exclude pantry items from shopping list
public static class ShoppingListMenu
{
    public static void Show()
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            Helpers.PrintHeader("GENERATE SHOPPING LIST");
            Console.WriteLine("  1. View Shopping List");
            Console.WriteLine("  2. Generate Shopping List from Meal Plan");
            Console.WriteLine("  3. Clear Shopping List");
            Console.WriteLine("  4. Return to Main Menu");
            Helpers.PrintDivider();
            Console.Write("  Select an option: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": ViewShoppingList(); break;
                case "2": GenerateShoppingList(); break;
                case "3": ClearShoppingList(); break;
                case "4": back = true; break;
                default: Helpers.PressAnyKey("  Invalid option."); break;
            }
        }
    }

    private static void ViewShoppingList()
    {
        Console.Clear();
        Helpers.PrintHeader("SHOPPING LIST");
        DisplayShoppingList();
        Helpers.PressAnyKey();
    }

    private static void DisplayShoppingList()
    {
        var list = DataStore.Data.ShoppingList;
        if (list.Count == 0)
        {
            Console.WriteLine("  (Shopping list is empty)");
            Console.WriteLine("  Use option 2 to generate from your meal plan.");
            return;
        }

        Console.WriteLine($"  {"#",-4} {"Item",-25} {"Qty",-7} {"Unit",-10}");
        Helpers.PrintDivider();
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            Console.WriteLine($"  {i + 1,-4} {item.Name,-25} {item.Quantity,-7} {item.Unit,-10}");
        }
        Console.WriteLine($"\n  Total: {list.Count} item(s)");
    }

    private static void GenerateShoppingList()
    {
        Console.Clear();
        Helpers.PrintHeader("GENERATING SHOPPING LIST");

        var mealPlan = DataStore.Data.MealPlan;
        if (mealPlan.Count == 0)
        {
            Helpers.PressAnyKey("  No meals in plan. Add meals first (Main Menu > 2).");
            return;
        }

        // FR5: Aggregate all ingredients from the weekly meal plan
        Console.WriteLine("  [1/3] Scanning weekly meal plan...");
        var needed = new Dictionary<string, (double qty, string unit)>(StringComparer.OrdinalIgnoreCase);

        foreach (var meal in mealPlan)
        {
            foreach (var ing in meal.Ingredients)
            {
                string key = ing.Name.Trim();
                if (needed.TryGetValue(key, out var existing))
                    needed[key] = (existing.qty + ing.Quantity, existing.unit);
                else
                    needed[key] = (ing.Quantity, ing.Unit);
            }
        }

        int totalNeeded = needed.Count;
        Console.WriteLine($"  Found {totalNeeded} unique ingredient(s) across {mealPlan.Count} meal(s).");

        // FR6: Compare against pantry and exclude what's covered
        Console.WriteLine("\n  [2/3] Checking pantry inventory...");
        var pantry = DataStore.Data.PantryItems;
        var shoppingItems = new List<ShoppingItem>();
        int excludedCount = 0;
        int partialCount = 0;

        foreach (var (ingName, (ingQty, ingUnit)) in needed)
        {
            var pantryMatch = pantry.FirstOrDefault(p =>
                p.Name.Contains(ingName, StringComparison.OrdinalIgnoreCase) ||
                ingName.Contains(p.Name, StringComparison.OrdinalIgnoreCase));

            if (pantryMatch != null && pantryMatch.Quantity >= ingQty)
            {
                Console.WriteLine($"  [HAVE]    {ingName} ({pantryMatch.Quantity} {pantryMatch.Unit} in pantry)");
                excludedCount++;
            }
            else if (pantryMatch != null && pantryMatch.Quantity < ingQty)
            {
                double stillNeed = Math.Round(ingQty - pantryMatch.Quantity, 2);
                Console.WriteLine($"  [PARTIAL] {ingName} - need {stillNeed} more {ingUnit} (have {pantryMatch.Quantity})");
                shoppingItems.Add(new ShoppingItem { Name = ingName, Quantity = stillNeed, Unit = ingUnit });
                partialCount++;
            }
            else
            {
                Console.WriteLine($"  [NEED]    {ingName} - {ingQty} {ingUnit}");
                shoppingItems.Add(new ShoppingItem { Name = ingName, Quantity = ingQty, Unit = ingUnit });
            }
        }

        // Save results
        Console.WriteLine("\n  [3/3] Saving shopping list...");
        DataStore.Data.ShoppingList = shoppingItems;
        DataStore.Save();

        Console.WriteLine("\n  ----------------------------------------");
        Console.WriteLine($"  Total ingredients needed : {totalNeeded}");
        Console.WriteLine($"  Already in pantry        : {excludedCount}");
        Console.WriteLine($"  Partially in pantry      : {partialCount}");
        Console.WriteLine($"  Items to buy             : {shoppingItems.Count}");
        Console.WriteLine("  ----------------------------------------");

        if (shoppingItems.Count == 0)
            Console.WriteLine("\n  Great! You already have everything you need!");
        else
            Console.WriteLine($"\n  Shopping list ready with {shoppingItems.Count} item(s)!");

        Helpers.PressAnyKey();
    }

    private static void ClearShoppingList()
    {
        if (DataStore.Data.ShoppingList.Count == 0)
        {
            Helpers.PressAnyKey("  Shopping list is already empty.");
            return;
        }
        Console.Write("\n  Clear shopping list? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            DataStore.Data.ShoppingList.Clear();
            DataStore.Save();
            Helpers.PressAnyKey("  Shopping list cleared.");
        }
    }
}
