using System.Windows;
using TitanStudioWpf.ViewModels;

namespace TitanStudioWpf.Views;

public partial class NotImplementedView : Window
{
    public NotImplementedView(NotImplementedViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        // Subscribe to the close request
        if (viewModel is NotImplementedViewModel vm)
        {
            vm.RequestClose += (s, e) => Close();
        }
    }
}
