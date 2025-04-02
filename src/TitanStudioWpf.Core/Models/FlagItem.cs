using CommunityToolkit.Mvvm.ComponentModel;

namespace TitanStudioWpf.Core.Models;

public partial class FlagItem : ObservableObject
{
    [ObservableProperty]
    private bool _isChecked;

    [ObservableProperty]
    private string _label;

    public int Position { get; }
    public string Category { get; }
    public byte UnlockedValue { get; } // Value when checked (usually 0x02)
    public byte DefaultValue { get; } // Value when unchecked (varies by category)
    public FlagItem(string label, int position, string category, byte unlockedValue, byte defaultValue, bool initialState = false)
    {
        Label = label;
        Position = position;
        Category = category;
        UnlockedValue = unlockedValue;
        DefaultValue = defaultValue;
        IsChecked = false;
    }
}
