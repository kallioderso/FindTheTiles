using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindTheTiles;

public partial class Tutorial : ContentPage
{
    private Button _startbutton;
    private Button[,] _buttons = new Button[7, 7];
    private bool[,] _pattern =
    {
        { false, false, false, false, false, true, true },
        { false, false, false, true, true, true, false },
        { false, false, false, true, false, true, true },
        { false, false, true, true, false, false, false },
        { false, true, true, false, false, false, false },
        { false, false, true, true, true, false, false },
        { false, false, false, true, false, false, false },
    };

    private int _musterTiles = 0;
    private readonly int[,] _disappearing =
    {
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 1, 1, 1, 1, 1, 0 },
        { 0, 1, 2, 2, 2, 1, 0 },
        { 0, 1, 2, 3, 2, 1, 0 },
        { 0, 1, 2, 2, 2, 1, 0 },
        { 0, 1, 1, 1, 1, 1, 0 },
        { 0, 0, 0, 0, 0, 0, 0 }
    };
    public Tutorial()
    {
        InitializeComponent();
        GenerateTiles();
        Preferences.Set("FirstStart", false);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GenerateStartPoint();
    }

    private void ExitButton_Clicked(object? sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void GenerateTiles()
    {
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
                _buttons[row, col] = button;
                TilesGrid.Children.Add(button);
            }
        }
        _startbutton = _buttons[3, 3];
    }

    private async Task GenerateStartPoint()
    {
        await Task.Delay(2000);
        for (int row = 0; row < 7; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                if (_disappearing[row, col] == 0) _buttons[row, col].IsVisible = false;
            }
        }
        await Task.Delay(500);
        for (int row = 0; row < 7; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                if (_disappearing[row, col] == 1) _buttons[row, col].IsVisible = false;
            }
        }
        await Task.Delay(500);
        for (int row = 0; row < 7; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                if (_disappearing[row, col] == 2) _buttons[row, col].IsVisible = false;
            }
        }
        await Task.Delay(500);
        _startbutton.BorderColor = Color.FromArgb("#4CAF50");
        _startbutton.Clicked += StartPoint_Clicked;
        await AddText("Klicke auf das grüne Startfeld");
        Unfroze();
    }

    private async Task RemoveText()
    {
        var letters = TutorialLabel.Text.Length;
        for (int i = 0; i < letters; i++)
        {
            await Task.Delay(10);
            TutorialLabel.Text = TutorialLabel.Text.Remove(TutorialLabel.Text.Length-1, 1);
        }
        TutorialLabel.Text = "";
    }
    
    private async Task AddText(string text)
    {
        Froze();
        await RemoveText();
        await Task.Delay(200);
        TutorialLabel.Text = "";
        foreach (var letter in text)
        {
            await Task.Delay(20);
            TutorialLabel.Text += letter;
        }
    }

    private async void StartPoint_Clicked(object? sender, EventArgs e)
    {
        _startbutton.BackgroundColor = Color.FromArgb("#D0E0FF");
        _startbutton.BorderColor = Color.FromArgb("#A0B8FF");
        _startbutton.Shadow.Opacity = 0.2f;
        _startbutton.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#B0C4FF"));
        _startbutton.Clicked += null;
        await AddText("Die blaue Farbe zeigt an, dass dieses Feld zum Muster gehört.");
        await Task.Delay(5000);
        await AddText("Das Ziel ist es, das Muster zu vervollständigen.");
        for (int row = 0; row < 7; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                if (_disappearing[row, col] == 2) _buttons[row, col].IsVisible = true;
            }
        }
        await Task.Delay(5000);
        await AddText("Als hinweis zeigt das Feld nun an, wie viele der Gelben Felder zu dem Muster gehören.");
        await Task.Delay(1000);
        _startbutton.Text = Generator.GetNeighborCount(3, 3, _pattern, _buttons).ToString();
        await Task.Delay(5000);
        await AddText("Versuche nun, die beiden gelben Feldern zu finden, die zum Muster gehören.");
        RemoveButtonFromList(_startbutton);
        _buttons[3, 2].Clicked += MusterClicked;
        _buttons[2, 3].Clicked += MusterClicked;
        _buttons[3, 4].Clicked += WrongClicked;
        _buttons[4, 3].Clicked += WrongClicked;
        Unfroze();
    }

    private async void WrongClicked(object? sender, EventArgs e)
    {
        var button = (Button)sender!;
        button.BackgroundColor = Color.FromArgb("#FFCDD2");
        button.BorderColor = Color.FromArgb("#E57373");
        button.Shadow.Opacity = 0.2f;
        button.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#FF8A80"));
        button.IsEnabled = false;
        button.Clicked -= WrongClicked; 
        await AddText("Das ist leider kein teil des Musters, versuche es noch einmal.");
        Unfroze();
    }

    private async void MusterClicked(object? sender, EventArgs e)
    {
        var button = (Button)sender!;
        button.BackgroundColor = Color.FromArgb("#D0E0FF");
        button.BorderColor = Color.FromArgb("#A0B8FF");
        button.Shadow.Opacity = 0.2f;
        button.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#B0C4FF"));
        button.IsEnabled = false;
        button.Clicked -= MusterClicked;
        _musterTiles++;
        if (_musterTiles == 2)
        {
            _musterTiles++;
            await AddText("Gut gemacht! Du hast beide Felder Gefunden.");
            await Task.Delay(5000);
            await AddText("Versuche nun, das Muster zu vervollständigen.");
            for (int row = 0; row < 7; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    if (_disappearing[row, col] == 1) _buttons[row, col].IsVisible = true;
                }
            }
            await Task.Delay(500);
            for (int row = 0; row < 7; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    if (_disappearing[row, col] == 0) _buttons[row, col].IsVisible = true;
                }
            }
            await Task.Delay(500);
            _buttons[3, 2].Text = Generator.GetNeighborCount(3, 2, _pattern, _buttons).ToString();
            _buttons[2, 3].Text = Generator.GetNeighborCount(2, 3, _pattern, _buttons).ToString();
            if (_buttons[3, 4]?.BackgroundColor.ToArgbHex() == Color.FromArgb("#FFCDD2").ToArgbHex())
            { 
                _buttons[3, 4].Text = Generator.GetNeighborCount(3, 4, _pattern, _buttons).ToString();
                RemoveButtonFromList(_buttons[3, 4]);
            }

            if (_buttons[4, 3]?.BackgroundColor.ToArgbHex() == Color.FromArgb("#FFCDD2").ToArgbHex())
            {
                _buttons[4, 3].Text = Generator.GetNeighborCount(4, 3, _pattern, _buttons).ToString();
                RemoveButtonFromList(_buttons[4, 3]);
            }
            RemoveButtonFromList(_buttons[3, 2]);
            RemoveButtonFromList(_buttons[2, 3]);
            Task.Delay(2000);
            await AddText("Als kleine Hilfe bekommst du nun einen Fortschrittsbalken, der dir anzeigt, wie viel des Musters du bereits gefunden hast.");
            FortschritsFrame.IsEnabled = true;
            FortschritsFrame.IsVisible = true;
            FinishProgress.Progress = ((double)_musterTiles / 16.0);
            ActivateNormalGame();
        }
        else
        {
            await AddText("Du hast ein Teil des Musters gefunden, Gut gemacht.");
            await Task.Delay(3000);
            await AddText("Versuche nun noch das nächste zu finden.");
        }
        Unfroze();
    }

    private void RemoveButtonFromList(Button button)
    {
        _buttons[TilesGrid.GetRow(button), TilesGrid.GetColumn(button)].IsEnabled = false!;
    }
    
    private void Froze()
    {
        for (int row = 0; row < 7; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                if (_buttons[row, col]?.IsVisible == true)
                    _buttons[row, col].IsEnabled = false;
            }
        }
    }

    private void Unfroze()
    {
        for (int row = 0; row < 7; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                if (_buttons[row, col]?.IsVisible == true && (_buttons[row, col].BackgroundColor.ToArgbHex() == Color.FromArgb("#F3F7FF").ToArgbHex() || _buttons[row, col].BackgroundColor.ToArgbHex() == Color.FromArgb("#FFF8DC").ToArgbHex()))
                    _buttons[row, col].IsEnabled = true;
            }
        }
    }

    private void ActivateNormalGame()
    {
        for (int row = 0; row < 7; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                var button = _buttons[row, col];
                if (button == null) continue;

                // Entferne alle vorherigen Handler
                button.Clicked -= MusterClicked;
                button.Clicked -= WrongClicked;
                button.Clicked -= StartPoint_Clicked;
                button.Clicked -= async (sender, e) => await TileClicked(button, true);
                button.Clicked -= async (sender, e) => await TileClicked(button, false);

                // Füge neuen Handler hinzu
                if (_pattern[row, col])
                    button.Clicked += async (sender, e) => await TileClicked(button, true);
                else
                    button.Clicked += async (sender, e) => await TileClicked(button, false);
            }
        }
    }

    private Task TileClicked(Button button, bool isPattern)
    {
        button.IsEnabled = false;
        
        int buttonrow = TilesGrid.GetRow(button);
        int buttoncolumn = TilesGrid.GetColumn(button);
        
        button.Text = $"{Generator.GetNeighborCount(buttonrow, buttoncolumn, _pattern, _buttons)}";
        if (isPattern)
        {
            button.BackgroundColor = Color.FromArgb("#D0E0FF");
            button.BorderColor = Color.FromArgb("#A0B8FF");
            button.Shadow.Opacity = 0.2f;
            button.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#B0C4FF"));
            _musterTiles++;
            FinishProgress.Progress = ((double)_musterTiles / 16.0);
            if(_musterTiles == 16)
                EndTutorial();
                
        }
        else
        {
            button.BackgroundColor = Color.FromArgb("#FFCDD2");
            button.BorderColor = Color.FromArgb("#E57373");
            button.Shadow.Opacity = 0.2f;
            button.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#FF8A80"));
        }

        return Task.CompletedTask;
    }

    private async Task EndTutorial()
    {
        await AddText("Glückwunsch! Du hast das Tutorial abgeschlossen. Du kannst nun das Spiel spielen.");
        await Task.Delay(2000);
        Application.Current.MainPage.Navigation.PopAsync();
    }
}