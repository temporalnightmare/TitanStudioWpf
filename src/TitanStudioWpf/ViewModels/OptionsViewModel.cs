using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;
using TitanStudioWpf.Core.Models;
using TitanStudioWpf.Views.UserControls;

namespace TitanStudioWpf.ViewModels;

public partial class OptionsViewModel : ObservableObject
{
    public ObservableCollection<OptionItems> OptionItems { get; set; }
    private readonly BakeryOptionsViewModel _bakeryOptionsViewModel;
    private readonly GeneralOptionsViewModel _generalOptionsViewModel;
    private readonly Option _options;
    private readonly ILogger _logger;
    public event EventHandler RequestClose;

    public OptionsViewModel(ILogger logger, BakeryOptionsViewModel bakeryoptionViewModel, GeneralOptionsViewModel generalOptionsViewModel, Option options)
    {
        _bakeryOptionsViewModel = bakeryoptionViewModel;
        _generalOptionsViewModel = generalOptionsViewModel;
        _options = options;
        _logger = logger;

        _logger.Information("[Options] Options opened");

        // Initialize BakeryOptionsViewModel with settings
        _bakeryOptionsViewModel.CakeToolPath = _options.CakeToolPath;
        _bakeryOptionsViewModel.ModsFolderPath = _options.ModsFolderPath;
        _bakeryOptionsViewModel.BakeFolderPath = _options.BakeFolderPath;

        OptionItems = new ObservableCollection<OptionItems>
        {
            new OptionItems("Environment", new ObservableCollection<OptionItems>
            {
                new OptionItems("General"),
                new OptionItems("Baking")
            })
        };
    }

    public ObservableCollection<OptionsViewModel> Children { get; set; }
    public string Name { get; set; }

    [ObservableProperty]
    private UserControl _selectedContent;

    [RelayCommand]
    private void SelectOptionItem(OptionItems? item)
    {
        _logger.Information(@"[Options] Item Selected: {0}", item.Name);

        if (item?.Name == null) return;

        // Ensure view models are not null before creating controls
        SelectedContent = item.Name switch
        {
            "General" when _generalOptionsViewModel != null => new GeneralOptionsUserControl(_generalOptionsViewModel),
            "Baking" when _bakeryOptionsViewModel != null => new BakeryOptionsUserControl(_bakeryOptionsViewModel),
            _ => null
        };

        if (SelectedContent == null)
        {
            _logger.Warning(@"[Options] Failed to create user control for {item} - required ViewModel was null", item.Name);
        }
    }

    [RelayCommand]
    public void SaveOptions()
    {
        _logger.Information("[Options] Save clicked");

        // Write to a JSON
        var option = new Option
        {
            CakeToolPath = _bakeryOptionsViewModel.CakeToolPath,         
            ModsFolderPath = _bakeryOptionsViewModel.ModsFolderPath,
            BakeFolderPath = _bakeryOptionsViewModel.BakeFolderPath,
            GamePath = _generalOptionsViewModel.GamePath,
            LogPath = _generalOptionsViewModel.LogPath,
            FontName = _generalOptionsViewModel.FontName
        };

        string jsonFile = "TitanStudioSettings.json";

        using (FileStream fs = File.Create(jsonFile))
        {
            JsonSerializer.Serialize(fs, option, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        _logger.Information("[Options] Saved options to {0}", jsonFile);

        App.UpdateGlobalFont(_generalOptionsViewModel.FontName);

        // Raise the close event
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    public void CancelSaveOptions()
    {
        // Raise the close event
        RequestClose?.Invoke(this, EventArgs.Empty);

        _logger.Information("[Options] Cancel clicked");
    }




}
