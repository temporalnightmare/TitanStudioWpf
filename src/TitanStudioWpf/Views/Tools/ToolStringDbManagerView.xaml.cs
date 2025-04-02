using Serilog;
using System.Windows;
using TitanStudioWpf.Core.Interfaces;
using TitanStudioWpf.ViewModels;

namespace TitanStudioWpf.Views.Tools;

public partial class ToolStringDbManagerView : Window
{
    public ToolStringDbManagerView(IServiceProvider serviceProvider, IFileDialog fileDialog, ILogger logger)
    {
        InitializeComponent();
        this.DataContext = new ToolStringDbManagerViewModel(serviceProvider, fileDialog, logger);
    }
}
