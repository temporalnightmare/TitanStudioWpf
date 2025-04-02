using System.Windows;
using TitanStudioWpf.ViewModels;

namespace TitanStudioWpf.Views;

public partial class UpdatePromptView : Window
{
    private readonly UpdatePromptViewModel _viewModel;

    public UpdatePromptView(UpdatePromptViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        // Subscribe to ViewModel events
        _viewModel.UpdateRequested += (s, e) =>
        {
            DialogResult = true;
            Close();
        };

        _viewModel.CancelRequested += (s, e) =>
        {
            DialogResult = false;
            Close();
        };
    }
}
