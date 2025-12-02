# RRT3SaveManager

A simple Avalonia-based save game manager for Railroad Tycoon 3 (RRT3).

## Features
- Select and persist a save game folder
- List save game files
- Delete save game files
- Rename save game files
- Material Symbols icons for UI

## Usage
1. **Build and run:**
   ```bash
   dotnet build
   dotnet run
   ```
2. **Select your save game folder** using the button at the top.
3. **Manage your save files:**
   - Hover over a file to reveal the delete button
   - Double-click a file to rename it
   - Use the check/cancel buttons to confirm or cancel renaming

## Requirements
- .NET 9.0 SDK or later
- Avalonia UI 11.x
- CommunityToolkit.Mvvm

## Project Structure
- `App.axaml`, `App.axaml.cs` — Application entry point and resources
- `Views/` — Avalonia XAML views
- `ViewModels/` — MVVM logic and converters
- `Models/` — (optional) Data models
- `Assets/` — Icons and other resources

## Development
- All UI logic follows the MVVM pattern
- Uses ObservableCollection for file lists
- Custom converters for conditional UI

## License
MIT
