using TitanStudioWpf.ViewModels;
using System.Windows.Controls;

namespace TitanStudioWpf.Views.UserControls;

public partial class BakeryOptionsUserControl : UserControl
{
    public BakeryOptionsUserControl(BakeryOptionsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
