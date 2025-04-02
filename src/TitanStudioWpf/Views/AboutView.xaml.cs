using System.Windows;
using TitanStudioWpf.ViewModels;

namespace TitanStudioWpf.Views;

public partial class AboutView : Window
{
    public AboutView(AboutViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        // Subscribe to the close request
        if (viewModel is AboutViewModel vm)
        {
            vm.RequestClose += (s, e) => Close();
        }
    }
}
