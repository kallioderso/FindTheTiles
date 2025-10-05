namespace FindTheTiles.Model;

public class TileButton : ImageButton
{
    private int _state_internal { get; set; }
    public int _state { set { _state_internal = value; On_state_Changed();} }
    public int _neighbor { set {_neighbor_internal = value;} }
    private int _neighbor_internal { set { _text._neighbor = value;} }
    private TileButtonText _text;

    public TileButton(int status)
    {
        _state = status;
        _text = new TileButtonText();
        _text.Background = Colors.Transparent;
        this.AddLogicalChild(_text);
    }

    private void On_state_Changed()
    {
        switch(_state_internal)
        {
            case 0: //Basic Mode
                this.Source = "wabe_normal.svg";
                break;
            case 1: //StartTile
                this.Source = "wabe_start.svg";
                break;
            case 2: //Tile-Field
                this.Source = "wabe_richtig.svg";
                break;
            case 3: //Non-Tile-Field
                this.Source = "wabe_falsch.svg";
                break;
            case 4: //Searcher-Field
                this.Source = "wabe_searcher.svg";
                break;
            case 5: //Bomb-Field
                this.Source = "wabe_bombe.svg";
                break;
        }
    }

}