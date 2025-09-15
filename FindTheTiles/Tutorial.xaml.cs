namespace FindTheTiles;

public partial class Tutorial
{
    private Button _startbutton = null!;
    private readonly Button[,] _buttons = new Button[7, 7];
    private readonly bool[,] _pattern =
    {
        { false, false, false, false, false, true, true },
        { false, false, false, true, true, true, false },
        { false, false, false, true, false, true, true },
        { false, false, true, true, false, false, false },
        { false, true, true, false, false, false, false },
        { false, false, true, true, true, false, false },
        { false, false, false, true, false, false, false },
    };

    private string lastText = string.Empty;

    private int _musterTiles;
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

    private CancellationTokenSource _textAnimationCts = new CancellationTokenSource();

    public Tutorial()
    {
        InitializeComponent();
        GenerateTiles();
        Preferences.Set("FirstStart", false);
        UpdateLanguage();
    }

    protected override async void OnAppearing()
    {
        try
        {
            base.OnAppearing();
            await GenerateStartPoint();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private void UpdateLanguage()
    {
        if (Preferences.Get("language", "en") == "en")
            LanguageButton.Source = "english.png";
        else if (Preferences.Get("language", "en") == "de")
            LanguageButton.Source = "german.png";
        else if (Preferences.Get("language", "en") == "fr")
            LanguageButton.Source = "frensh.png";
        ToolTipProperties.SetText(LanguageButton, $"{LanguageManager.GetText("TooltipLanguage")}");
        ToolTipProperties.SetText(ExitButton, LanguageManager.GetText("TooltipExit"));
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
        await AddTextCooldown("Tutorial1", 0);
        Unfroze();
    }

    private async Task AddTextCooldown(string Text, int cooldown)
    {
        lastText = Text;
        _textAnimationCts = new CancellationTokenSource();
        try
        {
            await AddText(LanguageManager.GetText(Text), _textAnimationCts.Token);
            await Task.Delay(cooldown, _textAnimationCts.Token);
        }
        catch (OperationCanceledException)
        {
            TutorialLabel.Text = LanguageManager.GetText(Text);
        }
    }

    private async Task RemoveText(CancellationToken token)
    {
        var letters = TutorialLabel.Text.Length;
        for (int i = 0; i < letters; i++)
        {
            token.ThrowIfCancellationRequested();
            await Task.Delay(10, token);
            TutorialLabel.Text = TutorialLabel.Text.Remove(TutorialLabel.Text.Length-1, 1);
        }
        TutorialLabel.Text = "";
    }
    
    private async Task AddText(string text, CancellationToken token)
    {
        Froze();
        await RemoveText(token);
        await Task.Delay(200, token);
        TutorialLabel.Text = "";
        foreach (var letter in text)
        {
            token.ThrowIfCancellationRequested();
            await Task.Delay(20, token);
            TutorialLabel.Text += letter;
        }
    }

    private async void StartPoint_Clicked(object? sender, EventArgs e)
    {
        try
        {
            _startbutton.BackgroundColor = Color.FromArgb("#D0E0FF");
            _startbutton.BorderColor = Color.FromArgb("#A0B8FF");
            _startbutton.Shadow.Opacity = 0.2f;
            _startbutton.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#B0C4FF"));
            _startbutton.Clicked -= StartPoint_Clicked;
            await AddTextCooldown("Tutorial2", 5000);
            await AddTextCooldown("Tutorial3", 0);
            for (int row = 0; row < 7; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    if (_disappearing[row, col] == 2) _buttons[row, col].IsVisible = true;
                }
            }
            await Task.Delay(5000);
            await AddTextCooldown("Tutorial4", 1000);
            _startbutton.Text = Generator.GetNeighborCount(3, 3, _pattern, _buttons).ToString();
            await Task.Delay(5000);
            await AddTextCooldown("Tutorial5", 0);
            RemoveButtonFromList(_startbutton);
            _buttons[3, 2].Clicked += MusterClicked;
            _buttons[2, 3].Clicked += MusterClicked;
            _buttons[3, 4].Clicked += WrongClicked;
            _buttons[4, 3].Clicked += WrongClicked;
            Unfroze();
        }
        catch (Exception)
        {
            // nothing
        }
    }

    private async void WrongClicked(object? sender, EventArgs e)
    {
        try
        {
            var button = (Button)sender!;
            button.BackgroundColor = Color.FromArgb("#FFCDD2");
            button.BorderColor = Color.FromArgb("#E57373");
            button.Shadow.Opacity = 0.2f;
            button.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#FF8A80"));
            UnEnable(button);
            button.Clicked -= WrongClicked; 
            await AddTextCooldown("Tutorial6", 0);
            Unfroze();
        }
        catch (Exception)
        {
            //Nothing still
        }
    }

    private async void MusterClicked(object? sender, EventArgs e)
    {
        try
        {
            var button = (Button)sender!;
            button.BackgroundColor = Color.FromArgb("#D0E0FF");
            button.BorderColor = Color.FromArgb("#A0B8FF");
            button.Shadow.Opacity = 0.2f;
            button.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#B0C4FF"));
            UnEnable(button);
            button.Clicked -= MusterClicked;
            _musterTiles++;
            if (_musterTiles == 2)
            {
                _musterTiles++;
                await AddTextCooldown("Tutorial7", 5000);
                await AddTextCooldown("Tutorial8", 0);
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
                if (_buttons[3, 4].BackgroundColor.ToArgbHex() == Color.FromArgb("#FFCDD2").ToArgbHex())
                { 
                    _buttons[3, 4].Text = Generator.GetNeighborCount(3, 4, _pattern, _buttons).ToString();
                    RemoveButtonFromList(_buttons[3, 4]);
                }

                if (_buttons[4, 3].BackgroundColor.ToArgbHex() == Color.FromArgb("#FFCDD2").ToArgbHex())
                {
                    _buttons[4, 3].Text = Generator.GetNeighborCount(4, 3, _pattern, _buttons).ToString();
                    RemoveButtonFromList(_buttons[4, 3]);
                }
                RemoveButtonFromList(_buttons[3, 2]);
                RemoveButtonFromList(_buttons[2, 3]);
                await Task.Delay(2000);
                await AddTextCooldown("Tutorial9", 0);
                FortschritsFrame.IsEnabled = true;
                FortschritsFrame.IsVisible = true;
                FinishProgress.Progress = (_musterTiles / 16.0);
                ActivateNormalGame();
            }
            else
            {
                await AddTextCooldown("Tutorial10", 3000);
                await AddTextCooldown("Tutorial11", 0);
            }
            Unfroze();
        }
        catch (Exception)
        {
            //And again nothing
        }
    }

    private void RemoveButtonFromList(Button button)
    {
        _buttons[TilesGrid.GetRow(button), TilesGrid.GetColumn(button)].IsEnabled = false;
    }
    
    private void Froze()
    {
        for (int row = 0; row < 7; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                if (_buttons[row, col].IsVisible)
                    UnEnable(_buttons[row, col]);
            }
        }
    }

