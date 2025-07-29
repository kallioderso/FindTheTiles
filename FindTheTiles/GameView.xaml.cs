using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace FindTheTiles;

public partial class GameView : ContentPage
{
    private bool[,] pattern;
    private char currentLetter;
    private int currentScore = 0;
    private bool gameOver = false;
    private int foundPatternTiles = 0;
    private int totalPatternTiles = 0;
    private int wrongTries = 0;

    // Multiplikator-Logik
    private int completedPatterns = 0;
    private double multiplier = 1.0;

    protected override void OnAppearing()
    {
        base.OnAppearing();
        NavigationPage.SetHasBackButton(this, false);
    }

    public GameView(int score = 0, int completedPatterns = 0, double multiplier = 1.0)
    {
        InitializeComponent();
        NavigationPage.SetHasBackButton(this, false);
        currentScore = score;
        this.completedPatterns = completedPatterns;
        this.multiplier = multiplier;
        GenerateTiles();
        UpdateScoreLabel();
        UpdateMultiplierLabel();
    }

    private void ExitButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void GenerateTiles()
    {
        TilesGrid.Children.Clear();
        currentLetter = GetRandomLetter();
        GeneratePatternForLetter(currentLetter);

        foundPatternTiles = 0;
        totalPatternTiles = 0;

        var patternCoords = new List<(int row, int col)>();
        for (int row = 0; row < 7; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                if (pattern[row, col])
                    patternCoords.Add((row, col));
            }
        }

        (int startRow, int startCol) = patternCoords[new Random().Next(patternCoords.Count)];

