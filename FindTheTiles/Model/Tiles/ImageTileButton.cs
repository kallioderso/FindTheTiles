using System.Windows.Input;

namespace FindTheTiles.Model;

public class ImageTileButton : ImageButton
{
    private int _state_internal { get; set; }
    public int _state { set { _state_internal = value; On_state_Changed(); } }
    private bool _bombActiv { get; set; }
    public bool _bomb { set { _bombActiv = value; On_state_Changed();}}
    public int _neighbor { set {_neighbor_internal = value;} }
    private int _neighbor_internal { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }

    public ImageTileButton()
    {
        this.WidthRequest = 66;
        this.HeightRequest = 66;
        Background = Colors.Transparent;
        BorderColor = Colors.Transparent;
        _state_internal = 0;
        _bombActiv = false;
    }

    private void On_state_Changed()
    {
        switch(_state_internal)
        {
            case 0: //Basic Mode
                this.Source = !_bombActiv ? "wabe_versiegelt.png" : "wabe_versiegelt_bombe.png";
                break;
            case 1: //StartTile
                this.Source = !_bombActiv ? "wabe_start.png" : "wabe_start_bombe.png";
                break;
            case 2: //Tile-Field
                this.Source = !_bombActiv ? $"larve{_neighbor_internal}.png" : $"larve{_neighbor_internal}_bombe.png";
                break;
            case 3: //Non-Tile-Field
                this.Source = !_bombActiv ? $"larven_attrape{_neighbor_internal}.png" : $"larven_attrape{_neighbor_internal}_bombe.png";
                break;
            case 4: //Searcher-Field
                this.Source = !_bombActiv ? "wabe_searcher.png" : "wabe_searcher_bombe.png";
                break;
        }
    }

}