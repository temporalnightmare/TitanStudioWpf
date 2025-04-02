using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using TitanStudioWpf.Core.Interfaces;
using TitanStudioWpf.Core.Models;
using TitanStudioWpf.Core.Services;
using TitanStudioWpf.ViewModels;
using TitanStudioWpf.Views;
using TitanStudioWpf.Views.Tools;
using Velopack;

namespace TitanStudioWpf;

public class Program
{
    // Since WPF has an "automatic" Program.Main, we need to create our own.
    // In order for this to work, you must also add the following to your .csproj:
    // <StartupObject>CSharpWpf.App</StartupObject>

    private static Option LoadSettings()
    {
        var defaultOption = new Option { LogPath = Assembly.GetExecutingAssembly().Location };
        
        string settingsFile = "TitanStudioSettings.json";

        if (File.Exists(settingsFile))
        {
            string jsonString = File.ReadAllText(settingsFile);
            var options = JsonSerializer.Deserialize<Option>(jsonString);
            return options ?? defaultOption;
        }
        
        return defaultOption;
    }

    [STAThread]
    private static void Main(string[] args)
    {
        try
        {
            // It's important to Run() the VelopackApp as early as possible in app startup.
            VelopackApp.Build().Run();

            // Load settings first
            var settings = LoadSettings();

            var appPath = AppContext.BaseDirectory;
            var logDirectory = Path.Combine(appPath, "Logs");

            // Ensure the Logs directory exists
            Directory.CreateDirectory(logDirectory);
            
            var logPath = string.IsNullOrEmpty(settings.LogPath) ? Path.Combine(logDirectory, "app.log") : settings.LogPath;
       
            // Set up logging
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File($@"{logPath}",            
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 31,
                fileSizeLimitBytes: 1024 * 1024 * 10,
                flushToDiskInterval: TimeSpan.FromSeconds(1)
                ).CreateLogger();

            // Configure Dependency Injection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Use 'using' to ensure disposal of the service provider
            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                // Start the WPF application
                var app = new App();
                app.InitializeComponent();

                var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();

                // Apply font from settings if available
                if (!string.IsNullOrEmpty(settings.FontName))
                {
                    App.UpdateGlobalFont(settings.FontName);
                }

                app.Run();
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show("Unhandled exception: " + ex.ToString());
        }       
    }

    private static void ConfigureServices(IServiceCollection services)
    {

        // Register Serilog
        services.AddSingleton(Log.Logger);


        // Load and register settings
        var settings = LoadSettings();
        services.AddSingleton(settings);


        // Register Views
        services.AddTransient<MainWindow>();
        services.AddTransient<OptionsView>();
        services.AddTransient<ToolBakeryView>();
        services.AddTransient<ToolHasherView>();
        services.AddTransient<ToolHideEditorView>();
        services.AddTransient<ToolJSFBScannerView>();
        services.AddTransient<ToolStringDbManagerView>();
        services.AddTransient<ToolFlatbufferTestView>();
        services.AddTransient<AboutView>();
        services.AddTransient<UpdatePromptView>();
        services.AddTransient<NotImplementedView>();

        // Register ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<BakeryOptionsViewModel>();
        services.AddSingleton<GeneralOptionsViewModel>();
        services.AddSingleton<OptionsViewModel>();

        services.AddSingleton(provider =>
        {
            var logger = provider.GetRequiredService<ILogger>();

            var generalOptionsViewModel = new GeneralOptionsViewModel(logger)
            {
                GamePath = settings.GamePath,
                LogPath = settings.LogPath
            };
            return generalOptionsViewModel;
        });


        services.AddSingleton(provider =>
        {
            var logger = provider.GetRequiredService<ILogger>();
            var bakeryOptionsViewModel = new BakeryOptionsViewModel(logger)
            {
                CakeToolPath = settings.CakeToolPath,
                ModsFolderPath = settings.ModsFolderPath,
                BakeFolderPath = settings.BakeFolderPath,
            };
            return bakeryOptionsViewModel;
        });


        services.AddSingleton<ToolBakeryViewModel>();
        services.AddSingleton<ToolHasherViewModel>();
        services.AddSingleton<ToolHideEditorViewModel>();
        services.AddSingleton<ToolJSFBScannerViewModel>();
        services.AddSingleton<ToolStringDbManagerViewModel>();
        services.AddSingleton<ToolFlatbufferTestViewModel>();
        services.AddSingleton<AboutViewModel>();
        services.AddSingleton<UpdatePromptViewModel>();
        services.AddSingleton<NotImplementedViewModel>();

        // Register Services
        services.AddSingleton<IFileDialog, FileDialog>();
        // DECREPATED services.AddSingleton<ILogger, Logger>();
    }
}
