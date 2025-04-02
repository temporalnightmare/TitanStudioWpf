using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TitanStudioWpf.Core.Interfaces;

namespace TitanStudioWpf.ViewModels;

public partial class ToolFlatbufferTestViewModel : ObservableObject
{

    //public event EventHandler RequestClose;
    private readonly IFileDialog _fileDialog;
    private readonly ILogger _logger;
  
    private static string ThirdPartyPath => Path.Combine(
        new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString(),
        "Thirdparty"
    );

    private static string FlatcPath => Path.Combine(ThirdPartyPath, "flatc.exe");

    [ObservableProperty]
    private byte[] _schema;

    [ObservableProperty]
    private double _progress;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SerializeCommand))]
    private string _jsfbPath;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SerializeCommand))]
    private string _jsonPath;

    [ObservableProperty]
    private string status = "Ready";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SerializeCommand))]
    private bool isProcessing;


    [RelayCommand(CanExecute = nameof(CanSerialize))]
    public async Task Serialize()
    {
        if (!File.Exists(FlatcPath))
        {
            Status = "Error: flatc.exe not found in Thirdparty folder";
            _logger.Fatal("[Flatbuffer] flatc.exe not found in the ThirdParty folder, can not serialize!");
            return;
        }

        IsProcessing = true;
        Progress = 0;
        Status = "Preparing to serialize...";
        _logger.Information("[Flatbuffer] Preparing to serialize");

        try
        {
            await Task.Run(async () =>
            {
                // Create temp paths with unique names
                string tempDir = Path.GetTempPath();
                string schemaPath = Path.Combine(tempDir, $"schema_{Guid.NewGuid()}.fbs");
                string dataPath = Path.Combine(tempDir, $"data_{Guid.NewGuid()}.jsfb");

                try
                {
                    // Write schema to temp file
                    _logger.Information("[Flatbuffer] Writing schema file");
                    Status = "Writing schema file...";
                    Progress = 20;
                    await File.WriteAllBytesAsync(schemaPath, Schema);

                    // Copy JSON file to temp location
                    File.Copy(JsonPath, dataPath, true);

                    // Prepare flatc arguments
                    Status = "Executing flatc compiler...";
                    _logger.Information("[Flatbuffer] Executing flatc compiler");
                    Progress = 40;
                    string arguments = $"-t --strict-json \"{schemaPath}\" -- \"{dataPath}\"";

                    // Start flatc process
                    using var process = new Process
                    {
                        StartInfo = new ProcessStartInfo(FlatcPath, arguments)
                        {
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardError = true,
                            WorkingDirectory = tempDir
                        }
                    };

                    Progress = 60;
                    process.Start();
                    string error = await process.StandardError.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    if (process.ExitCode == 0)
                    {
                        Progress = 80;
                        Status = "Reading serialized data...";
                        _logger.Information("[Flatbuffer] Reading serialized data");

                        // Here you can read and process the output file
                        byte[] serializedData = await File.ReadAllBytesAsync(Path.ChangeExtension(dataPath, ".json"));

                        Progress = 100;
                        Status = "Serialization completed successfully";
                        _logger.Information("[Flatbuffer] Serialization completed successfully");
                    }
                    else
                    {
                        Status = $"Serialization failed: {error}";
                        _logger.Error("[Flatbuffer] Serialization failed: {error}", error);
                    }
                }
                finally
                {
                    // Cleanup temp files
                    if (File.Exists(schemaPath)) File.Delete(schemaPath);
                    if (File.Exists(dataPath)) File.Delete(dataPath);
                    if (File.Exists(Path.ChangeExtension(dataPath, ".json")))
                        File.Delete(Path.ChangeExtension(dataPath, ".json"));
                }
            });
        }
        catch (Exception ex)
        {
            Status = $"Error: {ex.Message}";
            _logger.Error("[Flatbuffer] {ex.Message}", ex.Message);
        }
        finally
        {
            IsProcessing = false;
        }
    }

 
    /*
    {
        // We want to select a jsfb.
        string result = _fileDialog.OpenFile("JSFB files (*.jsfb)|*.jsfb|All files (*.*)|*.*", "Select Character Mapping File");

        //string result = _fileDialog.OpenFile("JSON files (*.json)|*.json|All files (*.*)|*.*", "Select Character Mapping File");
        if (!string.IsNullOrEmpty(result))
        {
            JsfbPath = result;

            try
            {
                // TESTING
                // Get the Core assembly directly
                var coreAssembly = typeof(ILogger).Assembly;
                _logger.Log($"Core Assembly Name: {coreAssembly.GetName().Name}");

                // List all resources in Core assembly
                var resources = coreAssembly.GetManifestResourceNames();
                _logger.Log($"Core Assembly Resources: {string.Join(Environment.NewLine, resources)}");

                // Load the schema file from the embedded resource
                //var assembly = Assembly.GetExecutingAssembly();
                // using var stream = assembly.GetManifestResourceStream("TitanStudioWpf.Core.Schemas.WWE2K25.CharacterMapping_WWE2K25.fbs");


                using var stream = coreAssembly.GetManifestResourceStream("TitanStudioWpf.Core.Schemas.WWE2K25.CharacterMapping_WWE2K25.fbs");
                if (stream != null)
                {
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    Schema = memoryStream.ToArray();

                    // Write the schema to a file
                    string outputPath = "CharacterMapping_WWE2K25.jsfb";
                    await File.WriteAllBytesAsync(outputPath, Schema);

                    Status = $"Schema loaded and saved to {outputPath}";
                }
                else
                {
                    Status = $"Error: Could not find schema resource. Available resources in Core: {string.Join(", ", resources)}";
                    Schema = null;
                }
            }
            catch (Exception ex)
            {
                Status = $"Error loading schema: {ex.Message}";
                _logger.LogError($"Schema load error: {ex}");
                Schema = null;
            }
        }
    }*/

    public ToolFlatbufferTestViewModel(IFileDialog fileDialog, ILogger logger)
    {
        _fileDialog = fileDialog;
        _logger = logger;

        _logger.Information("Flatbuffer Tool opened");
    }

    private bool CanSerialize() => !IsProcessing && Schema != null;

}
