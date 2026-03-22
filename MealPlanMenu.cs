namespace MealPlanner;

// UC2: Plan Weekly Meals
// FR3: Create a weekly meal plan
// FR4: Link recipes to meal plans
public static class MealPlanMenu
{
    private static readonly string[] Days =
        { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

    private static readonly string[] MealTypes = { "Breakfast", "Lunch", "Dinner" };

    public static void Show()
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            Helpers.PrintHeader("PLAN WEEKLY MEALS");
            Console.WriteLine("  1. View Meal Plan");
            Console.WriteLine("  2. Edit Meal Plan");
            Console.WriteLine("  3. Return to Main Menu");
            Helpers.PrintDivider();
            Console.Write("  Select an option: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": ViewMealPlan(); break;
                case "2": EditMealPlan(); break;
                case "3": back = true; break;
                default: Helpers.PressAnyKey("  Invalid option."); break;
            }
        }
    }

    private static void ViewMealPlan()
    {
        Console.Clear();
        Helpers.PrintHeader("WEEKLY MEAL PLAN");
        DisplayMealPlan();
        Helpers.PressAnyKey();
    }

    public static void DisplayMealPlan()
    {
        var plan = DataStore.Data.MealPlan;
        if (plan.Count == 0)
        {
            Console.WriteLine("  (No meals planned yet)");
            return;
        }

        foreach (var day in Days)
        {
            var dayMeals = plan.Where(m => m.Day == day).OrderBy(m => Array.IndexOf(MealTypes, m.MealType)).ToList();
            if (dayMeals.Count == 0) continue;

            Console.WriteLine($"\n  {day.ToUpper()}");
            Helpers.PrintDivider();
            foreach (var meal in dayMeals)
            {
                Console.WriteLine($"    {meal.MealType,-12}: {meal.MealName}");
                if (meal.Ingredients.Count > 0)
                {
                    string ings = string.Join(", ", meal.Ingredients.Select(i =>
                        $"{i.Quantity} {i.Unit} {i.Name}".Trim()));
                    Console.WriteLine($"    {"Ingredients",-12}: {ings}");
                }
            }
        }
    }

    private static void EditMealPlan()
    {
        Console.Clear();
        Helpers.PrintHeader("EDIT MEAL PLAN - SELECT DAY");
        for (int i = 0; i < Days.Length; i++)
            Console.WriteLine($"  {i + 1}. {Days[i]}");
        Helpers.PrintDivider();

        int? dayIndex = Helpers.PickFromList("Select day", Days.Length);
        if (dayIndex == null) return;

        ManageDayMeals(Days[dayIndex.Value]);
    }

    private static void ManageDayMeals(string day)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            Helpers.PrintHeader($"MEALS FOR {day.ToUpper()}");

            var dayMeals = DataStore.Data.MealPlan
                .Where(m => m.Day == day)
                .OrderBy(m => Array.IndexOf(MealTypes, m.MealType))
                .ToList();

            if (dayMeals.Count == 0)
                Console.WriteLine("  (No meals for this day)");
            else
                for (int i = 0; i < dayMeals.Count; i++)
                    Console.WriteLine($"  {i + 1}. {dayMeals[i].MealType}: {dayMeals[i].MealName}");

            Helpers.PrintDivider();
            Console.WriteLine("  1. Add Meal");
            Console.WriteLine("  2. Update Meal");
            Console.WriteLine("  3. Delete Meal");
            Console.WriteLine("  4. Return");
            Helpers.PrintDivider();
            Console.Write("  Select an option: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": AddMeal(day); break;
                case "2": UpdateMeal(day); break;
                case "3": DeleteMeal(day); break;
                case "4": back = true; break;
                default: Helpers.PressAnyKey("  Invalid option."); break;
            }
        }
    }

    private static void AddMeal(string day)
    {
        Console.Clear();
        Helpers.PrintHeader($"ADD MEAL - {day.ToUpper()}");

        Console.WriteLine("  Select Meal Type:");
        for (int i = 0; i < MealTypes.Length; i++)
            Console.WriteLine($"  {i + 1}. {MealTypes[i]}");

        int? typeIndex = Helpers.PickFromList("Meal type", MealTypes.Length);
        if (typeIndex == null) return;

        string mealType = MealTypes[typeIndex.Value];

        // Prevent duplicates
        bool exists = DataStore.Data.MealPlan.Any(m => m.Day == day && m.MealType == mealType);
        if (exists)
        {
            Helpers.PressAnyKey($"\n  {mealType} for {day} already exists. Use Update to modify it.");
            return;
        }

        string mealName = "";
        int? recipeId = null;
        List<Ingredient> ingredients = new();

        // Offer to link a saved recipe (FR4)
        var recipes = DataStore.Data.Recipes;
        if (recipes.Count > 0)
        {
            Console.Write("\n  Link to a saved recipe? (y/n): ");
            if (Console.ReadLine()?.Trim().ToLower() == "y")
            {
                Console.WriteLine("\n  Saved Recipes:");
                for (int i = 0; i < recipes.Count; i++)
                    Console.WriteLine($"  {i + 1}. {recipes[i].Name}");

                int? rIdx = Helpers.PickFromList("Select recipe", recipes.Count);
                if (rIdx != null)
                {
                    var recipe = recipes[rIdx.Value];
                    recipeId = recipe.Id;
                    mealName = recipe.Name;
                    ingredients = recipe.Ingredients.Select(i => new Ingredient
                    {
                        Name = i.Name, Quantity = i.Quantity, Unit = i.Unit
                    }).ToList();
                }
            }
        }

        if (string.IsNullOrEmpty(mealName))
        {
            mealName = Helpers.Prompt("Meal Name");
            if (string.IsNullOrEmpty(mealName)) { Helpers.PressAnyKey("  Meal name cannot be empty."); return; }

            Console.WriteLine("\n  Enter ingredients (blank name to finish):");
            while (true)
            {
                string ingName = Helpers.Prompt("  Ingredient name");
                if (string.IsNullOrEmpty(ingName)) break;
                double ingQty = Helpers.PromptDouble("  Quantity", 1);
                string ingUnit = Helpers.Prompt("  Unit");
                ingredients.Add(new Ingredient { Name = ingName, Quantity = ingQty, Unit = ingUnit });
            }
        }

        DataStore.Data.MealPlan.Add(new MealEntry
        {
            Day = day,
            MealType = mealType,
            MealName = mealName,
            RecipeId = recipeId,
            Ingredients = ingredients
        });
        DataStore.Save();
        Helpers.PressAnyKey($"\n  {mealType} \"{mealName}\" added for {day}!");
    }

    private static void UpdateMeal(string day)
    {
        var dayMeals = DataStore.Data.MealPlan.Where(m => m.Day == day).ToList();
        if (dayMeals.Count == 0) { Helpers.PressAnyKey("  No meals for this day."); return; }

        Console.Clear();
        Helpers.PrintHeader($"UPDATE MEAL - {day.ToUpper()}");
        for (int i = 0; i < dayMeals.Count; i++)
            Console.WriteLine($"  {i + 1}. {dayMeals[i].MealType}: {dayMeals[i].MealName}");
        Helpers.PrintDivider();

        int? index = Helpers.PickFromList("Select meal to update", dayMeals.Count);
        if (index == null) return;

        var meal = dayMeals[index.Value];

        Console.Write($"\n  Meal Name [{meal.MealName}]: ");
        string? newName = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(newName)) meal.MealName = newName;

        Console.Write("\n  Update ingredients? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            meal.Ingredients.Clear();
            meal.RecipeId = null;
            Console.WriteLine("  Enter new ingredients (blank name to finish):");
            while (true)
            {
                string ingName = Helpers.Prompt("  Ingredient name");
                if (string.IsNullOrEmpty(ingName)) break;
                double ingQty = Helpers.PromptDouble("  Quantity", 1);
                string ingUnit = Helpers.Prompt("  Unit");
                meal.Ingredients.Add(new Ingredient { Name = ingName, Quantity = ingQty, Unit = ingUnit });
            }
        }

        DataStore.Save();
        Helpers.PressAnyKey($"\n  \"{meal.MealName}\" updated!");
    }

    private static void DeleteMeal(string day)
    {
        var dayMeals = DataStore.Data.MealPlan.Where(m => m.Day == day).ToList();
        if (dayMeals.Count == 0) { Helpers.PressAnyKey("  No meals for this day."); return; }

        Console.Clear();
        Helpers.PrintHeader($"DELETE MEAL - {day.ToUpper()}");
        for (int i = 0; i < dayMeals.Count; i++)
            Console.WriteLine($"  {i + 1}. {dayMeals[i].MealType}: {dayMeals[i].MealName}");
        Helpers.PrintDivider();

        int? index = Helpers.PickFromList("Select meal to delete", dayMeals.Count);
        if (index == null) return;

        var meal = dayMeals[index.Value];
        Console.Write($"\n  Delete \"{meal.MealName}\"? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            DataStore.Data.MealPlan.Remove(meal);
            DataStore.Save();
            Helpers.PressAnyKey($"  \"{meal.MealName}\" removed.");
        }
    }
}
