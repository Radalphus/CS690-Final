namespace MealPlanner;

public class PantryItem
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public double Quantity { get; set; }
    public string Unit { get; set; } = "";
    public DateTime? ExpirationDate { get; set; }
}

public class Ingredient
{
    public string Name { get; set; } = "";
    public double Quantity { get; set; }
    public string Unit { get; set; } = "";
}

public class Recipe
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<Ingredient> Ingredients { get; set; } = new();
    public string Instructions { get; set; } = "";
}

public class MealEntry
{
    public string Day { get; set; } = "";
    public string MealType { get; set; } = "";
    public string MealName { get; set; } = "";
    public int? RecipeId { get; set; }
    public List<Ingredient> Ingredients { get; set; } = new();
}

public class ShoppingItem
{
    public string Name { get; set; } = "";
    public double Quantity { get; set; }
    public string Unit { get; set; } = "";
}

public class AppData
{
    public List<PantryItem> PantryItems { get; set; } = new();
    public List<Recipe> Recipes { get; set; } = new();
    public List<MealEntry> MealPlan { get; set; } = new();
    public List<ShoppingItem> ShoppingList { get; set; } = new();
    public int NextPantryId { get; set; } = 1;
    public int NextRecipeId { get; set; } = 1;
}
