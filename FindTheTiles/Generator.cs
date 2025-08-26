namespace FindTheTiles;

public class Generator
{
    private static readonly Random _random = new();
    public static int GetNeighborCount(int row, int col, bool[,] _pattern)
    {
        // Array statt List für Nachbarn, vermeidet Allocations
        (int row, int col)[] neighbors = new (int, int)[4] {
            (row - 1, col), (row + 1, col), (row, col - 1), (row, col + 1)
        };
        int matches = 0;
        foreach (var (nRow, nCol) in neighbors)
        {
            if (nRow >= 0 && nRow < 7 && nCol >= 0 && nCol < 7 && _pattern[nRow, nCol])
                matches++;
        }
        return matches;
    }

    public static int GetNeighborCount(int row, int col, bool[,] pattern, Button[,] buttons)
    {
        // ...analog wie oben, aber mit Button-Handling...
        (int row, int col)[] neighbors = new (int, int)[4] {
            (row - 1, col), (row + 1, col), (row, col - 1), (row, col + 1)
        };
        int matches = 0;
        foreach (var (nRow, nCol) in neighbors)
        {
            if (nRow >= 0 && nRow < 7 && nCol >= 0 && nCol < 7)
            {
                if (pattern[nRow, nCol])
                    matches++;
                try
                {
                    if (buttons[nRow, nCol] != null && buttons[nRow, nCol].BackgroundColor.ToArgbHex() == Color.FromArgb("#F3F7FF").ToArgbHex())
                    {
                        buttons[nRow, nCol].BackgroundColor = Color.FromArgb("#FFF8DC");
                        buttons[nRow, nCol].BorderColor = Color.FromArgb("#FAFAAA");
                    }
                }
                catch (Exception e)
                { Console.WriteLine(e); }
            }
        }
        return matches;
    }
    
    public static (int row, int col) GetNextTile((int row, int col) currentTile, bool[,] pattern, double[,] borderReduction )
    {
        int upChance = 0;
        int downChance = 0;
        int leftChance = 0;
        int rightChance = 0;
        if(currentTile.row > 0)
            upChance = GetChance(currentTile.row - 1, currentTile.col, pattern, borderReduction);
        if(currentTile.row < 6)
            downChance = GetChance(currentTile.row + 1, currentTile.col, pattern, borderReduction);
        if(currentTile.col > 0)
            leftChance = GetChance(currentTile.row, currentTile.col - 1, pattern, borderReduction);
        if(currentTile.col < 6)
            rightChance = GetChance(currentTile.row, currentTile.col + 1, pattern, borderReduction);
        if (upChance + downChance + leftChance + rightChance == 0)
            return (currentTile.row, currentTile.col);
        var roll = _random.Next(1, upChance + downChance + leftChance + rightChance);
        if (roll <= upChance + downChance + leftChance)
        {
            if (roll <= upChance + downChance)
            {
                if (roll <= upChance)
                    return (currentTile.row - 1, currentTile.col);
                return (currentTile.row + 1, currentTile.col);
            }

            return (currentTile.row, currentTile.col - 1);
        }
        return (currentTile.row, currentTile.col + 1);
    }
    
    private static int GetChance(int row, int col, bool[,] _pattern, double[,] _borderReduction)
    {
        if (_pattern[row, col] == true)
            return 0;
        double neighborsReduction;
        switch (Generator.GetNeighborCount(row, col, _pattern))
        {
            default: neighborsReduction = 0.0; break;
            case 2: neighborsReduction = 0.05; break;
            case 3: neighborsReduction = 0.10; break;
            case 4: neighborsReduction = 0.15; break;
        }
        double borderReduction = _borderReduction[row, col];
        return Convert.ToInt32((0.25 - neighborsReduction - borderReduction)*100);
    }
    
    public static (bool[,] _pattern, List<(int row, int col)> _patternCoordinates) GenerateRandomPattern(bool[,] _pattern, List<(int row, int col)> _patternCoordinates, double[,] _borderReduction)
    {
        int patterntrys;
        int maxPatternTrys = 10; // Reduziert, um Deadlocks zu vermeiden
        int maxTileTrys = 15;    // Reduziert, um Deadlocks zu vermeiden
        do
        {
            _pattern = new bool [7,7];
            _patternCoordinates = new List<(int row, int col)>();
            patterntrys = 0;
            var patternTiles = _random.Next(10, 20);
            (int row, int col) currentTile = GetEntryPointOptimized(_borderReduction);
            _pattern[currentTile.row, currentTile.col] = true;
            _patternCoordinates.Add((currentTile.row, currentTile.col));
            patternTiles--;
            do
            {
                var lastTile = currentTile;
                currentTile = Generator.GetNextTile(currentTile, _pattern, _borderReduction);
                if (currentTile != lastTile)
                {
                    _pattern[currentTile.row, currentTile.col] = true;
                    _patternCoordinates.Add((currentTile.row, currentTile.col));
                    patternTiles--;
                }
                else
                {
                    patterntrys++;
                    currentTile = lastTile;   
                }
            } while (patternTiles != 0 && patterntrys <= maxTileTrys);
        } while (patterntrys > maxTileTrys && --maxPatternTrys > 0);
        return (_pattern, _patternCoordinates);
    }

    // Optimierte Entry-Point-Suche: Erstelle Liste aller möglichen Entry-Points und wähle zufällig
    private static (int, int) GetEntryPointOptimized(double[,] _borderReduction)
    {
        double searchedReduction;
        switch (_random.Next(1, 10))
        {
            case 1: case 2: searchedReduction = 0.07; break;
            case 3: case 4: case 5: searchedReduction = 0.03; break;
            default: searchedReduction = 0; break;
        }
        var possible = new List<(int, int)>();
        for (int row = 0; row < 7; row++)
            for (int col = 0; col < 7; col++)
                if (_borderReduction[row, col] == searchedReduction)
                    possible.Add((row, col));
        if (possible.Count == 0)
            return (0, 0); // Fallback
        return possible[_random.Next(possible.Count)];
    }
    
    public static (int startrow1, int startcol1, int startrow2, int startcol2) GetStartPoints(bool[,] _pattern)
    {
        (int nachbarn1, int nachbarn2) nachbarn = (0, 0);
        (int row, int col) StartPoint1 = (0, 0);
        (int row, int col) StartPoint2 = (0, 0);
        int trys;
        int maxTrys = 15; // Reduziert, um Deadlocks zu vermeiden
        do
        {
            trys = 0;
            do
            { nachbarn = (_random.Next(1, 4), _random.Next(1, 4));
            } while (nachbarn.nachbarn1 + nachbarn.nachbarn2 > 5 || nachbarn.nachbarn1 + nachbarn.nachbarn2 < 3);
            
            do
            {
                StartPoint1.row = _random.Next(0, 7);
                StartPoint1.col = _random.Next(0, 7);
                StartPoint2.row = _random.Next(0, 7);
                StartPoint2.col = _random.Next(0, 7);
                trys++;
                if (trys > maxTrys)
                    break;
            } while (Generator.GetNeighborCount(StartPoint1.row, StartPoint1.col, _pattern) != nachbarn.nachbarn1 || Generator.GetNeighborCount(StartPoint2.row, StartPoint2.col, _pattern) != nachbarn.nachbarn2 || !(_pattern[StartPoint1.row, StartPoint1.col]) || !(_pattern[StartPoint2.row, StartPoint2.col]));
        } while (trys > maxTrys);
        return (StartPoint1.row, StartPoint1.col, StartPoint2.row, StartPoint2.col);
    }
}