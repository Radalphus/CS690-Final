using MealPlanner;

DataStore.Initialize();

bool exit = false;
while (!exit)
{
    Console.Clear();
    Helpers.PrintHeader("MEAL PLANNER & GROCERY MANAGER");
    Console.WriteLine("  1. Manage Pantry Inventory");
    Console.WriteLine("  2. Plan Weekly Meals");
    Console.WriteLine("  3. Generate Shopping List");
    Console.WriteLine("  4. Search and Save Recipes");
    Console.WriteLine("  5. Track Food Expiration Dates");
    Console.WriteLine("  6. Exit");
    Helpers.PrintDivider();
    Console.Write("  Select an option: ");

    switch (Console.ReadLine()?.Trim())
    {
        case "1": PantryMenu.Show(); break;
        case "2": MealPlanMenu.Show(); break;
        case "3": ShoppingListMenu.Show(); break;
        case "4": RecipeMenu.Show(); break;
        case "5": ExpirationMenu.Show(); break;
        case "6": exit = true; break;
        default: Helpers.PressAnyKey("  Invalid option."); break;
    }
}

Console.WriteLine("\n  Goodbye! Happy cooking!");
