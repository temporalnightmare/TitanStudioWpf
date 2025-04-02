using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TitanStudioWpf.ViewModels;

public partial class NotImplementedViewModel : ObservableObject
{
    #region Events
    public event EventHandler RequestClose;
    #endregion

    #region Properties
    [ObservableProperty]
    private string _featureNotImplementedTitle = "Not Implemented"; // Default value

    [ObservableProperty]
    private string _featureNotImplementedDescription = "This feature has not been implemented yet."; // Default value
    #endregion

    #region Commands
    [RelayCommand]
    public void Close()
    {
        // Raise the close event
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
    #endregion
}
