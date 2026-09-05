using System;

namespace RailroadTycoon3SaveManager.Models;

public class SaveGame
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullPath { get; set; } = string.Empty;
    public bool IsEditing { get; set; } = false;
}
