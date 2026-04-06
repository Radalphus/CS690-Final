using Xunit;
using System.IO;
using System.Text.Json;

namespace MealPlanner.Tests;

public class DataStoreTests : IDisposable
{
    public DataStoreTests()
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
    public void DataStore_InitializesCorrectly()
    {
        // Arrange & Act - create fresh instance
        var testData = new AppData();

        // Assert
        Assert.NotNull(testData);
        Assert.NotNull(testData.PantryItems);
        Assert.NotNull(testData.Recipes);
        Assert.NotNull(testData.MealPlan);
        Assert.NotNull(testData.ShoppingList);
        Assert.Equal(1, testData.NextPantryId);
        Assert.Equal(1, testData.NextRecipeId);
    }
}