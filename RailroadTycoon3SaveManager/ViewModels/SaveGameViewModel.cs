using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;

namespace RailroadTycoon3SaveManager.ViewModels;

public class SaveGameViewModel : INotifyPropertyChanged
{
    private string _fullPath;
    private bool _isEditing;

    public SaveGameViewModel(string path, Action<SaveGameViewModel> onDelete, Action<SaveGameViewModel> onDuplicate)
    {
        _fullPath = path;

        BeginRenameCommand = new RelayCommand<object>(_ => IsEditing = true);
        CancelRenameCommand = new RelayCommand<object>(_ => IsEditing = false);

        RenameFileCommand = new RelayCommand<string>(newName =>
        {
            if (string.IsNullOrWhiteSpace(newName) || newName == FileName)
            {
                IsEditing = false;
                return;
            }

            string directory = Path.GetDirectoryName(FullPath)!;
            string newPath = Path.Combine(directory, newName + ".gmc");

            try
            {
                File.Move(FullPath, newPath);
                FullPath = newPath;
                IsEditing = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Rename failed: {ex.Message}");
            }
        });

        DeleteFileCommand = new RelayCommand<object>(_ =>
        {
            try
            {
                onDelete?.Invoke(this);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete failed: {ex.Message}");
            }
        });

        DuplicateFileCommand = new RelayCommand<object>(_ =>
        {
            try
            {
                onDuplicate?.Invoke(this);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Duplication failed: {ex.Message}");
            }
        });
    }

    public SaveGameViewModel() : this("SaveGame", _ => { }, _ => { }) { }

    public string FullPath
    {
        get => _fullPath;
        set
        {
            _fullPath = value;
            OnPropertyChanged(nameof(FullPath));
            OnPropertyChanged(nameof(FileName));
        }
    }

    public string FileName => Path.GetFileNameWithoutExtension(FullPath);

    public bool IsEditing
    {
        get => _isEditing;
        set { _isEditing = value; OnPropertyChanged(nameof(IsEditing)); }
    }

    public ICommand BeginRenameCommand { get; }
    public ICommand CancelRenameCommand { get; }
    public ICommand RenameFileCommand { get; }
    public ICommand DeleteFileCommand { get; }
    public ICommand DuplicateFileCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}