    private void UnEnable(Button button)
    {
        Color background = button.BackgroundColor;
        Color border = button.BorderColor;
        button.IsEnabled = false;
        button.BackgroundColor = background;
        button.BorderColor = border;
    }

    private void Unfroze()
{
    for (int row = 0; row < 7; row++)
    {
        for (int col = 0; col < 7; col++)
        {
            var button = _buttons[row, col];
            if (button.IsVisible)
            {
                // Startbutton immer aktivieren
                if (button == _startbutton && button.BorderColor.ToArgbHex() == Color.FromArgb("#4CAF50").ToArgbHex())
                {
                    button.IsEnabled = true;
                }
                // Andere Buttons wie gehabt
                else if (button.BackgroundColor.ToArgbHex() == Color.FromArgb("#FFFFFF").ToArgbHex() ||
                         button.BackgroundColor.ToArgbHex() == Color.FromArgb("#FFF8DC").ToArgbHex())
                {
                    button.IsEnabled = true;
                }
            }
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

                // Entferne alle vorherigen Handler
                button.Clicked -= MusterClicked;
                button.Clicked -= WrongClicked;
                button.Clicked -= StartPoint_Clicked;
                button.Clicked -= async (_, _) => await TileClicked(button, true);
                button.Clicked -= async (_, _) => await TileClicked(button, false);

                // Füge neuen Handler hinzu
                if (_pattern[row, col])
                    button.Clicked += async (_, _) => await TileClicked(button, true);
                else
                    button.Clicked += async (_, _) => await TileClicked(button, false);
            }
        }
    }

    private async Task TileClicked(Button button, bool isPattern)
    {
        UnEnable(button);
        
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
            FinishProgress.Progress = (_musterTiles / 16.0);
            if(_musterTiles == 16)
                await EndTutorial();

        }
        else
        {
            button.BackgroundColor = Color.FromArgb("#FFCDD2");
            button.BorderColor = Color.FromArgb("#E57373");
            button.Shadow.Opacity = 0.2f;
            button.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#FF8A80"));
        }
    }

    private async Task EndTutorial()
    {
        await AddTextCooldown("Tutorial12", 2000);
        if (Application.Current != null && Application.Current.MainPage?.Navigation != null)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }

    private async void LanguageButton_OnClicked(object? sender, EventArgs e)
    {
        try
        {
            if (Preferences.Get("language", "en") == "en")
            {
                Preferences.Set("language", "de");
            }
            else if (Preferences.Get("language", "en") == "de")
            {
                Preferences.Set("language", "fr");
            }
            else if (Preferences.Get("language", "en") == "fr")
            {
                Preferences.Set("language", "en");
            }

            LanguageManager.Update();
            UpdateLanguage();
        }
        catch (Exception)
        {
            //Again Nothing
        }
    }

    private void SkipText (object? sender, EventArgs eventArgs)
    {
        _textAnimationCts.Cancel();
        TutorialLabel.Text = LanguageManager.GetText(lastText);
    }
}