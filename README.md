# MealPlanner

A console-based meal planning application built with C# and .NET 10.

## Features

- Manage pantry items and track expiration dates
- Create and manage recipes with ingredients
- Build weekly meal plans
- Generate shopping lists from your weekly meal plan

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Running from Source

```powershell
dotnet run
```

## Deployment

### Building a Release

1. Publish the application:

```powershell
dotnet publish -c Release -r win-x64 --self-contained false -o ./publish
```

2. Zip the `publish` folder contents.

3. To run the published application:

```powershell
dotnet MealPlanner.dll
```

## Usage

Use the numbered menus to navigate between:
- **Pantry** — add, view, and remove pantry items; check expiration dates
- **Recipes** — create and manage recipes
- **Meal Plan** — schedule meals for the week
- **Shopping List** — auto-generate a shopping list from your weekly meal plan
