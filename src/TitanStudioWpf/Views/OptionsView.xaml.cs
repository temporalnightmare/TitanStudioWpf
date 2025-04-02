using System.Windows;
using TitanStudioWpf.Core.Models;
using TitanStudioWpf.ViewModels;

namespace TitanStudioWpf.Views;

public partial class OptionsView : Window
{
    public OptionsView(OptionsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        // Subscribe to the close request
        if (viewModel is OptionsViewModel vm)
        {
            vm.RequestClose += (s, e) => Close();
        }
    }

    private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {

        if (DataContext is OptionsViewModel viewModel && e.NewValue is OptionItems selectedItem)
        {
            
            viewModel.SelectOptionItemCommand.Execute(selectedItem);
        }
    }
}
