namespace FindTheTiles.Model;

public static class serializer
{
    public static string Pattern(bool[,] pattern)
{
    var list = new List<string>();
    for (int i = 0; i < 7; i++)
        for (int j = 0; j < 7; j++)
            list.Add(Convert.ToInt32(pattern[i, j]).ToString());
    return string.Join("|", list);
}

    public static string States(int[,] states)
    {
        var list = new List<string>();
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
                list.Add(states[i, j].ToString());
        return string.Join("|", list);
    }
}