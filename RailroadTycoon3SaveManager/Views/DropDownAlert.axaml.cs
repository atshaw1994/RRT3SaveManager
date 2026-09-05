using Avalonia;
using Avalonia.Controls;
using System;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Media.Transformation;
using Color = Avalonia.Media.Color;
using Application = Avalonia.Application;
using Avalonia.Controls.ApplicationLifetimes;
using System.Linq;

namespace RailroadTycoon3SaveManager.Views;

public partial class DropDownAlert : UserControl
{
    public static readonly StyledProperty<string> AlertMessageProperty =
        AvaloniaProperty.Register<DropDownAlert, string>(
            nameof(AlertMessage),
            "This is where the alert message will go.");

    public string AlertMessage
    {
        get => GetValue(AlertMessageProperty);
        set => SetValue(AlertMessageProperty, value);
    }

    public event EventHandler? Closed;
    private bool _userConfirmed;

    public DropDownAlert()
    {
        InitializeComponent();
        // Start hidden above the viewport
        MainGrid.RenderTransform = TransformOperations.Parse("translateY(-200px)");
    }

    public static async Task<bool> Show(string message)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return false;

        var mainWindow = desktop.Windows.FirstOrDefault(w => w.IsActive) ?? desktop.MainWindow;
        if (mainWindow?.Content is not Grid rootGrid)
            throw new InvalidOperationException("MainWindow content must be a Grid to support DropDownAlert overlays.");

        // Overlay backdrop
        var overlay = new Grid
        {
            Background = new SolidColorBrush(Color.Parse("#80000000")),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
        };

        var alert = new DropDownAlert { AlertMessage = message };
        overlay.Children.Add(alert);
        rootGrid.Children.Add(overlay);

        // Animate alert down
        await Task.Delay(50); // Short frame delay to allow initial layout render
        alert.ShowAlert();

        var tcs = new TaskCompletionSource<bool>();

        alert.Closed += async (_, _) =>
        {
            await alert.CloseAlertAsync();
            rootGrid.Children.Remove(overlay);
            tcs.SetResult(alert._userConfirmed);
        };

        return await tcs.Task;
    }

    public void ShowAlert()
    {
        MainGrid.RenderTransform = TransformOperations.Parse("translateY(0px)");
    }

    public async Task CloseAlertAsync()
    {
        MainGrid.RenderTransform = TransformOperations.Parse("translateY(-200px)");
        await Task.Delay(400); // Match transition duration
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        _userConfirmed = false;
        Closed?.Invoke(this, EventArgs.Empty);
    }

    private void ConfirmButton_Click(object? sender, RoutedEventArgs e)
    {
        _userConfirmed = true;
        Closed?.Invoke(this, EventArgs.Empty);
    }
}