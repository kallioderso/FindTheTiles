using FindTheTiles.ViewModel;
using FindTheTiles.Model;

namespace FindTheTiles.View;

public partial class Level1View
{
    private Level1ViewModel _viewModel;

    public Level1View()
    {
        InitializeComponent();

        _viewModel = BindingContext as Level1ViewModel;
        _viewModel.GenerateTiles(new Pattern(), this);
        GenerateTiles();
    }

    private void GenerateTiles()
    {
        TilesGrid.Children.Clear();
        for (int i = 0; i < 7; i++)
        for (int j = 0; j < 7; j++)
        {
            var button = _viewModel._tiles[i, j];
            TilesGrid.Children.Add(button);
            TilesGrid.SetRow(button, button.Row);
            TilesGrid.SetColumn(button, button.Column);
            var specialButton = _viewModel._SpecialTiles[i, j];
            TilesGrid.Children.Add(specialButton);
            TilesGrid.SetRow(specialButton, button.Row);
            TilesGrid.SetColumn(specialButton, button.Column);
        }
    }
}
