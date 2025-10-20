namespace FindTheTiles.Model;

public static class ChangeType
{
    public static int[,] TextTilesToIntStates(TextTileButton[,] textTileButtons)
    {
        var states = new int[7, 7];
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
                states[i, j] = textTileButtons[i, j]._state;
        return states;
    }
}