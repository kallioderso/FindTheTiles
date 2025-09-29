using System;

namespace FindTheTiles;

public partial class GameView
{
    private const int GridSize = 7;

    // Private Felder
    private bool[,] _pattern = new bool[GridSize, GridSize];
    private readonly Button[,] _buttons = new Button[GridSize, GridSize];
    private List<(int row, int col)> _patternCoordinates = new();
    private List<(int row, int col)> _clickedTiles = new();

    private readonly double[,] _borderReduction =
    {
        { 0.07, 0.07, 0.07, 0.07, 0.07, 0.07, 0.07 },
        { 0.07, 0.03, 0.03, 0.03, 0.03, 0.03, 0.07 },
        { 0.07, 0.03, 0, 0, 0, 0.03, 0.07 },
        { 0.07, 0.03, 0, 0, 0, 0.03, 0.07 },
        { 0.07, 0.03, 0, 0, 0, 0.03, 0.07 },
        { 0.07, 0.03, 0.03, 0.03, 0.03, 0.03, 0.07 },
        { 0.07, 0.07, 0.07, 0.07, 0.07, 0.07, 0.07 },
    };

    private int _currentScore;
    private bool _isGameOver;
    private int _foundPatternTiles;
    private int _totalPatternTiles;
    private static readonly Random Random = new();
    private int _tries;

    // Multiplikator-Logik
    private int _completedPatterns;
    private double _multiplier;

    // Neuer Zustand für Bombenmodus
    private bool _bombMode;
    private bool _bombProcessing;
    private (int startrow1, int startcol1, int startrow2, int startcol2) _startPoints;

    public GameView(int score = 0, int completedPatterns = 0, double multiplier = 1.0, int tries = 1)
    {
        InitializeComponent();
        NavigationPage.SetHasBackButton(this, false);

        if (Preferences.Get("Resume", false))
        {
            _currentScore = 0;
            _completedPatterns = Preferences.Get("ResumeCompletedPatterns", 0);
            _multiplier = Preferences.Get("ResumeMultiplier", 1.0);
            _tries = 0;
        }
        else
        {
            _currentScore = score;
            _completedPatterns = completedPatterns;
            _multiplier = multiplier >= Preferences.Get("Multiplyer", 1.0) ? multiplier : Preferences.Get("Multiplyer", 1.0);
            _tries = Preferences.Get("Tries", 1);
        }

        GenerateTiles();
        UpdateScoreLabel();
        UpdateMultiplierLabel();
        UpdateTryLabel();
        SetLanguage();
        UpdateItems();
    }

    private void SetLanguage()
    {
        ProgressLabel.Text = LanguageManager.GetText("Progress");
        CurrentScoreLabelTitle.Text = LanguageManager.GetText("Score");
        MultiplierLabelTitle.Text = LanguageManager.GetText("Multiplyer");
        TriesLabelTitle.Text = LanguageManager.GetText("RemainingAttempts");
        ToolTipProperties.SetText(ExitButton, LanguageManager.GetText("TooltipExit"));
        ToolTipProperties.SetText(FortschritsFrame, $"{LanguageManager.GetText("Progress")}: {_foundPatternTiles} / {_totalPatternTiles}");
        ToolTipProperties.SetText(ScoreFrame, $"{LanguageManager.GetText("Score")}: {_currentScore}");
        ToolTipProperties.SetText(MultiplierFrame, $"{LanguageManager.GetText("Multiplyer")}: x{_multiplier}");
        ToolTipProperties.SetText(FailureFrame, $"{LanguageManager.GetText("RemainingAttempts")}: {_tries}");
        ToolTipProperties.SetText(HelpButton, $"{LanguageManager.GetText("TooltipTutorial")}");
        ToolTipProperties.SetText(LanguageButton, LanguageManager.GetText("TooltipLanguage"));
        ToolTipProperties.SetText(SearchButton, $"{LanguageManager.GetText("TooltipSearcher")}");
        ToolTipProperties.SetText(BombButton, $"{LanguageManager.GetText("TooltipBomb")}");
        if (Preferences.Get("language", "en") == "en")
            LanguageButton.Source = "english.png";
        else if (Preferences.Get("language", "en") == "de")
            LanguageButton.Source = "german.png";
        else if (Preferences.Get("language", "en") == "fr")
            LanguageButton.Source = "frensh.png";
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        string orientation = height > width ? "Portrait" : "Landscape";
        VisualStateManager.GoToState(MainStack, orientation);
    }

