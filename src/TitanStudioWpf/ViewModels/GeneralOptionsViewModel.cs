using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System.Collections.ObjectModel;
using TitanStudioWpf.Core.Helpers.IO;

namespace TitanStudioWpf.ViewModels;

public partial class GeneralOptionsViewModel : ObservableObject
{
    private readonly ILogger _logger;

    public GeneralOptionsViewModel(ILogger logger)
    {
        _logger = logger;
        InitializeFonts();
    }

    [ObservableProperty]
    private ObservableCollection<string> _fonts = new();

    [ObservableProperty]
    private string _fontName = string.Empty;

    [ObservableProperty]
    private string _gamePath = string.Empty;

    [ObservableProperty]
    private string _logPath = string.Empty;

    [RelayCommand]
    public void SetGamePath()
    {
        GamePath = FileSystem.BrowseFile("Executable File (*.exe)|*.exe", "Select WWE 2K25 File");
        _logger.Information("[Options] Game path set to {0}", GamePath);
    }

    [RelayCommand]
    public void SetLogPath()
    {
        LogPath = FileSystem.SetFile("TitanStudio Log File (*.log)|*.log", "Set Log File");
        _logger.Information("[Options] Log path set to {0}", LogPath);
    }


    private void InitializeFonts()
    {
        foreach (var family in System.Windows.Media.Fonts.SystemFontFamilies.OrderBy(f => f.Source))
        {
            Fonts.Add(family.Source);
        }
        if (Fonts.Any())
        {
            FontName = Fonts.First();
        }
    }
}
