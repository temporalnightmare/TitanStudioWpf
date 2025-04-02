using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using TitanStudioWpf.Core.Helpers.Formatting;
using TitanStudioWpf.Core.Helpers.Hashing;
using TitanStudioWpf.Core.Helpers.IO;

namespace TitanStudioWpf.ViewModels;

public partial class ToolHasherViewModel : ObservableObject
{
    private readonly ILogger _logger;
 
    public ToolHasherViewModel(ILogger logger)
    {
        _logger = logger;

        _logger.Information("[Hasher Tool] Hasher Tool opened");
    }

    [ObservableProperty]
    private string? stringInput;

    [ObservableProperty]
    private string? hashOutput;
    
    [RelayCommand]
    public void SetFilePath()
    {
        StringInput = FileSystem.BrowseFile("All Files (*.*)|*.*", "Select Path to Hash");
        _logger.Information(@"[Hasher Tool] String Input: {0}", StringInput);
    }

    [RelayCommand]
    public void SetFolderPath()
    {
        StringInput = FileSystem.BrowseFolder();
        _logger.Information(@"[Hasher Tool] String Input: {0}", StringInput);
    }

    [RelayCommand]
    public void GenerateHash()
    {
        if (StringInput == null)
            return;

        // 1. Strip path
        string? input = StringFormat.StripPath(StringInput);

        // 2. Convert slashes
        input = StringFormat.ConvertSlashes(input);

        // 3. Convert path to lowercase
        input = StringFormat.ToLowercase(input);
        StringInput = input;

        // Pass parameter from the view to the library
        var code = FNV1A.Compute(input, HashType.FNV1A64);

        // Convert the uint result to a string
        HashOutput = code.ToString();

        _logger.Information(@"[Hasher Tool] String Input: {StringInput} with Hash Output {HashOutput}", StringInput, HashOutput);
    }

}
