using Avalonia.Controls;
using Avalonia.Input;
using RRT3SaveManager.ViewModels;
using System;

namespace RRT3SaveManager.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContextChanged += MainWindow_DataContextChanged;
    }

    private void FileNameTextBlock_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount == 2 && sender is TextBlock tb && DataContext is MainWindowViewModel vm)
        {
            var fileName = tb.DataContext as string;
            vm.IsRenamingFile = fileName;
            vm.RenamingFileNewName = System.IO.Path.GetFileNameWithoutExtension(fileName);
        }
    }

    private void RenameTextBox_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ConfirmRename(sender);
    }

    private void RenameTextBox_KeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ConfirmRename(sender);
        }
    }

    private void ConfirmRename(object? sender)
    {
        if (DataContext is MainWindowViewModel vm && sender is TextBox tb)
        {
            var fileName = vm.IsRenamingFile;
            var newName = tb.Text;
            if (!string.IsNullOrWhiteSpace(fileName) && !string.IsNullOrWhiteSpace(newName) && vm.SelectedFolder != null)
            {
                var ext = System.IO.Path.GetExtension(fileName);
                var oldPath = System.IO.Path.Combine(vm.SelectedFolder, fileName);
                var newFileName = newName + ext;
                var newPath = System.IO.Path.Combine(vm.SelectedFolder, newFileName);
                if (oldPath != newPath && !System.IO.File.Exists(newPath))
                {
                    try
                    {
                        System.IO.File.Move(oldPath, newPath);
                        int idx = vm.SaveGameFiles.IndexOf(fileName);
                        if (idx >= 0)
                        {
                            vm.SaveGameFiles[idx] = newFileName;
                        }
                    }
                    catch { }
                }
            }
            vm.IsRenamingFile = null;
            vm.RenamingFileNewName = null;
        }
    }

    private void MainWindow_DataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            // Unsubscribe first to avoid multiple subscriptions
            vm.RequestFolderDialog -= OnRequestFolderDialog;
            vm.RequestFolderDialog += OnRequestFolderDialog;
        }
    }

    private async void OnRequestFolderDialog()
    {
        if (DataContext is MainWindowViewModel vm)
        {
            var folders = await StorageProvider.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions
            {
                Title = "Select SaveGame Folder",
                AllowMultiple = false
            });
            var folder = (folders != null && folders.Count > 0) ? folders[0] : null;
            if (folder != null)
            {
                vm.SelectedFolder = folder.Path.LocalPath;
            }
        }
    }
}