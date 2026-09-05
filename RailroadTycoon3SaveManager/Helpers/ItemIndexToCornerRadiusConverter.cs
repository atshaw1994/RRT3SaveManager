using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.VisualTree;
using System;
using System.Globalization;

namespace RailroadTycoon3SaveManager.Helpers;

public class ItemIndexToCornerRadiusConverter : IValueConverter
{
    public double Radius { get; set; } = 6.0;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ListBoxItem item)
        {
            // Find parent ListBox container via Visual Tree
            var listBox = item.FindAncestorOfType<ListBox>();
            if (listBox == null) return new CornerRadius(0);

            int index = listBox.IndexFromContainer(item);
            int totalCount = listBox.ItemCount;

            // Single item in list (round all 4 corners)
            if (totalCount == 1)
                return new CornerRadius(Radius);

            // First Item (top corners)
            if (index == 0)
                return new CornerRadius(Radius, Radius, 0, 0);

            // Last Item (bottom corners)
            if (index == totalCount - 1)
                return new CornerRadius(0, 0, Radius, Radius);
        }

        return new CornerRadius(0);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
