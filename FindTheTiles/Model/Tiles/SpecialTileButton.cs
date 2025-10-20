using System.Windows.Input;

namespace FindTheTiles.Model;

public class SpecialTileButton : ImageButton
{
    private int _state_internal { get; set; }
    public int _state { get { return _state_internal;} set { _state_internal = value; On_state_Changed(); } }
    private bool _visibility { get; set; }
    public bool _visibel { get { return _visibility; } set { _visibility = value; On_state_Changed(); }}
    public SpecialTileButton()
    {
        BackgroundColor = Colors.Transparent;
        BorderColor = Colors.Transparent;
        Source = "";
        IsEnabled = false;
        HeightRequest = 44;
        WidthRequest = 44;
    }

    private void On_state_Changed()
    {
        switch(_state_internal)
        {
            case 0: //seal
                this.Source = "siegel.png";
                break;
            case 1: //Star
                this.Source = "star.png";
                break;
        }

        IsVisible = _visibility;
    }
}