    private void ExitButton_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("ResumePattern", SerializePattern(_pattern));
        Preferences.Set("ResumeCordinates", SerializeCordinates(_patternCoordinates));
        Preferences.Set("ResumeCordinatesClicked", SerializeCordinates(_clickedTiles));
        Preferences.Set("ResumeCompletedPatterns", _completedPatterns);
        Preferences.Set("ResumeMultiplier", _multiplier);
        Preferences.Set("ResumeScore", _currentScore);
        Preferences.Set("ResumeTries", _tries);
        Preferences.Set("Resume", true);
        Navigation.PopAsync();
    }

    private void GenerateTiles()
    {
        try
        {
            TilesGrid.Children.Clear();
            _totalPatternTiles = 0;
            _foundPatternTiles = 0;
            if (!Preferences.Get("Resume", false))
            {
                var randomPattern = Generator.GenerateRandomPattern(_pattern, _patternCoordinates, _borderReduction);
                _pattern = randomPattern._pattern;
                _patternCoordinates = randomPattern._patternCoordinates;
            }
            else
            {
                _pattern = DeserializePattern(Preferences.Get("ResumePattern", ""));
                _patternCoordinates = DeserializeCordinates(Preferences.Get("ResumeCordinates", ""));
                _clickedTiles = DeserializeCordinates(Preferences.Get("ResumeCordinatesClicked", ""));
                _multiplier = Preferences.Get("ResumeMultiplier", 1);
                _currentScore = Preferences.Get("ResumeScore", 0);
                _tries = Preferences.Get("ResumeTries", Preferences.Get("Tries", 1));
                _foundPatternTiles = _clickedTiles.Count;
                FinishProgress.Progress = ((double)_foundPatternTiles / _totalPatternTiles);
            }
            var startPoints = Generator.GetStartPoints(_pattern);
            _startPoints = startPoints;
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    var button = new Button
                    {
                        BackgroundColor = Color.FromArgb("#FFFFFF"),
                        BorderColor = Color.FromArgb("#a997d7"),
                        BorderWidth = 2,
                        CornerRadius = 16,
                        Shadow = new Shadow
                        {
                            Brush = new SolidColorBrush(Color.FromArgb("#674daaff")),
                            Offset = new Point(0, 2),
                            Radius = 6,
                            Opacity = 0.4f
                        },
                        Text = "",
                        WidthRequest = 44,
                        HeightRequest = 44,
                        Padding = 0,
                    };
                    button.SetValue(Grid.RowProperty, row);
                    button.SetValue(Grid.ColumnProperty, col);

                    _buttons[row, col] = button;
                    ToolTipProperties.SetText(button, "?");
                    if (_pattern[row, col])
                    {
                        _totalPatternTiles++;
                        bool isStart = (row == startPoints.startrow1 && col == startPoints.startcol1) ||
                                    (row == startPoints.startrow2 && col == startPoints.startcol2);
                        if (isStart)
                        {
                            button.BorderColor = Color.FromArgb("#4CAF50");
                            ToolTipProperties.SetText(button, $"{LanguageManager.GetText("TooltipStartField")}");
                        }
                        button.Clicked += async (_, _) => await OnPatternTileClicked(button, true);
                    }
                    else
                        button.Clicked += async (_, _) => await OnPatternTileClicked(button, false);
                    TilesGrid.Children.Add(button);
                }
            }
            // Hier die neue Methode aufrufen:
            RestoreClickedTiles();
        }
        catch (Exception){/*Ignored*/}
    }

    private void RestoreClickedTiles()
    {
        foreach (var clickedTile in _clickedTiles)
        {
            var button = _buttons[clickedTile.row, clickedTile.col];
            if (button != null)
            {
                button.IsEnabled = false;
                button.Text = $"{Generator.GetNeighborCount(clickedTile.row, clickedTile.col, _pattern, _buttons)}";
                if (_pattern[clickedTile.row, clickedTile.col])
                {
                    button.BackgroundColor = Color.FromArgb("#D0E0FF");
                    button.BorderColor = Color.FromArgb("#A0B8FF");
                    button.Shadow.Opacity = 0.2f;
                    button.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#B0C4FF"));
                }
                else
                {
                    button.BackgroundColor = Color.FromArgb("#FFCDD2");
                    button.BorderColor = Color.FromArgb("#E57373");
                    button.Shadow.Opacity = 0.2f;
                    button.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#FF8A80"));
                }
            }
        }
        FinishProgress.Progress = ((double)_foundPatternTiles / _totalPatternTiles);
    }

    private List<(int row, int col)> DeserializeCordinates(string v)
    {
        List<(int row, int col)> lPCordinates = new();
        var values = v.Split('-');
        foreach (var value in values)
        {
            if (string.IsNullOrWhiteSpace(value)) continue; // <-- Hinzufügen!
            var Cordinate = value.Split('*');
            if (Cordinate.Length == 2)
                lPCordinates.Add((Convert.ToInt32(Cordinate[0]), Convert.ToInt32(Cordinate[1])));
        }
        return lPCordinates;
    }

    private string? SerializeCordinates(List<(int row, int col)> patternCoordinates)
    {
        string sCstring = "";
        foreach (var (row, col) in patternCoordinates)
            sCstring += $"{row}*{col}-";
        return sCstring;
    }

    private bool[,] DeserializePattern(string get)
    {
        bool[,] dPattern = new bool[GridSize, GridSize];
        var values = get.Split('-');
        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                int index = row * GridSize + col;
                if (index < values.Length) { dPattern[row, col] = values[index] == "1"; }
            }
        }
        return dPattern;
    }

    private string? SerializePattern(bool[,] pattern)
    {
        string sPstring = "";
        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                if (pattern[row, col]) { sPstring += "1-"; }
                else { sPstring += "0-"; }
            }
        }
        return sPstring.TrimEnd('-');
    }

    private void UpdateScoreLabel()
    {
        CurrentScoreLabel.Text = _currentScore.ToString();
        ToolTipProperties.SetText(ScoreFrame, $"{LanguageManager.GetText("Score")}: {_currentScore}");
    }

    private void UpdateTryLabel()
    {
        CurrentFailsLabel.Text = _tries.ToString();
        ToolTipProperties.SetText(FailureFrame, $"{LanguageManager.GetText("RemainingAttempts")}: {_tries}");
    }

    private void UpdateMultiplierLabel()
    {
        MultiplierLabel.Text = $"x{_multiplier:0.0}";
        ToolTipProperties.SetText(MultiplierFrame, $"{LanguageManager.GetText("Multiplyer")}: x{_multiplier}");
    }

    private void NextPattern()
    {
        _completedPatterns++;
        int block = _completedPatterns / 10;
        double baseMulti = 1.0;
        double add = 0.0;
        int rest = _completedPatterns;
        for (int i = 0; i <= block; i++)
        {
            int inBlock = Math.Min(10, rest);
            add += inBlock * (0.1 + 0.1 * i);
            rest -= inBlock;
        }

        _multiplier = baseMulti + add;
        UpdateMultiplierLabel();
    }

    private async Task OnPatternTileClicked(Button button, bool isPattern)
    {
        if (_isGameOver)return;
        if (_bombMode)
        {
            if (_bombProcessing) return;
            Place_Bomb(button);
            return;
        }
        button.IsEnabled = false;
        _buttons[TilesGrid.GetRow(button), TilesGrid.GetColumn(button)] = null!;
        _clickedTiles.Add((TilesGrid.GetRow(button), TilesGrid.GetColumn(button)));

        int buttonrow = TilesGrid.GetRow(button);
        int buttoncolumn = TilesGrid.GetColumn(button);

        // Kompaktere Nachbarzählung
        button.Text = $"{Generator.GetNeighborCount(buttonrow, buttoncolumn, _pattern, _buttons)}";

        if (isPattern)
        {
            button.BackgroundColor = Color.FromArgb("#D0E0FF");
            button.BorderColor = Color.FromArgb("#A0B8FF");
            button.Shadow.Opacity = 0.2f;
            button.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#B0C4FF"));
            ToolTipProperties.SetText(button, $"{LanguageManager.GetText("PatternTile")}");
            _currentScore += (int)Math.Round(1 * _multiplier);
            _foundPatternTiles++;
            FinishProgress.Progress = ((double)_foundPatternTiles / _totalPatternTiles);
            ToolTipProperties.SetText(FortschritsFrame, $"{LanguageManager.GetText("Progress")}: {_foundPatternTiles} / {_totalPatternTiles}");
            UpdateScoreLabel();

            if (_foundPatternTiles == _totalPatternTiles)
            {
                NextPattern();
                await Task.Delay(500);
                _tries++;
                await Navigation.PushAsync(new GameView(_currentScore, _completedPatterns, _multiplier, _tries));
                Navigation.RemovePage(this);
            }
        }
        else
        {
            if(!_bombProcessing)
                _tries--;
            UpdateTryLabel();
            button.BackgroundColor = Color.FromArgb("#FFCDD2");
            button.BorderColor = Color.FromArgb("#E57373");
            button.Shadow.Opacity = 0.2f;
            button.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#FF8A80"));
            ToolTipProperties.SetText(button, $"{LanguageManager.GetText("NotPatternTile")}");

            if (_tries == 0)
            {
                _isGameOver = true;
                int highscore = Preferences.Get("Highscore", 0);
                if (_currentScore > highscore)
                    Preferences.Set("Highscore", _currentScore);
                int newCoins = Preferences.Get("Coins", 0) + _currentScore;

                Preferences.Set("Coins", newCoins);
                Preferences.Set("LastScore", _currentScore);
                Preferences.Set("Resume", false);

                await Task.Delay(2000);
                await Navigation.PopToRootAsync();
            }
        }
    }

    private async void HelpButton_OnClicked(object? sender, EventArgs e)
    {
        try
        {
            if (Application.Current?.MainPage?.Navigation != null)
                await Application.Current.MainPage.Navigation.PushAsync(new Tutorial(), true);
        }
        catch (Exception){/* ignored*/}
    }

    private void LanguageButton_OnClicked(object? sender, EventArgs e)
    {
        if (Preferences.Get("language", "en") == "en")
            Preferences.Set("language", "de");
        else if (Preferences.Get("language", "en") == "de")
            Preferences.Set("language", "fr");
        else if (Preferences.Get("language", "en") == "fr")
            Preferences.Set("language", "en");

        LanguageManager.Update();
        SetLanguage(); // <-- Texte neu laden!
    }

    private void UpdateItems()
    {
        if (Preferences.Get("Bombs", 0) > 0)
            BombButton.Source = "bomb.png";
        else
            BombButton.Source = "nobomb.png";

        if (Preferences.Get("Searchers", 0) > 0)
            SearchButton.Source = "searcher.png";
        else
            SearchButton.Source = "nosearch.png";
    }

    private void Search_OnClicked(object? sender, EventArgs e)
    {
        if (Preferences.Get("Searchers", 0) > 0)
        {
            Preferences.Set("Searchers", Preferences.Get("Searchers", 0) - 1);
            Click_Random_Pattern_Tile();
        }
        UpdateItems();
    }

    private void Click_Random_Pattern_Tile() //Rework this because of youse of KI (note for myself)
    {
        while (true)
        {
            var availablePatternTiles = new List<(int row, int col)>();
            for (int row = 0; row < GridSize; row++)
                for (int col = 0; col < GridSize; col++)
                    if (_pattern[row, col] && _buttons[row, col] != null && _buttons[row, col].IsEnabled)
                        availablePatternTiles.Add((row, col));

            if (availablePatternTiles.Count > 0)
            {
                var randomIndex = Random.Next(availablePatternTiles.Count);
                var randomPatternTile = availablePatternTiles[randomIndex];
                _buttons[randomPatternTile.row, randomPatternTile.col].BorderColor = Color.FromArgb("#FA026E");
                break;
            }
        }

    }

    private void Bomb_OnClicked(object? sender, EventArgs e)
    {
        if (_bombMode) return;
        if (Preferences.Get("Bombs", 0) > 0)
        {
            Preferences.Set("Bombs", Preferences.Get("Bombs", 0) - 1);
            Activate_Bomb_Placing();
        }
        UpdateItems();
    }

    private void Activate_Bomb_Placing()
    {
        _bombMode = true;
        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                var button = _buttons[row, col];
                if (button == null || !button.IsEnabled) continue;
                button.BorderColor = Color.FromArgb("#474954");
            }
        }
    }


    private void Place_Bomb(Button button)
    {
        _bombMode = false;
        _bombProcessing = true;
        int row = (int)button.GetValue(Grid.RowProperty);
        int col = (int)button.GetValue(Grid.ColumnProperty);

        // Zentrum jetzt enthalten + vier Nachbarn
        int[,] directions = { { 0, 0 }, { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int r = row + directions[i, 0];
            int c = col + directions[i, 1];
            if (r >= 0 && r < GridSize && c >= 0 && c < GridSize)
            {
                var targetButton = _buttons[r, c];
                if (targetButton != null && targetButton.IsEnabled)
                {
                    try
                    {
                        OnPatternTileClicked(targetButton, _pattern[r, c]); // bewusst ohne await belassen (minimalinvasiv)
                    }
                    catch { /* ignored */ }
                }
            }
        }

        Deactivate_Bomb_Placing();
    }

    private void Deactivate_Bomb_Placing()
    {
        _bombProcessing = false;
        _bombMode = false;
        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                var button = _buttons[row, col];
                if (button == null) continue; // Fix: Absturz verhindern
                if (button.BackgroundColor == Color.FromArgb("#FFCDD2"))
                    button.BorderColor = Color.FromArgb("#E57373");
                else if (button.BackgroundColor == Color.FromArgb("#D0E0FF"))
                    button.BorderColor = Color.FromArgb("#A0B8FF");
                else if (button.BackgroundColor == Color.FromArgb("#FFF8DC"))
                    button.BorderColor = Color.FromArgb("#FAFAAA");
                else
                    button.BorderColor = Color.FromArgb("#a997d7");
            }
        }
    }
}