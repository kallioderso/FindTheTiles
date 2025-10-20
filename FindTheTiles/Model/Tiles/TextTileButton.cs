namespace FindTheTiles.Model;

public class TextTileButton : Button
{
    private int _state_internal { get; set; }
    public int _state { get { return _state_internal; } set { _state_internal = value; On_state_Changed(); } }
    private bool _bombActiv { get; set; }
    public bool _bomb { set { _bombActiv = value; On_state_Changed();}}
    public int _neighbor { set {_neighbor_internal = value;} }
    private int _neighbor_internal { get; set; }
    public bool _searcher { set { _searcherActiv = value; On_state_Changed(); }}
    private bool _searcherActiv { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }

    public TextTileButton(int Width, int Height)
    {
        HeightRequest = Height;
        WidthRequest = Width;
        BackgroundColor = Color.FromArgb("#FFFFFF");
        BorderColor = Color.FromArgb("#a997d7");
        TextColor = Color.FromArgb("#000000");
        BorderWidth = 2;
        CornerRadius = 16;
        Shadow = new Shadow
        {
            Brush = new SolidColorBrush(Color.FromArgb("#674daaff")),
            Offset = new Point(0, 2),
            Radius = 6,
            Opacity = 0.4f
        };
        Padding = 0;
    }

    private void On_state_Changed()
    {
        switch(_state_internal)
        {
            case 0:
                BackgroundColor = Color.FromArgb("#FFFFFF");
                BorderColor = Color.FromArgb(_searcherActiv ? "#FA026E" : _bombActiv ? "#474954" : "#a997d7");
                Text = "";
                break;
            case 1:
                BackgroundColor = Color.FromArgb("#FFFFFF");
                BorderColor = Color.FromArgb("#4CAF50");
                Text = "";
                break;
            case 2:
                BackgroundColor = Color.FromArgb("#D0E0FF");
                BorderColor = Color.FromArgb(_bombActiv ? "#474954" : "#A0B8FF");
                Shadow.Opacity = 0.2f;
                Shadow.Brush = new SolidColorBrush(Color.FromArgb("#B0C4FF"));
                Text = $"{_neighbor_internal}";
                break;
            case 3:
                BackgroundColor = Color.FromArgb("#FFCDD2");
                BorderColor = Color.FromArgb(_bombActiv ? "#474954" : "#E57373");
                Shadow.Opacity = 0.2f;
                Shadow.Brush = new SolidColorBrush(Color.FromArgb("#FF8A80"));
                Text = $"{_neighbor_internal}";
                break;
            case 4:
                BackgroundColor = Color.FromArgb("#FFF8DC");
                BorderColor = Color.FromArgb(_searcherActiv ? "#FA026E" : _bombActiv ? "#474954" : "#FAFAAA");
                Text = "";
                break;
        }
    }
}