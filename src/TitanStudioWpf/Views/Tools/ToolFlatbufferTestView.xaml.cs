using Serilog;
using System.Windows;
using TitanStudioWpf.Core.Interfaces;
using TitanStudioWpf.ViewModels;

namespace TitanStudioWpf.Views.Tools;

public partial class ToolFlatbufferTestView : Window
{
    private readonly ToolFlatbufferTestViewModel _viewModel;

    public ToolFlatbufferTestView(IFileDialog fileDialog, ILogger logger)
    {
        InitializeComponent();
        _viewModel = new ToolFlatbufferTestViewModel(fileDialog, logger);
        this.DataContext = _viewModel;

        // Subscribe to the close request
       // if (_viewModel is ToolFlatbufferTestViewModel vm)
       // {
       //     vm.RequestClose += (s, e) => Close();
       // }
    }
}
