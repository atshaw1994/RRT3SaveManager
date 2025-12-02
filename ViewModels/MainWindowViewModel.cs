using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace RRT3SaveManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? selectedFolder;
    private static readonly string SettingsFilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RRT3SaveManager", "settings.txt");

    public ObservableCollection<string> SaveGameFiles { get; } = [];    
    public IRelayCommand OpenFolderDialogCommand { get; }
    public IRelayCommand DeleteFileCommand { get; }
    public IRelayCommand<string> BeginRenameCommand { get; }
    public IRelayCommand ConfirmRenameCommand { get; }
    public IRelayCommand CancelRenameCommand { get; }
    public event Action? RequestFolderDialog;
    
    public MainWindowViewModel()
    {
        OpenFolderDialogCommand = new RelayCommand(OnOpenFolderDialog);
        DeleteFileCommand = new RelayCommand<string?>(DeleteFile);
        SelectedFolder = LoadFolderFromSettings();
        BeginRenameCommand = new RelayCommand<string>(BeginRename);
        ConfirmRenameCommand = new RelayCommand(ConfirmRename);
        CancelRenameCommand = new RelayCommand(CancelRename);
    }
    public string FormattedFolderPath
    {
        get
        {
            if (string.IsNullOrWhiteSpace(SelectedFolder))
                return "SaveGame Folder...";
            try
            {
                var parts = SelectedFolder.TrimEnd(System.IO.Path.DirectorySeparatorChar).Split(System.IO.Path.DirectorySeparatorChar);
                if (parts.Length <= 2)
                    return SelectedFolder;
                return $"{System.IO.Path.DirectorySeparatorChar}{parts[0]}{System.IO.Path.DirectorySeparatorChar}...{System.IO.Path.DirectorySeparatorChar}{parts[^1]}{System.IO.Path.DirectorySeparatorChar}";
            }
            catch
            {
                return SelectedFolder;
            }
        }
    }

    private static void EnsureSettingsDirectory()
    {
        var dir = System.IO.Path.GetDirectoryName(SettingsFilePath);
        if (!System.IO.Directory.Exists(dir))
            System.IO.Directory.CreateDirectory(dir!);
    }
    private static void SaveFolderToSettings(string? folder)
    {
        EnsureSettingsDirectory();
        System.IO.File.WriteAllText(SettingsFilePath, folder ?? string.Empty);
    }
    private static string? LoadFolderFromSettings()
    {
        if (System.IO.File.Exists(SettingsFilePath))
        {
            return System.IO.File.ReadAllText(SettingsFilePath).Trim();
        }
        return null;
    }
    private void OnOpenFolderDialog()
    {
        RequestFolderDialog?.Invoke();
    }
    private void DeleteFile(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(SelectedFolder))
            return;
        var filePath = System.IO.Path.Combine(SelectedFolder, fileName);
        try
        {
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }
        catch { }
        SaveGameFiles.Remove(fileName);
    }
    private void BeginRename(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return;
        IsRenamingFile = fileName;
        RenamingFileNewName = System.IO.Path.GetFileNameWithoutExtension(fileName);
    }
    private void ConfirmRename()
    {
        if (string.IsNullOrWhiteSpace(IsRenamingFile) || string.IsNullOrWhiteSpace(RenamingFileNewName) || string.IsNullOrWhiteSpace(SelectedFolder))
        {
            IsRenamingFile = null;
            RenamingFileNewName = null;
            return;
        }
        var oldPath = System.IO.Path.Combine(SelectedFolder, IsRenamingFile);
        var ext = System.IO.Path.GetExtension(IsRenamingFile);
        var newFileName = RenamingFileNewName + ext;
        var newPath = System.IO.Path.Combine(SelectedFolder, newFileName);
        if (oldPath != newPath && !System.IO.File.Exists(newPath))
        {
            try
            {
                System.IO.File.Move(oldPath, newPath);
                int idx = SaveGameFiles.IndexOf(IsRenamingFile);
                if (idx >= 0)
                {
                    SaveGameFiles[idx] = newFileName;
                }
            }
            catch { }
        }
        IsRenamingFile = null;
        RenamingFileNewName = null;
    }

    private void CancelRename()
    {
        IsRenamingFile = null;
        RenamingFileNewName = null;
    }

    partial void OnSelectedFolderChanged(string? value)
    {
        OnPropertyChanged(nameof(FormattedFolderPath));
        SaveFolderToSettings(value);
        SaveGameFiles.Clear();
        if (!string.IsNullOrWhiteSpace(value) && System.IO.Directory.Exists(value))
        {
            try
            {
                foreach (var file in System.IO.Directory.GetFiles(value))
                {
                    SaveGameFiles.Add(System.IO.Path.GetFileName(file));
                }
            }
            catch { }
        }
    }

    private string? isRenamingFile;
    public string? IsRenamingFile
    {
        get => isRenamingFile;
        set
        {
            if (isRenamingFile != value)
            {
                isRenamingFile = value;
                OnPropertyChanged(nameof(IsRenamingFile));
            }
        }
    }

    private string? renamingFileNewName;
    public string? RenamingFileNewName
    {
        get => renamingFileNewName;
        set
        {
            if (renamingFileNewName != value)
            {
                renamingFileNewName = value;
                OnPropertyChanged(nameof(RenamingFileNewName));
            }
        }
    }

}
