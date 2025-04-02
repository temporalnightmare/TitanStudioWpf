using System.Collections.ObjectModel;

namespace TitanStudioWpf.Core.Models;

public class Option
{
    public string CakeToolPath { get; set; } = string.Empty;
    public string ModsFolderPath { get; set; } = string.Empty;
    public string BakeFolderPath { get; set; } = string.Empty;
    public string GamePath { get; set; } = string.Empty;
    public string LogPath { get; set; } = string.Empty;
    public double FontSize { get; set; } = 14.0;
    public string FontName { get; set; } = string.Empty;
}

public class OptionItems
{
    public string Name { get; }
    public ObservableCollection<OptionItems> Children { get; }
    public OptionItems(string name, ObservableCollection<OptionItems>? children = null)
    {
        Name = name;
        Children = children ?? new ObservableCollection<OptionItems>();
    }
}

