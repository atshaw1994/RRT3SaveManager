using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using RailroadTycoon3SaveManager.Services;
using RailroadTycoon3SaveManager.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RailroadTycoon3SaveManager.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly AppSettings _appSettings;
    private string _saveDirectory = string.Empty;
    public string SaveDirectory
    {
        get => _saveDirectory;
        set
        {
            _saveDirectory = value;
            OnPropertyChanged(nameof(SaveDirectory));
        }
    }

    public ObservableCollection<SaveGameViewModel> SaveGames { get; } = [];

    public ICommand DeleteSaveCommand { get; }
    public ICommand BrowseCommand { get; }

    public MainViewModel()
    {
        // Load persisted settings
        _appSettings = AppSettings.Load();
        _saveDirectory = _appSettings.LastSaveDirectory;

        DeleteSaveCommand = new RelayCommand<SaveGameViewModel>(ExecuteDeleteSave);
        BrowseCommand = new AsyncRelayCommand(ExecuteBrowseAsync);
        LoadSaveGames();
    }

    private void LoadSaveGames()
    {
        if (!Directory.Exists(SaveDirectory)) return;

        SaveGames.Clear();
        var files = Directory.GetFiles(SaveDirectory, "*.gmc");

        foreach (var file in files)
        {
            var vm = new SaveGameViewModel(file, ExecuteDeleteSave, ExecuteDuplicateSave);
            SaveGames.Add(vm);
        }
    }

    private void ExecuteDuplicateSave(SaveGameViewModel vm)
    {
        if (vm == null || !File.Exists(vm.FullPath)) return;

        string dir = Path.GetDirectoryName(vm.FullPath)!;
        string originalName = Path.GetFileNameWithoutExtension(vm.FullPath);
        string extension = Path.GetExtension(vm.FullPath);

        string newPath;
        int suffix = 1;

        do
        {
            string newFileName = $"{originalName}_{suffix}{extension}";
            newPath = Path.Combine(dir, newFileName);
            suffix++;
        }
        while (File.Exists(newPath) || SaveGames.Any(s => s.FullPath.Equals(newPath, StringComparison.OrdinalIgnoreCase)));

        try
        {
            File.Copy(vm.FullPath, newPath);

            var newVm = new SaveGameViewModel(newPath, ExecuteDeleteSave, ExecuteDuplicateSave);
            SaveGames.Add(newVm);
        }
        catch (IOException ex)
        {
            // Handle alert/dialog display using your custom alert mechanism or UI framework dialog
            Console.WriteLine($"Could not duplicate file: {ex.Message}");
        }
    }

    private async void ExecuteDeleteSave(SaveGameViewModel vm)
    {
        if (vm == null) return;

        bool confirmed = await DropDownAlert.Show($"Delete {vm.FileName}.gmc?");

        if (confirmed)
        {
            if (File.Exists(vm.FullPath))
            {
                File.Delete(vm.FullPath);
                SaveGames.Remove(vm);
            }
        }
    }

    private async Task ExecuteBrowseAsync()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);
            if (topLevel == null) return;

            var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Save Directory",
                AllowMultiple = false
            });

            if (folders.Count > 0)
            {
                string path = folders[0].Path.LocalPath;
                if (Directory.Exists(path))
                {
                    // Save settings
                    _appSettings.LastSaveDirectory = path;
                    _appSettings.Save();

                    SaveDirectory = path;
                    LoadSaveGames();
                }
            }
        }
    }
}