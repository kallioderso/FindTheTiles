namespace FindTheTiles.Model;

public static class deserializer
{
    public static bool[,] Pattern(string pattern)
{
    var PatternArray = pattern.Split('|');
    
    var deserializedPattern = new bool[7, 7];
    for(int i=0; i<49; i++)
    {
        deserializedPattern[i / 7, i % 7] = (PatternArray[i] == "1");
    }
    return deserializedPattern;
}

public static int[,] States(string states)
{
    var statesArray = states.Split('|');
    var deserializedStates = new int[7, 7];
    for (int i = 0; i < 49; i++)
    {
        deserializedStates[i / 7, i % 7] = Convert.ToInt32(statesArray[i]);
    }
    return deserializedStates;
}
}