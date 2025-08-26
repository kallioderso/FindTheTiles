using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;

namespace FindTheTiles;

public partial class GameView
{
    private const int GridSize = 7;
    private const int MinPatternTiles = 10;
    private const int MaxPatternTiles = 20;
    private const int MaxTries = 30;

    private static readonly (int dRow, int dCol)[] Directions = { (-1, 0), (1, 0), (0, -1), (0, 1) };

    // Private Felder
    private bool[,] _pattern = new bool[GridSize, GridSize];
    private Button[,] _buttons = new Button[GridSize, GridSize];
    private Button _lastButton = null;
    private List<(int row, int col)> _patternCoordinates = new();
    private readonly double[,] _borderReduction =
    {
        { 0.07, 0.07, 0.07, 0.07, 0.07, 0.07, 0.07 },
        { 0.07, 0.03, 0.03, 0.03, 0.03, 0.03, 0.07 },
        { 0.07, 0.03, 0, 0, 0, 0.03, 0.07},
        { 0.07, 0.03, 0, 0, 0, 0.03, 0.07},
        { 0.07, 0.03, 0, 0, 0, 0.03, 0.07},
        { 0.07, 0.03, 0.03, 0.03, 0.03, 0.03, 0.07 },
        { 0.07, 0.07, 0.07, 0.07, 0.07, 0.07, 0.07 },
    };
    private int _currentScore = 0;
    private bool _isGameOver = false;
    private int _foundPatternTiles = 0;
    private int _totalPatternTiles = 0;
    private static readonly Random _random = new();
    private int _tries = 0;
    
    // Multiplikator-Logik
    private int _completedPatterns = 0;
    private double _multiplier = 1.0;

    public GameView(int score = 0, int completedPatterns = 0, double multiplier = 1.0)
    {
        InitializeComponent();
        NavigationPage.SetHasBackButton(this, false);
        _currentScore = score;
        this._completedPatterns = completedPatterns;
        this._multiplier = multiplier;
        GenerateTiles();
        UpdateScoreLabel();
        UpdateMultiplierLabel();
        _tries = _random.Next(2, 5);
        UpdateTryLabel();
    }
    
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        string orientation = height > width ? "Portrait" : "Landscape";
        
        VisualStateManager.GoToState(MainStack, orientation);
    }

    private void ExitButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void GenerateTiles()
    {
        TilesGrid.Children.Clear();
        _totalPatternTiles = 0;
        _foundPatternTiles = 0;
        var randomPattern = Generator.GenerateRandomPattern(_pattern, _patternCoordinates, _borderReduction);
        _pattern = randomPattern._pattern;
        _patternCoordinates = randomPattern._patternCoordinates;
        var startPoints = Generator.GetStartPoints(_pattern);

        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                var button = new Button
                {
                    BackgroundColor = Color.FromArgb("#F3F7FF"),
                    BorderColor = Color.FromArgb("#E0E8FF"),
                    BorderWidth = 2,
                    CornerRadius = 16,
                    Shadow = new Shadow
                    {
                        Brush = new SolidColorBrush(Color.FromArgb("#B0C4FF")),
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
                if (_pattern[row, col])
                {
                    _totalPatternTiles++;
                    bool isStart = (row == startPoints.startrow1 && col == startPoints.startcol1) || (row == startPoints.startrow2 && col == startPoints.startcol2);
                    if (isStart)
                        button.BorderColor = Color.FromArgb("#4CAF50");
                    button.Clicked += async (sender, e) => await OnPatternTileClicked(button, true);
                }
                else
                {
                    button.Clicked += async (sender, e) => await OnPatternTileClicked(button, false);
                }
                TilesGrid.Children.Add(button);
            }
        }
    }

    private void UpdateScoreLabel()
    {
        CurrentScoreLabel.Text = _currentScore.ToString();
    }

    private void UpdateTryLabel()
    {
        CurrentFailsLabel.Text = _tries.ToString();
    }

    private void UpdateMultiplierLabel()
    {
        MultiplierLabel.Text = $"x{_multiplier:0.0}";
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
        if (_isGameOver)
            return;
        button.IsEnabled = false;
        _buttons[TilesGrid.GetRow(button), TilesGrid.GetColumn(button)] = null!;
        _lastButton = button;

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
            _currentScore += (int)Math.Round(1 * _multiplier);
            _foundPatternTiles++;
            FinishProgress.Progress = ((double)_foundPatternTiles / _totalPatternTiles);
            UpdateScoreLabel();

            if (_foundPatternTiles == _totalPatternTiles)
            {
                NextPattern();
                await Task.Delay(500);
                await Navigation.PushAsync(new GameView(_currentScore, _completedPatterns, _multiplier));
                Navigation.RemovePage(this);
            }
        }
        else
        {
            _tries--;
            UpdateTryLabel();
            button.BackgroundColor = Color.FromArgb("#FFCDD2");
            button.BorderColor = Color.FromArgb("#E57373");
            button.Shadow.Opacity = 0.2f;
            button.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#FF8A80"));

            if (_tries == 0)
            {
                _isGameOver = true;
                int highscore = Preferences.Get("Highscore", 0);
                if (_currentScore > highscore)
                    Preferences.Set("Highscore", _currentScore);
                int newXp = Preferences.Get("XP", 0) + _currentScore;
                Preferences.Set("XP", newXp);
                Preferences.Set("LastScore", _currentScore);

                await Task.Delay(2000);
                await Navigation.PopToRootAsync();
            }
        }
    }

    private async void OnHelpButtonClicked(object? sender, EventArgs e)
    {
        Application.Current.MainPage.Navigation.PushAsync(new Tutorial(), true);
    }
}