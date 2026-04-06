using Xunit;
using System.Linq;

namespace MealPlanner.Tests;

public class PantryTests : IDisposable
{
    public PantryTests()
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
    public void AddPantryItem_Works()
    {
        // Arrange
        var item = new PantryItem
        {
            Id = 1,
            Name = "Milk",
            Quantity = 1,
            Unit = "gallon",
            ExpirationDate = DateTime.Now.AddDays(7)
        };

        // Act
        DataStore.Data.PantryItems.Add(item);

        // Assert
        Assert.Single(DataStore.Data.PantryItems);
        Assert.Equal("Milk", DataStore.Data.PantryItems[0].Name);
        Assert.Equal(1, DataStore.Data.PantryItems[0].Quantity);
        Assert.Equal("gallon", DataStore.Data.PantryItems[0].Unit);
    }

    [Fact]
    public void UpdatePantryItem_Works()
    {
        // Arrange
        var item = new PantryItem
        {
            Id = 1,
            Name = "Milk",
            Quantity = 1,
            Unit = "gallon",
            ExpirationDate = DateTime.Now.AddDays(7)
        };
        DataStore.Data.PantryItems.Add(item);

        // Act
        item.Quantity = 0.5;
        item.Unit = "liters";

        // Assert
        Assert.Equal(0.5, DataStore.Data.PantryItems[0].Quantity);
        Assert.Equal("liters", DataStore.Data.PantryItems[0].Unit);
    }

    [Fact]
    public void RemovePantryItem_Works()
    {
        // Arrange
        var item = new PantryItem
        {
            Id = 1,
            Name = "Milk",
            Quantity = 1,
            Unit = "gallon"
        };
        DataStore.Data.PantryItems.Add(item);

        // Act
        DataStore.Data.PantryItems.Remove(item);

        // Assert
        Assert.Empty(DataStore.Data.PantryItems);
    }
}