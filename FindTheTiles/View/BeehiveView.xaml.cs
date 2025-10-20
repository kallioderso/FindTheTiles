using FindTheTiles.Model;
using FindTheTiles.ViewModel;

namespace FindTheTiles.View;
public partial class BeehiveView
{
    private BeehiveViewModel _viewModel;
    public BeehiveView()
    {
        InitializeComponent();
        _viewModel = BindingContext as BeehiveViewModel;
        _viewModel.GenerateTiles(new Pattern());
        AddButtons();
    }

    private void AddButtons()
    {
        TilesGrid.Children.Clear();
        for (int i = 0; i < 7; i++)
        for (int j = 0; j < 7; j++)
        {
            var button = _viewModel._TileButtons[i, j];
            TilesGrid.Children.Add(button);
            TilesGrid.SetRow(button, button.Row);
            TilesGrid.SetColumn(button, button.Column);
        }
    }

    private void ExitButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}