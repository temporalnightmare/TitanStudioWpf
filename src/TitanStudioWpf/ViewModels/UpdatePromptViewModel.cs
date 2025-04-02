using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TitanStudioWpf.ViewModels;

public partial class UpdatePromptViewModel : ObservableObject
{
    [ObservableProperty]
    private string _updateMessage = "An update is available!";

    public event EventHandler? UpdateRequested;
    public event EventHandler? CancelRequested;


    [RelayCommand]
    private void Update()
    {
        UpdateRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Cancel()
    {
        CancelRequested?.Invoke(this, EventArgs.Empty);
    }
}
