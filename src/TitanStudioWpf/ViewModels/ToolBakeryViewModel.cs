using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System.Diagnostics;
using System.IO;
using TitanStudioWpf.Core.Helpers.IO;
using TitanStudioWpf.Core.Interfaces;
using TitanStudioWpf.Core.Models;

namespace TitanStudioWpf.ViewModels;

public partial class ToolBakeryViewModel : ObservableObject
{
    public event EventHandler RequestClose;

    private readonly IFileDialog _fileDialog;
    private readonly ILogger _logger;
    private readonly BakeryOptionsViewModel _bakeryOptionsViewModel;
    private readonly Option _options;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public ToolBakeryViewModel(IFileDialog fileDialog, ILogger logger, BakeryOptionsViewModel bakeryOptionsViewModel, Option options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        _fileDialog = fileDialog;
        _logger = logger;
        _bakeryOptionsViewModel = bakeryOptionsViewModel;
        _options = options;

        _logger.Information("[Bakery Tool] Bakery Tool opened");

        _bakeableFolderPath = _options.BakeFolderPath;

        // Initialize version
        _version = "9.3"; // Show default game version (WWE 2K25)
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(BakeCommand))]
    private bool _isBaking;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(BakeCommand))]
    private string _bakeableFolderPath = string.Empty;

    [RelayCommand]
    public void SetBakeFolder()
    {
        BakeableFolderPath = FileSystem.BrowseFolder();
        _logger.Information("[Bakery Tool] Bake Folder set");
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(BakeCommand))]
    private string _bakedCakFileNamePath = string.Empty;

    [RelayCommand]
    public void SetCakOutputFolder()
    {
        BakedCakFileNamePath = FileSystem.SetFile("CAK File (*.cak)|*.cak", "Select CAK Location");
        _logger.Information("[Bakery Tool] CAK Folder set");
    }

    [RelayCommand(CanExecute = nameof(CanBake))]
    public async Task Bake()
    {
        // Only bake if the bake folder exists
        if (Directory.Exists(BakeableFolderPath))
        {
            try
            {
                IsBaking = true;
                string exePath = _bakeryOptionsViewModel.CakeToolPath;

                // Ensure exePath exists
                if (!File.Exists(exePath))
                {
                    StatusMessage = $"File not found - \"{exePath}\"";
                    _logger.Error($"[Bakery Tool] File not found: \"{exePath}\"");
                    return;
                }


                if (BakedCakFileNamePath == string.Empty)
                {
                    StatusMessage = $"Please specify a output cak.";
                    _logger.Information(@"[Bakery Tool] Please specify a output cak.");
                    return;
                }

                string arguments = $"pack -i \"{BakeableFolderPath}\" -v {Version} -o \"{BakedCakFileNamePath}\"";

                _logger.Information(@"[Bakery Tool] CakeTool Path: {exePath} with {arguments}", exePath, arguments);

                // Configure the process to capture output
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = exePath, // FileName should NOT have quotes
                    Arguments = arguments, // Arguments should be formatted separately
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Execute the process

                await Task.Run(() =>
                {
                    using (Process process = new Process { StartInfo = startInfo })
                    {
                        process.Start();
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();
                        process.WaitForExit();

                        Debug.WriteLine("Output: " + output);
                        Debug.WriteLine("Error: " + error);
                    }
                });
 

                StatusMessage = @$"Baking is done. Your baked cake is located in {BakedCakFileNamePath}";
                _logger.Information(@"[Bakery Tool] Baking is now done, your bake file is located in {OutputFilePath}", BakedCakFileNamePath);

                // Raise the close event
                RequestClose?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex, "Failed to pack.");
                _logger.Error("Failed to pack the cak file.");
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBaking = false;
            }
        }
        else
        {
            StatusMessage = "Error: Bake folder does not exist";
            _logger.Error("[Bakery Tool] The bake folder does not exist, please choose one.");
        }
    }


    #region EXTRACTING
    [ObservableProperty]
    private string _cakFileNamePath = string.Empty;

    [RelayCommand]
    public void SetCakFileName()
    {
        CakFileNamePath = FileSystem.BrowseFile("All WWE 2K CAK Files (*.cak)|*.cak");
        _logger.Information("[Bakery Tool] CAK File Path set");
    }

    [ObservableProperty]
    private string _extractedFileNamePath = string.Empty;

    [ObservableProperty]
    private string _outputFilePath = string.Empty;


    [RelayCommand]
    public void SetOutputFolder()
    {
        OutputFilePath = FileSystem.BrowseFolder();
        _logger.Information("[Bakery Tool] Output Folder set");
    }

    [RelayCommand]
    public void Extract()
    {

        try
        {
            string exePath = _bakeryOptionsViewModel.CakeToolPath;

            if (!File.Exists(CakFileNamePath))
            {
                StatusMessage = $"File not found - \"{CakFileNamePath}\"";
                _logger.Error($"[Bakery Tool] File not found: \"{CakFileNamePath}\"");
                return;
            }

            if (ExtractedFileNamePath == string.Empty)
            {
                StatusMessage = $"Please specify a filename";
                _logger.Information(@"[Bakery Tool] Please specify a filename");
                return;
            }

            string arguments = $"unpack-file -i \"{CakFileNamePath}\" -f \"{ExtractedFileNamePath}\" -o \"{OutputFilePath}\"";

            _logger.Information(@"[Bakery Tool] CakeTool Path: {exePath} with {arguments}", exePath, arguments);

            // Configure the process to capture output
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath, // FileName should NOT have quotes
                Arguments = arguments, // Arguments should be formatted separately
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Execute the process
            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                Debug.WriteLine("Output: " + output);
                Debug.WriteLine("Error: " + error);

                StatusMessage = @$"Extracting is done. Your extracted file is located in {OutputFilePath}";
                _logger.Information(@"[Bakery Tool] Extracting is done, your extracted file is located in {OutputFolderPath}", OutputFilePath);

                // Raise the close event
                RequestClose?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to unpack the cak file.");
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    #endregion

    // string arguments = $"unpack-file -i \"{CakFileNamePath}\" -f \"{ExtractFileNamePath}\" -o \"{OutputFilePath}\"";

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _version = "9.3";  // Default to latest version
 
    [RelayCommand]
    public void CancelBake()
    {
        // Raise the close event
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    public class GameVersion
    {
        public string Display { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public List<GameVersion> GameVersions { get; } = new()
    {
        new GameVersion { Display = "WWE 2K24", Value = "9.2" },
        new GameVersion { Display = "WWE 2K25", Value = "9.3" }
    };


    private bool CanBake()
    {
        var canBake = !IsBaking && !string.IsNullOrWhiteSpace(BakeableFolderPath) && !string.IsNullOrWhiteSpace(BakedCakFileNamePath);

        _logger.Verbose(@"CanBake: {0} (IsBaking: {1}, JsfbFilePath: '{2}', CsvPath: '{3}')", 
            canBake, IsBaking, BakeableFolderPath, BakedCakFileNamePath);

        return canBake;
    }

}
