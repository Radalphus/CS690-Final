namespace MealPlanner;

// UC4: Search and Save Recipes
// FR7: Add and save recipes
// FR8: View and search saved recipes
public static class RecipeMenu
{
    public static void Show()
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            Helpers.PrintHeader("SEARCH AND SAVE RECIPES");
            Console.WriteLine("  1. View All Recipes");
            Console.WriteLine("  2. Add Recipe");
            Console.WriteLine("  3. Search Recipes");
            Console.WriteLine("  4. Return to Main Menu");
            Helpers.PrintDivider();
            Console.Write("  Select an option: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": ListAndPickRecipe(DataStore.Data.Recipes); break;
                case "2": AddRecipe(); break;
                case "3": SearchRecipes(); break;
                case "4": back = true; break;
                default: Helpers.PressAnyKey("  Invalid option."); break;
            }
        }
    }

    private static void ListAndPickRecipe(List<Recipe> recipes)
    {
        Console.Clear();
        Helpers.PrintHeader("SAVED RECIPES");

        if (recipes.Count == 0)
        {
            Helpers.PressAnyKey("  No recipes saved yet. Use option 2 to add one.");
            return;
        }

        for (int i = 0; i < recipes.Count; i++)
            Console.WriteLine($"  {i + 1}. {recipes[i].Name}");
        Helpers.PrintDivider();

        int? index = Helpers.PickFromList("Select recipe to view", recipes.Count);
        if (index == null) return;

        ShowRecipe(recipes[index.Value]);
    }

    private static void ShowRecipe(Recipe recipe)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            Helpers.PrintHeader($"RECIPE: {recipe.Name.ToUpper()}");

            Console.WriteLine("  INGREDIENTS:");
            if (recipe.Ingredients.Count == 0)
                Console.WriteLine("    (none listed)");
            else
                foreach (var ing in recipe.Ingredients)
                    Console.WriteLine($"    - {ing.Quantity} {ing.Unit} {ing.Name}".TrimEnd());

            Console.WriteLine("\n  INSTRUCTIONS:");
            if (string.IsNullOrWhiteSpace(recipe.Instructions))
                Console.WriteLine("    (none)");
            else
            {
                // Word-wrap long instructions
                foreach (var line in WrapText(recipe.Instructions, 46))
                    Console.WriteLine($"    {line}");
            }

            Helpers.PrintDivider();
            Console.WriteLine("  1. Update Recipe");
            Console.WriteLine("  2. Delete Recipe");
            Console.WriteLine("  3. Return");
            Helpers.PrintDivider();
            Console.Write("  Select an option: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": UpdateRecipe(recipe); break;
                case "2": if (DeleteRecipe(recipe)) back = true; break;
                case "3": back = true; break;
                default: Helpers.PressAnyKey("  Invalid option."); break;
            }
        }
    }

    private static void AddRecipe()
    {
        Console.Clear();
        Helpers.PrintHeader("ADD RECIPE");

        string name = Helpers.Prompt("Recipe Name");
        if (string.IsNullOrEmpty(name)) { Helpers.PressAnyKey("  Name cannot be empty."); return; }

        var ingredients = new List<Ingredient>();
        Console.WriteLine("\n  Enter ingredients (blank name to finish):");
        while (true)
        {
            string ingName = Helpers.Prompt("  Ingredient name");
            if (string.IsNullOrEmpty(ingName)) break;
            double ingQty = Helpers.PromptDouble("  Quantity", 1);
            string ingUnit = Helpers.Prompt("  Unit");
            ingredients.Add(new Ingredient { Name = ingName, Quantity = ingQty, Unit = ingUnit });
        }

        Console.Write("\n  Instructions: ");
        string instructions = Console.ReadLine()?.Trim() ?? "";

        DataStore.Data.Recipes.Add(new Recipe
        {
            Id = DataStore.Data.NextRecipeId++,
            Name = name,
            Ingredients = ingredients,
            Instructions = instructions
        });
        DataStore.Save();
        Helpers.PressAnyKey($"\n  Recipe \"{name}\" saved!");
    }

    private static void UpdateRecipe(Recipe recipe)
    {
        Console.Clear();
        Helpers.PrintHeader($"UPDATE: {recipe.Name}");
        Console.WriteLine("  (Press Enter to keep current value)\n");

        Console.Write($"  Name [{recipe.Name}]: ");
        string? name = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(name)) recipe.Name = name;

        Console.Write("\n  Update ingredients? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            recipe.Ingredients.Clear();
            Console.WriteLine("  Enter new ingredients (blank name to finish):");
            while (true)
            {
                string ingName = Helpers.Prompt("  Ingredient name");
                if (string.IsNullOrEmpty(ingName)) break;
                double ingQty = Helpers.PromptDouble("  Quantity", 1);
                string ingUnit = Helpers.Prompt("  Unit");
                recipe.Ingredients.Add(new Ingredient { Name = ingName, Quantity = ingQty, Unit = ingUnit });
            }
        }

        string instrPreview = recipe.Instructions.Length > 30
            ? recipe.Instructions[..30] + "..." : recipe.Instructions;
        Console.Write($"\n  Instructions [{instrPreview}]: ");
        string? instructions = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(instructions)) recipe.Instructions = instructions;

        DataStore.Save();
        Helpers.PressAnyKey($"\n  Recipe \"{recipe.Name}\" updated!");
    }

    private static bool DeleteRecipe(Recipe recipe)
    {
        Console.Write($"\n  Delete \"{recipe.Name}\"? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            DataStore.Data.Recipes.Remove(recipe);
            DataStore.Save();
            Helpers.PressAnyKey($"  \"{recipe.Name}\" deleted.");
            return true;
        }
        return false;
    }

    private static void SearchRecipes()
    {
        Console.Clear();
        Helpers.PrintHeader("SEARCH RECIPES");

        string query = Helpers.Prompt("Search by name or ingredient");
        if (string.IsNullOrEmpty(query)) { Helpers.PressAnyKey("  Search term cannot be empty."); return; }

        var results = DataStore.Data.Recipes
            .Where(r =>
                r.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                r.Ingredients.Any(i => i.Name.Contains(query, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (results.Count == 0)
        {
            Helpers.PressAnyKey($"  No recipes found matching \"{query}\".");
            return;
        }

        Console.WriteLine($"\n  Found {results.Count} result(s) for \"{query}\":");
        ListAndPickRecipe(results);
    }

    private static IEnumerable<string> WrapText(string text, int maxWidth)
    {
        var words = text.Split(' ');
        var line = new System.Text.StringBuilder();
        foreach (var word in words)
        {
            if (line.Length + word.Length + 1 > maxWidth && line.Length > 0)
            {
                yield return line.ToString();
                line.Clear();
            }
            if (line.Length > 0) line.Append(' ');
            line.Append(word);
        }
        if (line.Length > 0) yield return line.ToString();
    }
}
