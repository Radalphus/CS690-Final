using System.Text.Json;

namespace MealPlanner;

public static class DataStore
{
    private static readonly string DataPath = "data.json";
    public static AppData Data { get; private set; } = new();

    public static void Initialize()
    {
        if (File.Exists(DataPath))
        {
            try
            {
                string json = File.ReadAllText(DataPath);
                Data = JsonSerializer.Deserialize<AppData>(json) ?? new AppData();
            }
            catch
            {
                Data = new AppData();
            }
        }
    }

    public static void Save()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(Data, options);
        File.WriteAllText(DataPath, json);
    }
}
