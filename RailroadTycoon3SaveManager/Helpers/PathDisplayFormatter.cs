using Avalonia.Data.Converters;
using System;
using System.Globalization;
using System.IO;

namespace RailroadTycoon3SaveManager.Helpers;

public class PathDisplayFormatter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string path && !string.IsNullOrWhiteSpace(path))
        {
            try
            {
                var directoryInfo = new DirectoryInfo(path);
                string root = Path.GetPathRoot(path) ?? "";
                string folderName = directoryInfo.Name;

                // If the path is just the root (e.g., "C:\"), don't truncate
                if (root.Equals(path.TrimEnd('\\', '/'), StringComparison.OrdinalIgnoreCase))
                {
                    return root;
                }

                // Format: Root + Ellipses + FolderName (using cross-platform path separator)
                return $"{root}...{Path.DirectorySeparatorChar}{folderName}";
            }
            catch
            {
                return value; // Fallback to original string if error occurs
            }
        }

        return "Browse...";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
