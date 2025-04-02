using System.Windows;
using TitanStudioWpf.ViewModels;

namespace TitanStudioWpf;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}