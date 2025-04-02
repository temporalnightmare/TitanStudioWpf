using Microsoft.Extensions.Logging;
using System.Windows;
using TitanStudioWpf.ViewModels;

namespace TitanStudioWpf.Views.Tools;


public partial class ToolHasherView : Window
{

    public ToolHasherView(ToolHasherViewModel viewModel)
    {
        InitializeComponent();
        this.DataContext = viewModel;
    }
}
