using Serilog;
using System.Windows;
using TitanStudioWpf.Core.Interfaces;
using TitanStudioWpf.ViewModels;

namespace TitanStudioWpf.Views.Tools;

public partial class ToolJSFBScannerView : Window
{
    public ToolJSFBScannerView(IFileDialog fileDialog, ILogger logger)
    {
        InitializeComponent();
        this.DataContext = new ToolJSFBScannerViewModel(fileDialog, logger);
    }
}
