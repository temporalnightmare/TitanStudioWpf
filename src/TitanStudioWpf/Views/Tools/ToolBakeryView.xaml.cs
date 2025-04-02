using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Windows;
using TitanStudioWpf.Core.Interfaces;
using TitanStudioWpf.Core.Models;
using TitanStudioWpf.ViewModels;

namespace TitanStudioWpf.Views.Tools;

public partial class ToolBakeryView : Window
{
    public ToolBakeryView(IFileDialog fileDialog, ILogger logger, BakeryOptionsViewModel bakeryOptionsViewModel, Option options)
    {
        InitializeComponent();
        this.DataContext = new ToolBakeryViewModel(fileDialog, logger, bakeryOptionsViewModel, options);
    }
}