        for (int row = 0; row < 7; row++)
        {
            for (int col = 0; col < 7; col++)
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

                if (pattern[row, col])
                {
                    totalPatternTiles++;
                    button.Clicked += async (sender, e) => await MusterClicked(button, true);

                    if (row == startRow && col == startCol)
                        button.BorderColor = Color.FromArgb("#4CAF50");
                }
                else
                {
                    button.Clicked += async (sender, e) => await MusterClicked(button, false);
                }

                TilesGrid.Children.Add(button);
            }
        }
    }

    private char GetRandomLetter()
    {
        char[] letters = {
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x'
        };
        var random = new Random();
        return letters[random.Next(letters.Length)];
    }

     private void GeneratePatternForLetter(char letter)
    {
        pattern = new bool[7, 7];
        switch (letter)
        {
            case 'A':
                for (int i = 0; i < 7; i++) pattern[i, i] = true;
                break;
            case 'B':
                for (int i = 0; i < 7; i++) pattern[i, 6 - i] = true;
                break;
            case 'C':
                for (int i = 0; i < 7; i++) pattern[0, i] = true;
                for (int i = 0; i < 7; i++) pattern[6, i] = true;
                break;
            case 'D':
                for (int i = 0; i < 7; i++) pattern[i, 0] = true;
                for (int i = 0; i < 7; i++) pattern[i, 6] = true;
                break;
            case 'E':
                for (int i = 0; i < 7; i++) pattern[i, 3] = true;
                for (int i = 0; i < 7; i++) pattern[3, i] = true;
                break;
            case 'F':
                for (int i = 0; i < 7; i++) pattern[0, i] = true;
                for (int i = 0; i < 7; i++) pattern[i, 0] = true;
                break;
            case 'G':
                for (int i = 0; i < 7; i++) pattern[6, i] = true;
                for (int i = 0; i < 7; i++) pattern[i, 6] = true;
                break;
            case 'H':
                for (int i = 0; i < 7; i++) pattern[i, 0] = true;
                for (int i = 0; i < 7; i++) pattern[i, 6] = true;
                for (int i = 0; i < 7; i++) pattern[3, i] = true;
                break;
            case 'I':
                for (int i = 0; i < 7; i++) pattern[0, i] = true;
                for (int i = 0; i < 7; i++) pattern[6, i] = true;
                for (int i = 0; i < 7; i++) pattern[i, 3] = true;
                break;
            case 'J':
                for (int i = 0; i < 7; i++) pattern[0, i] = true;
                for (int i = 0; i < 7; i++) pattern[i, 6] = true;
                for (int i = 3; i < 7; i++) pattern[6, i] = true;
                break;
            case 'K':
                for (int i = 0; i < 7; i++) pattern[i, 0] = true;
                for (int i = 0; i < 7; i++) pattern[i, i] = true;
                break;
            case 'L':
                for (int i = 0; i < 7; i++) pattern[i, 0] = true;
                for (int i = 0; i < 7; i++) pattern[6, i] = true;
                break;
            case 'M':
                for (int i = 0; i < 7; i++) pattern[i, 0] = true;
                for (int i = 0; i < 7; i++) pattern[i, 6] = true;
                pattern[1, 1] = pattern[2, 2] = pattern[1, 5] = pattern[2, 4] = true;
                break;
            case 'N':
                for (int i = 0; i < 7; i++) pattern[i, 0] = true;
                for (int i = 0; i < 7; i++) pattern[i, 6] = true;
                for (int i = 0; i < 7; i++) pattern[i, i] = true;
                break;
            case 'O':
                for (int i = 0; i < 7; i++) pattern[0, i] = true;
                for (int i = 0; i < 7; i++) pattern[6, i] = true;
                for (int i = 0; i < 7; i++) pattern[i, 0] = true;
                for (int i = 0; i < 7; i++) pattern[i, 6] = true;
                break;
            case 'P':
                for (int i = 0; i < 7; i++) pattern[i, 0] = true;
                for (int i = 0; i < 7; i++) pattern[0, i] = true;
                for (int i = 0; i < 4; i++) pattern[i, 6] = true;
                for (int i = 0; i < 7; i++) pattern[3, i] = true;
                break;
            case 'Q':
                for (int i = 0; i < 7; i++) pattern[0, i] = true;
                for (int i = 0; i < 7; i++) pattern[6, i] = true;
                for (int i = 0; i < 7; i++) pattern[i, 0] = true;
                for (int i = 0; i < 7; i++) pattern[i, 6] = true;
                pattern[5, 5] = pattern[6, 6] = true;
                break;
            case 'R':
                for (int i = 0; i < 7; i++) pattern[i, 0] = true;
                for (int i = 0; i < 7; i++) pattern[0, i] = true;
                for (int i = 0; i < 4; i++) pattern[i, 6] = true;
                for (int i = 0; i < 7; i++) pattern[3, i] = true;
                pattern[4, 4] = pattern[5, 5] = pattern[6, 6] = true;
                break;
            case 'S':
                for (int i = 0; i < 7; i++) pattern[0, i] = true;
                for (int i = 0; i < 7; i++) pattern[3, i] = true;
                for (int i = 0; i < 7; i++) pattern[6, i] = true;
                pattern[1, 0] = pattern[2, 0] = pattern[4, 6] = pattern[5, 6] = true;
                break;
            case 'T':
                for (int i = 0; i < 7; i++) pattern[0, i] = true;
                for (int i = 0; i < 7; i++) pattern[i, 3] = true;
                break;
            case 'U':
                for (int i = 0; i < 7; i++) pattern[i, 0] = true;
                for (int i = 0; i < 7; i++) pattern[i, 6] = true;
                for (int i = 0; i < 7; i++) pattern[6, i] = true;
                break;
            case 'V':
                for (int i = 0; i < 6; i++) pattern[i, 0] = pattern[i, 6] = true;
                pattern[6, 3] = true;
                pattern[5, 2] = pattern[5, 4] = true;
                break;
            case 'W':
                for (int i = 0; i < 7; i++) pattern[i, 0] = pattern[i, 6] = true;
                pattern[5, 1] = pattern[5, 5] = true;
                pattern[6, 2] = pattern[6, 4] = true;
                pattern[4, 3] = true;
                break;
            case 'X':
                for (int i = 0; i < 7; i++)
                {
                    pattern[i, i] = true;
                    pattern[i, 6 - i] = true;
                }
                break;
            // 24 weitere zufällige Muster (a-x)
            case 'a':
                for (int i = 0; i < 7; i++) pattern[1, i] = true;
                for (int i = 0; i < 7; i++) pattern[i, 5] = true;
                break;
            case 'b':
                for (int i = 0; i < 7; i++) pattern[2, i] = true;
                for (int i = 0; i < 7; i++) pattern[i, 4] = true;
                break;
            case 'c':
                for (int i = 0; i < 7; i++) pattern[3, i] = true;
                for (int i = 0; i < 7; i++) pattern[i, 2] = true;
                break;
            case 'd':
                for (int i = 0; i < 7; i++) pattern[4, i] = true;
                for (int i = 0; i < 7; i++) pattern[i, 1] = true;
                break;
            case 'e':
                for (int i = 0; i < 7; i++) pattern[5, i] = true;
                for (int i = 0; i < 7; i++) pattern[i, 0] = true;
                break;
            case 'f':
                for (int i = 0; i < 7; i++) pattern[i, (i + 1) % 7] = true;
                break;
            case 'g':
                for (int i = 0; i < 7; i++) pattern[i, (6 - i + 1) % 7] = true;
                break;
            case 'h':
                for (int i = 0; i < 7; i++) pattern[6 - i, i] = true;
                break;
            case 'i':
                for (int i = 0; i < 7; i++) pattern[i, (i + 2) % 7] = true;
                break;
            case 'j':
                for (int i = 0; i < 7; i++) pattern[(i + 2) % 7, i] = true;
                break;
            case 'k':
                for (int i = 0; i < 7; i++) pattern[(i + 3) % 7, i] = true;
                break;
            case 'l':
                for (int i = 0; i < 7; i++) pattern[i, (i + 3) % 7] = true;
                break;
            case 'm':
                for (int i = 0; i < 7; i++) pattern[i, (i + 4) % 7] = true;
                break;
            case 'n':
                for (int i = 0; i < 7; i++) pattern[(i + 4) % 7, i] = true;
                break;
            case 'o':
                for (int i = 0; i < 7; i++) pattern[i, (i + 5) % 7] = true;
                break;
            case 'p':
                for (int i = 0; i < 7; i++) pattern[(i + 5) % 7, i] = true;
                break;
            case 'q':
                for (int i = 0; i < 7; i++) pattern[i, (i + 6) % 7] = true;
                break;
            case 'r':
                for (int i = 0; i < 7; i++) pattern[(i + 6) % 7, i] = true;
                break;
            case 's':
                for (int i = 0; i < 7; i++) pattern[i, (6 - i + 2) % 7] = true;
                break;
            case 't':
                for (int i = 0; i < 7; i++) pattern[(6 - i + 2) % 7, i] = true;
                break;
            case 'u':
                for (int i = 0; i < 7; i++) pattern[i, (6 - i + 3) % 7] = true;
                break;
            case 'v':
                for (int i = 0; i < 7; i++) pattern[(6 - i + 3) % 7, i] = true;
                break;
            case 'w':
                for (int i = 0; i < 7; i++) pattern[i, (6 - i + 4) % 7] = true;
                break;
            case 'x':
                for (int i = 0; i < 7; i++) pattern[(6 - i + 4) % 7, i] = true;
                break;
            default:
                // Fallback: Diagonalen
                for (int i = 0; i < 7; i++)
                {
                    pattern[i, i] = true;
                    pattern[i, 6 - i] = true;
                }
                break;
        }
    }

    private void UpdateScoreLabel()
    {
        CurrentScoreLabel.Text = currentScore.ToString();
    }

    private void UpdateMultiplierLabel()
    {
        MultiplierLabel.Text = $"x{multiplier:0.0}";
    }

    private void NextPattern()
    {
        completedPatterns++;
        int block = completedPatterns / 10;
        double baseMulti = 1.0;
        double add = 0.0;
        int rest = completedPatterns;
        for (int i = 0; i <= block; i++)
        {
            int inBlock = Math.Min(10, rest);
            add += inBlock * (0.1 + 0.1 * i);
            rest -= inBlock;
        }
        multiplier = baseMulti + add;
        UpdateMultiplierLabel();
    }

    private async Task MusterClicked(Button button, bool isPattern)
    {
        if (gameOver)
            return;

        button.IsEnabled = false;
        
        var buttonrow = TilesGrid.GetRow(button);
        var buttoncolumn = TilesGrid.GetColumn(button);
        var buttonNeigbors = 0;

        (int dRow, int dCol)[] directions = { (-1, 0), (1, 0), (0, -1), (0, 1) };
        foreach (var (dRow, dCol) in directions)
        {
            int nRow = buttonrow + dRow;
            int nCol = buttoncolumn + dCol;
            if (nRow >= 0 && nRow < 7 && nCol >= 0 && nCol < 7)
            {
                if (pattern[nRow, nCol])
                {
                    buttonNeigbors++;
                }
            }
        }

        button.Text = $"{buttonNeigbors}";

        if (isPattern)
        {
            if (button.BackgroundColor != Color.FromArgb("#D0E0FF"))
            {
                button.BackgroundColor = Color.FromArgb("#D0E0FF");
                button.BorderColor = Color.FromArgb("#A0B8FF");
                button.Shadow.Opacity = 0.2f;
                button.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#B0C4FF"));
                currentScore += (int)Math.Round(1 * multiplier);
                foundPatternTiles++;
                UpdateScoreLabel();

                if (foundPatternTiles == totalPatternTiles)
                {
                    NextPattern();
                    await Task.Delay(500);
                    await Navigation.PushAsync(new GameView(currentScore, completedPatterns, multiplier));
                    Navigation.RemovePage(this);
                }
            }
        }
        else
        {
            if (button.BackgroundColor != Color.FromArgb("#FFCDD2"))
            {
                wrongTries++;
                button.BackgroundColor = Color.FromArgb("#FFCDD2");
                button.BorderColor = Color.FromArgb("#E57373");
                button.Shadow.Opacity = 0.2f;
                button.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#FF8A80"));

                if (wrongTries >= 3)
                {
                    gameOver = true;
                    int highscore = Preferences.Get("Highscore", 0);
                    if (currentScore > highscore)
                        Preferences.Set("Highscore", currentScore);

                    Preferences.Set("LastScore", currentScore);

                    await Task.Delay(2000);
                    await Navigation.PopToRootAsync();
                }
            }
        }
    }
}