using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FindTheTiles.Model;

namespace FindTheTiles.ViewModel;

public class Level1ViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public int _score { get; set; }
    public int _trys { get; set; }
    public double _multiplier { get; set; }
    public double _progress { get; set; }
    public string _bombObtained { get; set; } = "nobomb.png";
    public string _searcherObtained { get; set; } = "nosearch.png";
    public string _sealObtained { get; set; } = "nosiegel.png";
    public string _multiplierText { get; private set; }
    public string _scoreText { get; private set; }
    public string _tryText { get; private set; }
    public string _progressText { get; private set; }
    public string _staticTextProgress {get; private set;}
    public string _staticTextMultiplyer {get; private set;}
    public string _staticTextScore {get; private set;}
    public string _staticTextTrys {get; private set;}
    public string _staticTextExitTT {get; private set;}
    public string _staticTextSearcherTT {get; private set;}
    public string _staticTextBombTT {get; private set;}
    public string _staticTextSealTT { get; private set; }
    public string _staticTextTutorialTT {get; private set;}
    public string _staticTextLanguageTT {get; private set;}
    public bool _languagesettings { get; set; }
    public TextTileButton[,] _tiles { get; set; }
    public SpecialTileButton[,] _SpecialTiles { get; set; }
    public ICommand _clickCommand { get; }
    public ICommand _exitCommand { get; }
    public ICommand _searchCommand { get; }
    public ICommand _bombCommand { get; }
    public ICommand _sealCommand { get; }
    public ICommand _tutorialCommand { get; }
    public ICommand _languageCommand { get; }
    public ICommand _germanCommand { get; }
    public ICommand _frenshCommand { get; }
    public ICommand _englishCommand { get; }
    private Pattern _pattern { get; set; }
    private int TileWidth = 44;
    private int TileHeight = 44;
    private int _foundTiles = 0;
    private int _totalTiles = 0;
    private bool _gameOnPause = false;
    private bool _bombMode = false;
    private bool _sealMode = false;
    private bool _bombPlaced = false;
    private bool _bombInInventory
    {
        get => _bombObtained == "bomb.png";
        set => _bombObtained = value ? "bomb.png" : "nobomb.png";
    }
    private bool _searcherInInventory
    {
        get => _searcherObtained == "searcher.png";
        set => _searcherObtained = value ? "searcher.png" : "nosearch.png";
    }

    private bool _SealInInventory
    {
        get => _sealObtained == "siegel.png";
        set => _sealObtained = value ? "siegel.png" : "nosiegel.png";
    }

    private Page _page;

    public Level1ViewModel()
    {
        _clickCommand = new Command<TextTileButton>(TileClicked);
        _exitCommand = new Command<ImageButton>(ExitButton);
        _searchCommand = new Command<ImageButton>(SearchItem);
        _bombCommand = new Command<ImageButton>(BombItem);
        _sealCommand = new Command<ImageButton>(SealItem);
        _tutorialCommand = new Command<ImageButton>(TutorialButton);
        _languageCommand = new Command<ImageButton>(LanguageButton);
        _germanCommand = new Command<ImageButton>(btn => LanguageChange("de", btn));
        _frenshCommand = new Command<ImageButton>(btn => LanguageChange("fr", btn));
        _englishCommand = new Command<ImageButton>(btn => LanguageChange("en", btn));
        _bombInInventory = Preferences.Get("Bombs", 0) > 0;
        _searcherInInventory = Preferences.Get("Searchers", 0) > 0;
        if (Preferences.Get("Resume", false))
        {
            _score = Preferences.Get("ResumeScore", 0);
            _multiplier = Preferences.Get("ResumeMultiplier", 1.0);
            _trys = Preferences.Get("ResumeTrys", 0);
            _foundTiles = Preferences.Get("ResumeFound", 0);
            _totalTiles = Preferences.Get("ResumeMax", 0);
            _progress = (double)_foundTiles / _totalTiles;
        }
        else
        {
            _score = 0;
            _trys = Preferences.Get("Tries", 0);
            _multiplier = Preferences.Get("Multiplyer", 1.0);
            _progress = 0.01;
        }
        _tiles = new TextTileButton[7, 7]; // <--- Initialisierung hinzugefügt
        _SpecialTiles = new SpecialTileButton[7, 7];
        ChangeLanguageItems();
    }

    public void GenerateTiles(Pattern pattern, Page page)
    {
        _pattern = Preferences.Get("Resume", false) ? new Pattern(deserializer.Pattern(Preferences.Get("ResumePattern", ""))) : pattern;
        if(!Preferences.Get("Resume", false))
            _totalTiles = _pattern.TilesAmount;
        _page = page;
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
            {
                _SpecialTiles[i, j] = new SpecialTileButton();
                var tile = new TextTileButton(TileWidth, TileHeight);
                tile.Row = i;
                tile.Column = j;
                tile._neighbor = _pattern.TilesNeighbors[i, j];
                tile._bomb = false;
                tile._searcher = false;
                if (!Preferences.Get("Resume", false))
                    tile._state = _pattern.StartTiles.Contains((i, j)) ? 1 : 0;
                else
                    tile._state = deserializer.States(Preferences.Get("ResumeState", ""))[i, j];
                if(tile._state != 2 && tile._state != 3)
                {
                    tile.Command = _clickCommand;
                    tile.CommandParameter = tile;
                }
                _tiles[i, j] = tile;
            }
        ChangeLanguageItems();
    }

    private void GenerateNewPattern()
    {
        _pattern = new Pattern();
        _foundTiles = 0;
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
            {
                var tile = _tiles[i, j];
                if (_pattern.StartTiles.Contains((i, j)))
                    tile._state = 1;
                else
                    tile._state = 0;
                tile._neighbor = _pattern.TilesNeighbors[i, j];
                tile._bomb = false;
                tile._searcher = false;
                tile.Command = _clickCommand;
                tile.CommandParameter = tile;
                tile.IsEnabled = true;
                _SpecialTiles[i, j]._visibel = false;
            }
        _totalTiles = _pattern.TilesAmount;
        _progress = 0.01;
        OnPropertyChanged(nameof(_progress));
        ChangeLanguageItems();
        var _rand = new Random();
        if (_rand.Next(1, 5) == 1)
        {
            var i = _rand.Next(1, 7);
            var j = _rand.Next(1, 7);
            _SpecialTiles[i, j]._state = 1;
            _SpecialTiles[i, j]._visibel = true;
        }
    }

    private async void TileClicked(TextTileButton tile)
    {
        if (_gameOnPause)
            return;
        
        if (_sealMode)
        {
            if (_SpecialTiles[tile.Row, tile.Column]._visibel)
                _SpecialTiles[tile.Row, tile.Column]._visibel = false;
            else if (Preferences.Get("Seals", 0) > 0)
            {
                _SpecialTiles[tile.Row, tile.Column]._visibel = true;
                Preferences.Set("Seals", Preferences.Get("Seals", 0) -1);
            }
            return;
        }
        else if (_SpecialTiles[tile.Row, tile.Column]._visibel)
        {
            if (_SpecialTiles[tile.Row, tile.Column]._state == 1)
            {
                _SpecialTiles[tile.Row, tile.Column]._state = 0;
                _SpecialTiles[tile.Row, tile.Column]._visibel = false;
                Preferences.Set("Star", Preferences.Get("Star", 0) + 1);
            }
            return;
        }
            

        tile.IsEnabled = false;
        if (_bombMode && !_bombPlaced)
        {
            _bombPlaced = true;
            tile._state = _pattern.IsPattern[tile.Row, tile.Column] ? 2 : 3;
            TileClicked(_tiles[tile.Row - 1, tile.Column]);
            TileClicked(_tiles[tile.Row + 1, tile.Column]);
            TileClicked(_tiles[tile.Row, tile.Column - 1]);
            TileClicked(_tiles[tile.Row, tile.Column + 1]);
            _bombPlaced = false;
            _bombMode = false;
            for(int i=0; i<7; i++)
                for(int j=0; j<7; j++)
                    _tiles[i,j]._bomb = false;
            return;
        }

        int rows = _tiles.GetLength(0);
        int cols = _tiles.GetLength(1);

        List<TextTileButton> neighbors = new List<TextTileButton>();

        if (tile.Row - 1 >= 0)
            neighbors.Add(_tiles[tile.Row - 1, tile.Column]);
        if (tile.Row + 1 < rows)
            neighbors.Add(_tiles[tile.Row + 1, tile.Column]);
        if (tile.Column - 1 >= 0)
            neighbors.Add(_tiles[tile.Row, tile.Column - 1]);
        if (tile.Column + 1 < cols)
            neighbors.Add(_tiles[tile.Row, tile.Column + 1]);

        foreach (var neighbor in neighbors)
        {
            if (neighbor._state == 0)
                neighbor._state = 4;
        }


        if (_pattern.IsPattern[tile.Row, tile.Column])
        {
            tile._state = 2;
            _score++;
            _foundTiles++;
            _progress = (double)_foundTiles / _totalTiles;
            OnPropertyChanged(nameof(_score));
            OnPropertyChanged(nameof(_progress));

            if (_foundTiles == _totalTiles)
            {
                _gameOnPause = true;
                await Task.Delay(1000);
                _multiplier += _multiplier * 0.1;
                OnPropertyChanged(nameof(_multiplier));
                _trys++;
                OnPropertyChanged(nameof(_trys));
                GenerateNewPattern();
            }
        }
        else
        {
            tile._state = 3;
            if (_bombMode)
                return;
            _trys--;
            OnPropertyChanged(nameof(_trys));
            if (_trys == 0)
            {
                _gameOnPause = true;
                Preferences.Set("Highscore", Preferences.Get("Highscore", 0) < _score ? _score : Preferences.Get("Highscore", 0));
                Preferences.Set("LastScore", _score);
                Preferences.Set("Coins", Preferences.Get("Coins", 0) + (int)(_score * _multiplier));
                Preferences.Set("Resume", false);
                await Task.Delay(2000);
                await _page.Navigation.PopAsync();
            }
        }
        _gameOnPause = false;
        ChangeLanguageItems();
    }

    private void ChangeLanguageItems()
    {
        _progressText = $"{LanguageManager.GetText("Progress")}: {_foundTiles}/{_totalTiles}";
        OnPropertyChanged(nameof(_progressText));
        _multiplierText = $"{LanguageManager.GetText("Multiplyer")}: {_multiplier}";
        OnPropertyChanged(nameof(_multiplierText));
        _scoreText = $"{LanguageManager.GetText("Score")}: {_score}";
        OnPropertyChanged(nameof(_scoreText));
        _tryText = $"{LanguageManager.GetText("RemainingAttempts")}: {_trys}";
        OnPropertyChanged(nameof(_tryText));
        _staticTextProgress = LanguageManager.GetText("Progress");
        OnPropertyChanged(nameof(_staticTextProgress));
        _staticTextMultiplyer = LanguageManager.GetText("Multiplyer");
        OnPropertyChanged(nameof(_staticTextMultiplyer));
        _staticTextScore = LanguageManager.GetText("Score");
        OnPropertyChanged(nameof(_staticTextScore));
        _staticTextTrys = LanguageManager.GetText("RemainingAttempts");
        OnPropertyChanged(nameof(_staticTextTrys));
        _staticTextExitTT = LanguageManager.GetText("TooltipExit");
        OnPropertyChanged(nameof(_staticTextExitTT));
        _staticTextSearcherTT = LanguageManager.GetText("TooltipSearcher");
        OnPropertyChanged(nameof(_staticTextSearcherTT));
        _staticTextBombTT = LanguageManager.GetText("TooltipBomb");
        OnPropertyChanged(nameof(_staticTextBombTT));
        _staticTextSealTT = LanguageManager.GetText("TooltipSeal");
        OnPropertyChanged(nameof(_staticTextSealTT));
        _staticTextTutorialTT = LanguageManager.GetText("TooltipTutorial");
        OnPropertyChanged(nameof(_staticTextTutorialTT));
        _staticTextLanguageTT = LanguageManager.GetText("TooltipLanguage");
        OnPropertyChanged(nameof(_staticTextLanguageTT));
    }
    private void SearchItem(ImageButton button)
    {
        if (Preferences.Get("Searchers", 0) == 0)
            return;
        Preferences.Set("Searchers", Preferences.Get("Searchers", 0)-1);
        (int row, int col) randomTile;
        do
        {
            var random = new Random();
            randomTile = (random.Next(0, 7), random.Next(0, 7));
        }
        while ((_tiles[randomTile.row, randomTile.col]._state != 0 && _tiles[randomTile.row, randomTile.col]._state != 4) || !_pattern.IsPattern[randomTile.row, randomTile.col]);
        _tiles[randomTile.row, randomTile.col]._searcher = true;
    }

    private void BombItem(ImageButton button)
    {
        if (_bombMode)
        {
            _bombMode = false;
            Preferences.Set("Bombs", Preferences.Get("Bombs", 0) + 1);
            for(int i=0; i<7; i++)
                for(int j=0; j<7; j++)
                    _tiles[i,j]._bomb = false;
        }
        else if (Preferences.Get("Bombs", 0) > 0)
        {
            _bombMode = true;
            Preferences.Set("Bombs", Preferences.Get("Bombs", 0) - 1);
            for(int i=0; i<7; i++)
                for(int j=0; j<7; j++)
                    _tiles[i,j]._bomb = true;
        }

    }

    private void SealItem(ImageButton button)
    {
        _sealMode = !_sealMode;
        _SealInInventory = !_SealInInventory;
        OnPropertyChanged(nameof(_sealObtained));
    }

    private void ExitButton(ImageButton button)
    {
        Preferences.Set("ResumeState", serializer.States(ChangeType.TextTilesToIntStates(_tiles)));
        Preferences.Set("ResumePattern", serializer.Pattern(_pattern.IsPattern));
        Preferences.Set("ResumeScore", _score);
        Preferences.Set("ResumeMultiplier", _multiplier);
        Preferences.Set("ResumeTrys", _trys);
        Preferences.Set("Resume", true);
        Preferences.Set("ResumeFound", _foundTiles);
        Preferences.Set("ResumeMax", _totalTiles);
        _page.Navigation.PopAsync();
    }

    private async void TutorialButton(ImageButton button)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new Tutorial(), true);
    }

    private void LanguageButton(ImageButton button) 
    {
        _languagesettings = !_languagesettings ;
        OnPropertyChanged(nameof(_languagesettings));
    }

    private async void LanguageChange(string language, ImageButton button)
    {
        if (!_languagesettings)
            return;

        Preferences.Set("language", language);
        LanguageManager.Update();

        _languagesettings = false;
        OnPropertyChanged(nameof(_languagesettings));
        ChangeLanguageItems();
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}