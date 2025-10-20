namespace FindTheTiles.Model;

public class Pattern
{
    private Random _random = new Random();
    private double[,] _borderReduction = 
    {
        { 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03 },
        { 0.03, 0.05, 0.05, 0.05, 0.05, 0.05, 0.03 },
        { 0.03, 0.05, 0.08, 0.08, 0.08, 0.05, 0.03 },
        { 0.03, 0.05, 0.08, 0.08, 0.08, 0.05, 0.03 },
        { 0.03, 0.05, 0.08, 0.08, 0.08, 0.05, 0.03 },
        { 0.03, 0.05, 0.05, 0.05, 0.05, 0.05, 0.03 },
        { 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03 },
    };

    private double[] _neighborsReduction = { 0.0, 0.05, 0.10, 0.15 };
    public bool[,] IsPattern { get; private set; }
    public int[,] TilesNeighbors { get; private set; }
    public int TilesAmount { get; private set; }
    private int _minTiles = 10;
    private int _maxTiles = 20;
    private int _startTilesAmount = 2;
    public List<(int row, int col)> StartTiles { get; private set; }

    /// <summary>
    /// Konstruktoren
    /// </summary>
    public Pattern()
    {
        GetEverything();
    }
    public Pattern(int startTiles)
    { 
        _startTilesAmount = startTiles; 
        GetEverything();
    }

    public Pattern(int minTiles, int maxTiles)
    {
        _minTiles = minTiles;
        _maxTiles = maxTiles;
        GetEverything();
    }

    public Pattern(int minTiles, int maxTiles, int startTiles)
        : this(minTiles, maxTiles)
    {
        _startTilesAmount = startTiles;
        GetEverything();
    }

    public Pattern (bool[,] restoredPattern)
    {
        RestoreEverything(restoredPattern);
    }

    public void ChangeDifficulty(double[,] borderReduction, double[] neighborReduction) => ChangeDiffi(borderReduction, neighborReduction);

    private void ChangeDiffi(double[,] borderReduction, double[] neighborReduction)
    {
        _borderReduction = borderReduction;
        _neighborsReduction = neighborReduction;
        GetEverything();
    }

    private void GetEverything()
    {
        IsPattern = GetPattern();
        TilesNeighbors = GetEveryNeighbor();
        StartTiles = GetStartTiles();

        var fourneighbors = 0;
        foreach (var neighbor in TilesNeighbors)
            if (neighbor == 4)
                fourneighbors++;

        if(fourneighbors > 2)
            GetEverything();
    }

    private void RestoreEverything(bool[,] pattern)
    {
        IsPattern = pattern;
        TilesNeighbors = GetEveryNeighbor();
    }
    
    /// <summary>
    /// Generiert das Muster
    /// </summary>
    private bool[,] GetPattern()
    {
        bool[,] pattern;
        int patternTries;
        int maxPatternTries = 10;
        int maxTileTries = 15;
        do
        {
            pattern = new bool[7, 7];
            patternTries = 0;
            var patternTiles = _random.Next(_minTiles, _maxTiles);
            TilesAmount = patternTiles;
            (int row, int col) currentTile = GetEntryPoint();
            pattern[currentTile.row, currentTile.col] = true;
            patternTiles--;
            do
            {
                var lastTile = currentTile;
                currentTile = GetNextTile(currentTile, pattern);
                if (currentTile != lastTile)
                {
                    pattern[currentTile.row, currentTile.col] = true;
                    patternTiles--;
                }
                else
                {
                    patternTries++;
                    currentTile = lastTile;
                }
            } while (patternTiles != 0 && patternTries <= maxTileTries);
        } while (patternTries > maxTileTries && --maxPatternTries > 0);
        return pattern;
    }

    private (int, int) GetEntryPoint() => (_random.Next(1, 7), _random.Next(1, 7));

    public (int row, int col) GetNextTile((int row, int col) currentTile, bool[,] pattern)
    {
        int upChance = 0;
        int downChance = 0;
        int leftChance = 0;
        int rightChance = 0;
        if (currentTile.row > 0)
            upChance = GetChance(currentTile.row - 1, currentTile.col, pattern);
        if (currentTile.row < 6)
            downChance = GetChance(currentTile.row + 1, currentTile.col, pattern);
        if (currentTile.col > 0)
            leftChance = GetChance(currentTile.row, currentTile.col - 1, pattern);
        if (currentTile.col < 6)
            rightChance = GetChance(currentTile.row, currentTile.col + 1, pattern);
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

    private int GetChance(int row, int col, bool[,] pattern)
    {
        if (pattern[row, col])
            return 0;
        double neighborsReduction;
        neighborsReduction = _neighborsReduction[GetNeighborCount(row, col, pattern)-1];
        double borderReduction = _borderReduction[row, col];
        return Convert.ToInt32((0.25 - neighborsReduction - borderReduction) * 100) > 0 ? Convert.ToInt32((0.25 - neighborsReduction - borderReduction) * 100) : 1;
    }

    private int GetNeighborCount(int row, int col, bool[,] _pattern)
    {
        (int row, int col)[] neighbors = new (int, int)[4] {
            (row - 1, col), (row + 1, col), (row, col - 1), (row, col + 1)
        };
        int matches = 0;
        foreach (var (nRow, nCol) in neighbors)
            if (nRow >= 0 && nRow < 7 && nCol >= 0 && nCol < 7 && _pattern[nRow, nCol])
                matches++;
        return matches;
    }

    /// <summary>
    /// Zählt die Nachbarn für jedes Feld
    /// </summary>
    private int[,] GetEveryNeighbor()
    {
        int[,] neighbors = new int[7, 7];
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                (int row, int col)[] neighborCoords = new (int, int)[4] { (i - 1, j), (i + 1, j), (i, j - 1), (i, j + 1) };
                int matches = 0;
                foreach (var (nRow, nCol) in neighborCoords)
                    if (nRow >= 0 && nRow < 7 && nCol >= 0 && nCol < 7 && IsPattern[nRow, nCol])
                        matches++;
                neighbors[i, j] = matches;
            }
        }
        return neighbors;
    }

    /// <summary>
    /// Sucht die StartTiles ohne Duplikate
    /// </summary>
    private List<(int row, int col)> GetStartTiles()
    {
        var startTiles = new List<(int row, int col)>();
        int maxTries = 1000;
        int tries = 0;
        while (startTiles.Count < _startTilesAmount && tries < maxTries)
        {
            int r, c;
            do
            {
                r = _random.Next(0, 7);
                c = _random.Next(0, 7);
            } while (!IsPattern[r, c]);
            if (!startTiles.Contains((r, c)))
                startTiles.Add((r, c));
            tries++;
        }
        return startTiles;
    }
}
