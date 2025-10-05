namespace FindTheTiles.Model;

public class TileButtonText : Label
{
    private int _neighbor_internal { get; set; }
    public int _neighbor { set { _neighbor_internal = value; got_Number(); } }

    private void got_Number()
    {
        this.Text = _neighbor_internal.ToString();
    }
}