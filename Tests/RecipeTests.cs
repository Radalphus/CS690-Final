using Xunit;
using System.Linq;

namespace MealPlanner.Tests;

public class RecipeTests : IDisposable
{
    public RecipeTests()
    {
        // Start with fresh data for each test (don't load from disk)
        DataStore.Data = new AppData();
    }

    public void Dispose()
    {
        // Clean up after each test
        DataStore.Data = new AppData();
    }

    [Fact]
    public void AddRecipe_Works()
    {
        // Arrange
        var recipe = new Recipe
        {
            Id = 1,
            Name = "Pasta",
            Ingredients = new List<Ingredient>
            {
                new Ingredient { Name = "Pasta", Quantity = 8, Unit = "oz" },
                new Ingredient { Name = "Tomato Sauce", Quantity = 1, Unit = "jar" }
            },
            Instructions = "Boil pasta, heat sauce, mix together."
        };

        // Act
        DataStore.Data.Recipes.Add(recipe);

        // Assert
        Assert.Single(DataStore.Data.Recipes);
        Assert.Equal("Pasta", DataStore.Data.Recipes[0].Name);
        Assert.Equal(2, DataStore.Data.Recipes[0].Ingredients.Count);
        Assert.Contains(DataStore.Data.Recipes[0].Ingredients, i => i.Name == "Pasta");
        Assert.Contains(DataStore.Data.Recipes[0].Ingredients, i => i.Name == "Tomato Sauce");
    }

    [Fact]
    public void SearchRecipes_Works()
    {
        // Arrange
        var recipe1 = new Recipe
        {
            Id = 1,
            Name = "Pasta Carbonara",
            Ingredients = new List<Ingredient> { new Ingredient { Name = "Pasta", Quantity = 8, Unit = "oz" } },
            Instructions = "Cook pasta."
        };
        var recipe2 = new Recipe
        {
            Id = 2,
            Name = "Chicken Stir Fry",
            Ingredients = new List<Ingredient> { new Ingredient { Name = "Chicken", Quantity = 1, Unit = "lb" } },
            Instructions = "Stir fry chicken."
        };
        DataStore.Data.Recipes.Add(recipe1);
        DataStore.Data.Recipes.Add(recipe2);

        // Act
        var pastaResults = DataStore.Data.Recipes.Where(r =>
            r.Name.Contains("Pasta", StringComparison.OrdinalIgnoreCase) ||
            r.Ingredients.Any(i => i.Name.Contains("Pasta", StringComparison.OrdinalIgnoreCase))
        ).ToList();

        var chickenResults = DataStore.Data.Recipes.Where(r =>
            r.Name.Contains("Chicken", StringComparison.OrdinalIgnoreCase) ||
            r.Ingredients.Any(i => i.Name.Contains("Chicken", StringComparison.OrdinalIgnoreCase))
        ).ToList();

        // Assert
        Assert.Single(pastaResults);
        Assert.Equal("Pasta Carbonara", pastaResults[0].Name);
        Assert.Single(chickenResults);
        Assert.Equal("Chicken Stir Fry", chickenResults[0].Name);
    }
}