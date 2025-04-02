using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using TitanStudioWpf.Core.Helpers.IO;

namespace TitanStudioWpf.ViewModels;

public partial class BakeryOptionsViewModel : ObservableObject
{
    private readonly ILogger _logger;

    public BakeryOptionsViewModel(ILogger logger)
    {
        _logger = logger;
    }

    [ObservableProperty]
    private string _cakeToolPath = string.Empty;

    [ObservableProperty]
    private string _modsFolderPath = string.Empty;

    [ObservableProperty]
    private string _bakeFolderPath = string.Empty;

   
    [RelayCommand]
    public void SetCakeToolPath()
    {
        CakeToolPath = FileSystem.BrowseFile("Executable File (*.exe)|*.exe", "Select CakeTool EXE Path");
        _logger.Information("[Options] CakeTool path set to {0}", CakeToolPath);
    }

    [RelayCommand]
    public void SetModsFolderPath()
    {
        ModsFolderPath = FileSystem.BrowseFolder();
        _logger.Information("[Options] Mods path set to {0}", ModsFolderPath);
    }

    [RelayCommand]
    public void SetBakeFolderPath()
    {
        BakeFolderPath = FileSystem.BrowseFolder();
        _logger.Information("[Options] Bake folder set to {0}", BakeFolderPath);
    }
}
