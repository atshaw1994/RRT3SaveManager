# RRT3SaveManager

A simple Avalonia-based save game manager for Railroad Tycoon 3 (RRT3).

## Features
- Select and persist a save game folder (remembered across sessions)
- List `.gmc` save game files in the selected folder
- Rename save game files inline (double-click to edit, confirm/cancel)
- Duplicate save game files (automatically avoids name collisions)
- Delete save game files, with an animated drop-down confirmation dialog
- Fluent/Material icons for the UI via FluentIcons.Avalonia

## Usage
1. **Build and run:**
   ```bash
   dotnet build
   dotnet run --project RailroadTycoon3SaveManager.Desktop
   ```
2. **Select your save game folder** using the browse button at the top.
3. **Manage your save files:**
   - Hover over a file to reveal the duplicate/delete buttons
   - Double-click a file to rename it
   - Use the check/cancel buttons to confirm or cancel renaming
   - Confirm deletion in the drop-down alert that appears

## Requirements
- .NET 10.0 SDK or later
- Avalonia UI 11.x
- CommunityToolkit.Mvvm

## Project Structure
This is split into a shared UI project and a desktop launcher:
- `RailroadTycoon3SaveManager/` — Core Avalonia application project
  - `App.axaml`, `App.axaml.cs` — Application entry point and resources
  - `ViewLocator.cs` — MVVM view resolution
  - `Views/` — Avalonia XAML views (`MainWindow`, `SaveGameControl`, `DropDownAlert`)
  - `ViewModels/` — MVVM logic (`MainViewModel`, `SaveGameViewModel`)
  - `Models/` — Data models (`SaveGame`)
  - `Services/` — App settings persistence (`AppSettings`)
  - `Helpers/` — Value converters (corner radius, path display formatting)
  - `Assets/` — Icons and other resources
- `RailroadTycoon3SaveManager.Desktop/` — Desktop entry point (`Program.cs`) and app manifest

## Development
- All UI logic follows the MVVM pattern
- Uses `ObservableCollection<SaveGameViewModel>` for file lists
- Settings (last used save directory) are persisted as JSON via `AppSettings`
- Custom converters for conditional UI
- No automated test project currently exists

## License
MIT