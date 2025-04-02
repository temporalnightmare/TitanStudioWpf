using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System.Collections.ObjectModel;
using System.IO;
using TitanStudioWpf.Core.Helpers.IO;
using TitanStudioWpf.Core.Interfaces;
using TitanStudioWpf.Core.Models;

namespace TitanStudioWpf.ViewModels;

public partial class ToolJSFBScannerViewModel : ObservableObject
{
    private readonly IFileDialog _fileDialog;
    private readonly ILogger _logger;

    public ToolJSFBScannerViewModel(IFileDialog fileDialog, ILogger logger)
    {
        _fileDialog = fileDialog;
        _logger = logger;
        FilteredScanResults = new ObservableCollection<ScanResult>();

        _logger.Information("[JSFB Scanner] JSFB Scanner opened");
    }

    [ObservableProperty]
    private string _csvFilePath = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ScanCommand))]
    private string _jsfbFilePath = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ScanCommand))]
    private string _csvPath = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ScanResult> _scanResults = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ScanCommand))]
    private bool _isScanning;

    [ObservableProperty]
    private string _status = string.Empty;

    private ObservableCollection<ScanResult> _filteredScanResults = new();
    public ObservableCollection<ScanResult> FilteredScanResults
    {
        get => _filteredScanResults;
        private set => SetProperty(ref _filteredScanResults, value);
    }
 
    [RelayCommand(CanExecute = nameof(CanScan))]
    public async Task ScanAsync()
    {
        if (!File.Exists(JsfbFilePath) || !File.Exists(CsvPath))
        {
            Status = "Please select both JSFB and CSV files";
            return;
        }

        try
        {
            IsScanning = true;
            Status = "Scanning...";
            ScanResults.Clear();

            // Read CSV values
            var csvValues = await File.ReadAllLinesAsync(CsvPath);
            _logger.Verbose($"{csvValues.Length} lines read from {CsvPath}");

            // Create dictionary to store ID -> String mapping
            var idToStringMap = new Dictionary<uint, string>();
            var searchValues = csvValues
                .Skip(1) // Skip header
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line =>
                {
                    var parts = line.Split(',');
                    if (parts.Length < 2) return (id: 0u, str: string.Empty);

                    // Remove quotes and trim whitespace
                    var idStr = parts[0].Trim('"', ' ');
                    var valueStr = parts[1].Trim('"', ' ');
                    _logger.Verbose($"Processing CSV ID: {idStr}");

                    if (uint.TryParse(idStr, out uint value))
                    {
                        _logger.Verbose($"Successfully parsed value: {value}");
                        idToStringMap[value] = valueStr;
                        return (id: value, str: valueStr);
                    }
     
                    return (id: 0u, str: string.Empty);
                })
                .Where(v => v.id != 0)
                .Select(v => v.id)
                .ToHashSet();

            _logger.Verbose($"Found {searchValues.Count} unique values to search for");

            // Read binary file in chunks
            using var fileStream = File.OpenRead(JsfbFilePath);
            _logger.Verbose($"Opened binary file, size: {fileStream.Length} bytes");

            byte[] buffer = new byte[sizeof(uint)];
            long position = 0;
            int valuesChecked = 0;

            while (position < fileStream.Length - sizeof(uint))
            {
                fileStream.Position = position;
                await fileStream.ReadAsync(buffer);
                uint value = BitConverter.ToUInt32(buffer, 0);

                if (searchValues.Contains(value))
                {
                    _logger.Verbose($"Found match at position {position}: value {value}");
                    ScanResults.Add(new ScanResult
                    {
                        Offset = position,
                        Value = value,
                        HexOffset = $"0x{position:X8}",
                        String = idToStringMap.GetValueOrDefault(value, string.Empty)
                    });
                }

                position++;
                valuesChecked++;

                if (valuesChecked % 1000000 == 0)
                {
                    _logger.Verbose($"Checked {valuesChecked} values, current position: {position}");
                }
            }

            Status = $"Scan complete. Found {ScanResults.Count} matches.";
        }
        catch (Exception ex)
        {
            Status = $"{ex.Message}";
            _logger.Error($"Error during JSFB scan: {ex}");
        }
        finally
        {
            IsScanning = false;
        }
    }

    [RelayCommand]
    public void BrowseJSFBFile()
    {
        JsfbFilePath = FileSystem.BrowseFile("JSON FlatBuffer (*.jsfb)|*.jsfb", "Select a JSFB File");
    }

    [RelayCommand]
    public void BrowseCSVFile()
    {
        CsvPath = FileSystem.BrowseFile("String Database CSV (*.csv)|*.csv", "Select a SDB CSV File");
    }

    partial void OnSearchTextChanged(string value)
    {
        FilterResults();
    }

    partial void OnScanResultsChanged(ObservableCollection<ScanResult> value)
    {
        FilterResults();
    }

    private void FilterResults()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredScanResults = new ObservableCollection<ScanResult>(ScanResults);
            Status = $"Found {ScanResults.Count} matches.";
            _logger.Information("[JSFB Scanner] Found {0} matches", ScanResults.Count);
            return;
        }

        var searchLower = SearchText.ToLowerInvariant();
        var filtered = ScanResults.Where(result =>
            result.HexOffset.ToLowerInvariant().Contains(searchLower) ||
            result.Offset.ToString().Contains(searchLower) ||
            result.Value.ToString().Contains(searchLower) ||
            (result.String?.ToLowerInvariant().Contains(searchLower) ?? false)
        ).ToList();

        FilteredScanResults = new ObservableCollection<ScanResult>(filtered);
        Status = $"Showing {filtered.Count} of {ScanResults.Count} matches";
        _logger.Information(@"[JSFB Scanner] Showing {0} of {1} matches", filtered.Count, ScanResults.Count);
    }

    private bool CanScan()
    {
        var canScan = !IsScanning && !string.IsNullOrWhiteSpace(JsfbFilePath) && !string.IsNullOrWhiteSpace(CsvPath);

        _logger.Verbose(@"CanScan: {0} (IsScanning: {1}, JsfbFilePath: '{2}', CsvPath: '{3}')", canScan, IsScanning, JsfbFilePath, CsvPath);

        return canScan;
    }
   
}
