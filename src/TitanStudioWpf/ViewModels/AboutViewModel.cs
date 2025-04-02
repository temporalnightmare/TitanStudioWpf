using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TitanStudioWpf.Core.Helpers;
namespace TitanStudioWpf.ViewModels;

public partial class AboutViewModel : ObservableObject
{
    public event EventHandler RequestClose;

    [ObservableProperty]
    private string _appName;

    [ObservableProperty]
    private string _appVersion;

    public AboutViewModel()
    {
        AppName = AppConfigHelper.GetAppNameFromConfig();
        AppVersion = AppConfigHelper.GetAppVersionFromConfig();
    }

    [RelayCommand]
    public void Close()
    {
        // Raise the close event
        RequestClose?.Invoke(this, EventArgs.Empty);
    }


}
