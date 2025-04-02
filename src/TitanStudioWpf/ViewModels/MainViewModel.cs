using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using TitanStudioWpf.Core.Helpers;
using TitanStudioWpf.Views;
using TitanStudioWpf.Views.Tools;
using Velopack;

namespace TitanStudioWpf.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly BakeryOptionsViewModel _bakeryOptionsViewModel;
    private readonly GeneralOptionsViewModel _generalOptionsViewModel;

    public MainViewModel(ILogger logger, IServiceProvider serviceProvider, 
        BakeryOptionsViewModel bakeryOptionsViewModel, GeneralOptionsViewModel generalOptionsViewModel)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _bakeryOptionsViewModel = bakeryOptionsViewModel;
        _generalOptionsViewModel = generalOptionsViewModel;

        AppVersion = AppConfigHelper.GetAppVersionFromConfig();

        _logger.Information("[Main App] TitanSTUDIO launched");
    }

    public bool? DialogResult { get; private set; }

    [ObservableProperty]
    private string _updateMessage = "An update is available!";

    [ObservableProperty]
    private string _appVersion = string.Empty;

    [RelayCommand]
    private void Update()
    {
        DialogResult = true;
        CloseWindow();
    }

    [RelayCommand]
    private void Cancel()
    {
        DialogResult = false;
        CloseWindow();
    }

    private void CloseWindow()
    {
        if (App.Current.Windows.OfType<Views.UpdatePromptView>().FirstOrDefault() is Views.UpdatePromptView window)
        {
            window.Close();
        }
    }

    [RelayCommand]
    public void ShowOptions()
    {
        var view = _serviceProvider.GetRequiredService<OptionsView>();
        view.ShowDialog();
    }

    [RelayCommand]
    public void ShowAbout()
    {
        var view = _serviceProvider.GetRequiredService<AboutView>();
        view.ShowDialog();
    }

    [RelayCommand]
    public void ShowBakeryTool()
    {
        var view = _serviceProvider.GetRequiredService<ToolBakeryView>();
        view.ShowDialog();
    }

    [RelayCommand]
    public void ShowHasherTool()
    {
        var view = _serviceProvider.GetRequiredService<ToolHasherView>();
        view.ShowDialog();
    }

    [RelayCommand]
    public void ShowHideEditorTool()
    {
        var view = _serviceProvider.GetRequiredService<ToolHideEditorView>();
        view.ShowDialog();
    }

    [RelayCommand]
    public void ShowJSFBScannerTool()
    {
        var view = _serviceProvider.GetRequiredService<ToolJSFBScannerView>();
        view.ShowDialog();  
    }

    [RelayCommand]
    public void ShowStringDbManagerTool()
    {
        var view = _serviceProvider.GetRequiredService<ToolStringDbManagerView>();
        view.ShowDialog();
    }

    [RelayCommand]
    public void ShowJSFBFlatbufferTestTool()
    {
        //var view = _serviceProvider.GetRequiredService<ToolFlatbufferTestView>();
        var view = _serviceProvider.GetRequiredService<NotImplementedView>();
        view.ShowDialog();
    }

    [RelayCommand]
    public void LaunchGame()
    {
        try
        {
            if (string.IsNullOrEmpty(_generalOptionsViewModel.GamePath))
            {
                _logger.Error("Game path is null or empty");
                return;
            }

            if (!File.Exists(_generalOptionsViewModel.GamePath))
            {
                _logger.Error($"Game executable not found at path: {_generalOptionsViewModel.GamePath}");
                return;
            }

            _logger.Error($"Launching game: {_generalOptionsViewModel.GamePath}");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = _generalOptionsViewModel.GamePath,
                UseShellExecute = true,  // This allows the process to start in a new window
                WorkingDirectory = Path.GetDirectoryName(_generalOptionsViewModel.GamePath) // Set working directory to game folder
            };
            using (Process process = new Process { StartInfo = startInfo })
            {
                bool started = process.Start();
                _logger.Information($"Process started: {started}");
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"Error launching game: {ex.Message}");
            _logger.Verbose($"Stack trace: {ex.StackTrace}");
        }
    }

    [RelayCommand]
    private async Task UpdateApp()
    {
        try
        {
            string urlOrURI = "Add your own location.";
            var mgr = new UpdateManager(urlOrURI);

            var newVersion = await mgr.CheckForUpdatesAsync();
            if (newVersion == null)
                return;

            _logger.Information("Update command executed at: {Time}", DateTime.Now);
            _logger.Information("{Time} New version found! {newVersion}", DateTime.Now, newVersion);
  
            var updatePromptViewModel = new UpdatePromptViewModel();
            var updatePromptView = new UpdatePromptView(updatePromptViewModel);

            if (updatePromptView.ShowDialog() == true)
            {
                _logger.Information("{Time} End-user clicked Yes", DateTime.Now);

                await mgr.DownloadUpdatesAsync(newVersion);
                mgr.ApplyUpdatesAndRestart(newVersion);
            }
           
        }
        catch (Exception ex)
        {
            _logger.Error("{Time} Error checking for updates! {ex}", DateTime.Now, ex);
        } 
    }


}
