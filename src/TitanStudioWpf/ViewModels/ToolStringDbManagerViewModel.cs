using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using TitanStudioWpf.Core.Helpers.Formatting;
using TitanStudioWpf.Core.Interfaces;
using TitanStudioWpf.Core.Models;
using TitanStudioWpf.Views;

namespace TitanStudioWpf.ViewModels;

public partial class ToolStringDbManagerViewModel : ObservableObject
{
    private readonly IFileDialog _fileDialog;
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    public ToolStringDbManagerViewModel(IServiceProvider serviceProvider, IFileDialog fileDialog, ILogger logger)
    {
        _serviceProvider = serviceProvider;
        _fileDialog = fileDialog;
        _logger = logger;

        _logger.Information("[String DB Manager] String DB Manager opened");
    }

    [ObservableProperty]
    private string _strings = string.Empty;

    [ObservableProperty]
    private ObservableCollection<StringGridItem> _gridItems = new();

    [ObservableProperty]
    private ObservableCollection<StringGridItem> _filteredGridItems = new();

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private StringDataModel? _currentSdbModel;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _newString = string.Empty;

    [RelayCommand]
    public void ShowNotImplemented()
    {
        var view = _serviceProvider.GetRequiredService<NotImplementedView>();
        view.ShowDialog();
    }

    [RelayCommand]
    public void Search()
    {
        if (CurrentSdbModel == null || GridItems.Count == 0)
        {
            StatusMessage = "No data has been loaded to do a search on.";
            return;
        }

        if (string.IsNullOrWhiteSpace(SearchText))
        {

            // If search is empty, show all items by default
            FilteredGridItems = new ObservableCollection<StringGridItem>(GridItems);
            StatusMessage = $"Showing all {FilteredGridItems.Count} items";
            return;
        }

        // Filter the grid items based on the search paramters (case-insensitive)
        var filteredItems = GridItems.Where(item =>
            item.String.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.ID.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.Index.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase)
        //item.Offset.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
        ).ToList();

        FilteredGridItems = new ObservableCollection<StringGridItem>(filteredItems);
        StatusMessage = $"Found {FilteredGridItems.Count} matches for '{SearchText}'";
    }

    [RelayCommand]
    public async Task OpenSDBAsync()
    {
        string? filePath = _fileDialog.OpenFile("SDB Files (*.sdb)|*.sdb", "Open SDB File");
        if (!string.IsNullOrEmpty(filePath))
        {
            // Process the selected file
            try
            {
                _logger.Verbose($"Opening SDB File: {filePath}");

                FilePath = filePath;

                // Read the file
                CurrentSdbModel = await StringDataHelper.ReadStringAsync(filePath);

                // Convert model to grid items
                GridItems = StringDataHelper.SerializeDataForGrid(CurrentSdbModel);

                // Initialize filtered items with all items
                FilteredGridItems = new ObservableCollection<StringGridItem>(GridItems);

                // Update status information
                StatusMessage = $"# of Rows: {CurrentSdbModel?.DatabaseStrings?.Length ?? 0}";

                //ExamineSdb(filePath);
            }
            catch (Exception ex)
            {
                // Add a messager service or ilogger
                StatusMessage = $"Error: {ex.Message}";
                _logger.Error(ex.Message);
            }

        }
    }

    [RelayCommand]
    public async Task ExportToCSV()
    {
        if (FilteredGridItems.Count == 0)
        {
            StatusMessage = "No data available to export";
            _logger.Information(StatusMessage);
            return;
        }
        try
        {
            string? savePath = _fileDialog.SaveFile("CSV Files (*.csv)|*.csv", "Export to CSV");
            if (string.IsNullOrEmpty(savePath))
            {
                return; // User cancelled
            }

            await Task.Run(() =>
            {
                using var writer = new StreamWriter(savePath);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                csv.WriteRecords(FilteredGridItems);
            });

            StatusMessage = $"Successfully exported {FilteredGridItems.Count} items to CSV";
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            StatusMessage = "$Failed to export data to {savePath}. Check the logs for details.";
        }
    }

    [RelayCommand]
    public void AddString()
    {
        try
        {
            // NOTE : THIS WORKS
            if (string.IsNullOrEmpty(NewString))
            {
                StatusMessage = "Please enter some text";
                return;
            }

            // STILL NEED TO ADD..
        }
        catch (Exception ex)
        {
            var message = ex is InvalidOperationException ? ex.Message : "Error adding string";
            StatusMessage = message;
            _logger.Error(message);
        }
    }


    partial void OnSearchTextChanged(string value)
    {
        Search();
    }
    private void ClearInput()
    {
        NewString = string.Empty;
    }
    private void ValidateSdbLoaded()
    {
        if (CurrentSdbModel == null)
        {
            throw new InvalidOperationException("Please load an SDB file first");
        }
    }
    private int CalculateNextOffset()
    {
        if (CurrentSdbModel?.DatabaseStrings == null || !CurrentSdbModel.DatabaseStrings.Any())
        {
            return 0x653254; // Starting offset matching the pattern
        }

        // Get the last string's offset
        var lastString = CurrentSdbModel.DatabaseStrings.Last();

        // Calculate next offset based on string length
        // Ensure we maintain the hex pattern by aligning to 0x100 boundaries
        int nextOffset = ((lastString.Offset + lastString.Length + 0xFF) & ~0xFF);

        // Log for debugging
        _logger.Debug($"Calculated offset: 0x{nextOffset:X} from last offset: 0x{lastString.Offset:X}");

        return nextOffset;
    }
}