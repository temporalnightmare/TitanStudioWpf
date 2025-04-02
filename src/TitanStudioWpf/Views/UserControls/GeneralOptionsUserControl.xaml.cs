using TitanStudioWpf.ViewModels;
using System.Windows.Controls;

namespace TitanStudioWpf.Views.UserControls;

public partial class GeneralOptionsUserControl : UserControl
{
    public GeneralOptionsUserControl(GeneralOptionsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
