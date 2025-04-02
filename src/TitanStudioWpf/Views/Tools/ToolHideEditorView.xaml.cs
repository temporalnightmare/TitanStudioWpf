using Serilog;
using System.Windows;
using TitanStudioWpf.Core.Interfaces;
using TitanStudioWpf.ViewModels;

namespace TitanStudioWpf.Views.Tools;

public partial class ToolHideEditorView : Window
{
    public ToolHideEditorView(IFileDialog fileDialog, ILogger logger)
    {
        InitializeComponent();
        this.DataContext = new ToolHideEditorViewModel(fileDialog, logger);
    }
}
