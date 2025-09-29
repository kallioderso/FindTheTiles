using System.Text;

namespace FindTheTiles;

public partial class Tutorial : IDisposable
{
    // Konstanten für bessere Lesbarkeit
    private const int _gridSize = 7;
    private const int _totalPatternTiles = 16;
    private const int _textDelays = 20;
    private const int _textRemovalDelays = 10;
    
    private Button _startbutton = null!;
    private readonly Button[,] _buttons = new Button[_gridSize, _gridSize];
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

    private CancellationTokenSource? _textAnimationCts;
    private bool TextCreating = false;
    private int TutorialSection = 0;
    private int TutorialPart = 0;
    private bool _disposed = false;

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
        catch (Exception ex){System.Diagnostics.Debug.WriteLine($"Error in OnAppearing: {ex.Message}");}
    }

    private void UpdateLanguage()
    {
        var language = Preferences.Get("language", "en");
        LanguageButton.Source = language switch
        {
            "en" => "english.png",
            "de" => "german.png",
            "fr" => "frensh.png",
            _ => "english.png"
        };
        
        ToolTipProperties.SetText(LanguageButton, $"{LanguageManager.GetText("TooltipLanguage")}");
        ToolTipProperties.SetText(ExitButton, LanguageManager.GetText("TooltipExit"));
    }

    private async void ExitButton_Clicked(object? sender, EventArgs e){await Navigation.PopAsync();}
    
    ////////////////////////////////////
    ///////Tutorial Text Methods////////
    ////////////////////////////////////
    private async Task AddText(string text, CancellationToken token)
    {
        if (_disposed) return;
        
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            Froze();
            TutorialPart++;
            await RemoveText(token);
            await Task.Delay(200, token);
            
            var sb = new StringBuilder();
            TutorialLabel.Text = "";
            
            foreach (var letter in text)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(_textDelays, token);
                sb.Append(letter);
                TutorialLabel.Text = sb.ToString();
            }
            TextCreating = false;
        });
    }

    private async Task RemoveText(CancellationToken token)
    {
        var text = TutorialLabel.Text;
        var letters = text.Length;
        
        for (int i = letters - 1; i >= 0; i--)
        {
            token.ThrowIfCancellationRequested();
            await Task.Delay(_textRemovalDelays, token);
            TutorialLabel.Text = text.Substring(0, i);
        }
        TutorialLabel.Text = "";
    }
    
    private async Task AddTextes(string textKey)
    {
        if (_disposed) return;
        
        lastText = textKey;
        
        // Dispose previous token
        _textAnimationCts?.Cancel();
        _textAnimationCts?.Dispose();
        _textAnimationCts = new CancellationTokenSource();
        
        TextCreating = true;
        
        try
        {await AddText(LanguageManager.GetText(textKey), _textAnimationCts.Token);}
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in AddTextes: {ex.Message}");
            TextCreating = false;
        }
    }

    ////////////////////////////////////
    /////Generating Purpose Methods/////
    ////////////////////////////////////
    private void GenerateTiles()
    {
        for (int row = 0; row < _gridSize; row++)
        {
            for (int col = 0; col < _gridSize; col++)
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
        if (_disposed) return;
        
        await Task.Delay(2000);
        
        // Phase 1: Hide outer ring
        for (int row = 0; row < _gridSize; row++)
            for (int col = 0; col < _gridSize; col++)
                if (_disappearing[row, col] == 0) 
                    _buttons[row, col].IsVisible = false;
        
        await Task.Delay(500);
        
        // Phase 2: Hide middle ring
        for (int row = 0; row < _gridSize; row++)
            for (int col = 0; col < _gridSize; col++)
                if (_disappearing[row, col] == 1) 
                    _buttons[row, col].IsVisible = false;
        
        await Task.Delay(500);
        
        // Phase 3: Hide inner ring
        for (int row = 0; row < _gridSize; row++)
            for (int col = 0; col < _gridSize; col++)
                if (_disappearing[row, col] == 2) 
                    _buttons[row, col].IsVisible = false;
        
        await Task.Delay(500);
        
        _startbutton.BorderColor = Color.FromArgb("#4CAF50");
        _startbutton.Clicked += StartPoint_Clicked;
        await AddTextes("Tutorial1");
        TutorialPart = 0;
        Unfroze();
    }

    ////////////////////////////////////
    ///////////Tutorial Flow////////////
    ////////////////////////////////////
    private async void StartPoint_Clicked(object? sender, EventArgs e)
    {
        if (_disposed) return;
        
        TutorialSection = 2;
        try
        {
            if(TutorialPart == 0)
            {
                _startbutton.BackgroundColor = Color.FromArgb("#D0E0FF");
                _startbutton.BorderColor = Color.FromArgb("#A0B8FF");
                _startbutton.Shadow.Opacity = 0.2f;
                _startbutton.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#B0C4FF"));
                _startbutton.Clicked -= StartPoint_Clicked;
                await AddTextes("Tutorial2");
            }
            else if(TutorialPart == 1 && !TextCreating)
                await AddTextes("Tutorial3");
            else if (TutorialPart == 2 && !TextCreating)
            {
                for (int row = 0; row < _gridSize; row++)
                {
                    for (int col = 0; col < _gridSize; col++)
                    {
                        if (_disappearing[row, col] == 2) 
                            _buttons[row, col].IsVisible = true;
                    }
                }
                await AddTextes("Tutorial4");
                _startbutton.Text = Generator.GetNeighborCount(3, 3, _pattern, _buttons).ToString();
            }
            else if (TutorialPart == 3 && !TextCreating)
            {
                await AddTextes("Tutorial5");
                RemoveButtonFromList(_startbutton);
                
                // Sicher Event Handler hinzufügen
                AddSingleEventHandler(_buttons[3, 2], MusterClicked);
                AddSingleEventHandler(_buttons[2, 3], MusterClicked);
                AddSingleEventHandler(_buttons[3, 4], WrongClicked);
                AddSingleEventHandler(_buttons[4, 3], WrongClicked);
                
                TutorialPart = 0;
                Unfroze();
            }
        }
        catch (Exception ex){System.Diagnostics.Debug.WriteLine($"Error in StartPoint_Clicked: {ex.Message}");}
    }

    private void AddSingleEventHandler(Button button, EventHandler handler)
    {
        // Erst alle entfernen, dann sicher hinzufügen
        button.Clicked -= MusterClicked;
        button.Clicked -= WrongClicked;
        button.Clicked -= StartPoint_Clicked;
        button.Clicked += handler;
    }

    private async void WrongClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button || _disposed) return;
        
        try
        {
            button.BackgroundColor = Color.FromArgb("#FFCDD2");
            button.BorderColor = Color.FromArgb("#E57373");
            button.Shadow.Opacity = 0.2f;
            button.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#FF8A80"));
            UnEnable(button);
            button.Clicked -= WrongClicked;
            await AddTextes("Tutorial6");
            TutorialPart--;
            Unfroze();
        }
        catch (Exception ex){System.Diagnostics.Debug.WriteLine($"Error in WrongClicked: {ex.Message}");}
    }

    private async void MusterClicked(object? sender, EventArgs e)
    {
        if (_disposed) return;
        
        TutorialSection = 3;
        try
        {
            if(sender is Button button)
            {
                button.BackgroundColor = Color.FromArgb("#D0E0FF");
                button.BorderColor = Color.FromArgb("#A0B8FF");
                button.Shadow.Opacity = 0.2f;
                button.Shadow.Brush = new SolidColorBrush(Color.FromArgb("#B0C4FF"));
                UnEnable(button);
                button.Clicked -= MusterClicked;
                _musterTiles++;
            }
            
            if (_musterTiles == 2)
            {
                if(TutorialPart == 0)
                    await AddTextes("Tutorial7");
                else if(TutorialPart == 1 && !TextCreating)
                {
                    await AddTextes("Tutorial8");
                    
                    // Show middle ring
                    for (int row = 0; row < _gridSize; row++)
                    {
                        for (int col = 0; col < _gridSize; col++)
                            if (_disappearing[row, col] == 1) 
                                _buttons[row, col].IsVisible = true;
                    }
                    await Task.Delay(500);
                    
                    // Show outer ring
                    for (int row = 0; row < _gridSize; row++)
                    {
                        for (int col = 0; col < _gridSize; col++)
                            if (_disappearing[row, col] == 0) 
                                _buttons[row, col].IsVisible = true;
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
                    _musterTiles++;
                }
                else if(TutorialPart == 2 && !TextCreating)
                {
                    await AddTextes("Tutorial9");
                    FortschritsFrame.IsEnabled = true;
                    FortschritsFrame.IsVisible = true;
                    FinishProgress.Progress = ((double)_musterTiles / _totalPatternTiles);
                    ActivateNormalGame();
                }
            }
            else
            {
                await AddTextes("Tutorial10");
                if(TutorialPart == 1 && !TextCreating)
                {
                    await AddTextes("Tutorial11");
                    TutorialPart = 0;
                }
            }
            Unfroze();
        }
        catch (Exception ex){System.Diagnostics.Debug.WriteLine($"Error in MusterClicked: {ex.Message}");}
    }

    private void RemoveButtonFromList(Button button)
    {
        var row = TilesGrid.GetRow(button);
        var col = TilesGrid.GetColumn(button);
        
        if (row >= 0 && row < _gridSize && col >= 0 && col < _gridSize)
            _buttons[row, col].IsEnabled = false;
    }
    
    private void Froze()
    {
        for (int row = 0; row < _gridSize; row++)
            for (int col = 0; col < _gridSize; col++)
                if (_buttons[row, col].IsVisible)
                    UnEnable(_buttons[row, col]);
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
        for (int row = 0; row < _gridSize; row++)
        {
            for (int col = 0; col < _gridSize; col++)
            {
                var button = _buttons[row, col];
                if (button.IsVisible)
                {
                    // Startbutton immer aktivieren
                    if (button == _startbutton && button.BorderColor.ToArgbHex() == Color.FromArgb("#4CAF50").ToArgbHex())
                        button.IsEnabled = true;
                    // Andere Buttons wie gehabt
                    else if (button.BackgroundColor.ToArgbHex() == Color.FromArgb("#FFFFFF").ToArgbHex() ||
                             button.BackgroundColor.ToArgbHex() == Color.FromArgb("#FFF8DC").ToArgbHex())
                        button.IsEnabled = true;
                }
            }
        }
    }

    private void ActivateNormalGame()
    {
        for (int row = 0; row < _gridSize; row++)
        {
            for (int col = 0; col < _gridSize; col++)
            {
                var button = _buttons[row, col];

                // Entferne alle vorherigen Handler sicher
                button.Clicked -= MusterClicked;
                button.Clicked -= WrongClicked;
                button.Clicked -= StartPoint_Clicked;

                // Füge neuen Handler hinzu basierend auf Pattern
                if (_pattern[row, col])
                    button.Clicked += async (_, _) => await TileClicked(button, true);
                else
                    button.Clicked += async (_, _) => await TileClicked(button, false);
            }
        }
    }

    private async Task TileClicked(Button button, bool isPattern)
    {
        if (_disposed) return;
        
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
            FinishProgress.Progress = ((double)_musterTiles / _totalPatternTiles);
            
            if(_musterTiles == _totalPatternTiles)
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
        await AddTextes("Tutorial12");
        if (Application.Current?.MainPage?.Navigation != null)
            await Application.Current.MainPage.Navigation.PopAsync();
    }

    private void LanguageButton_OnClicked(object? sender, EventArgs e)
    {
        try
        {
            var currentLanguage = Preferences.Get("language", "en");
            var newLanguage = currentLanguage switch
            {
                "en" => "de",
                "de" => "fr",
                "fr" => "en",
                _ => "en"
            };
            
            Preferences.Set("language", newLanguage);
            LanguageManager.Update();
            UpdateLanguage();
        }
        catch (Exception ex){System.Diagnostics.Debug.WriteLine($"Error in LanguageButton_OnClicked: {ex.Message}");}
    }

    private void ClickText(object? sender, EventArgs eventArgs)
    {
        if (_disposed) return;
        
        if(TextCreating)
        {
            _textAnimationCts?.Cancel();
            TutorialLabel.Text = LanguageManager.GetText(lastText); 
            TextCreating = false;
        }
        else
        {
            if(TutorialSection == 2)
                StartPoint_Clicked(null, EventArgs.Empty);
            else if(TutorialSection == 3)
                MusterClicked(null, EventArgs.Empty);
        }
    }

    // IDisposable Implementation
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _textAnimationCts?.Cancel();
            _textAnimationCts?.Dispose();
            _disposed = true;
        }
    }

    ~Tutorial(){Dispose(false);}
}