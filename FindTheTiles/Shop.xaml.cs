namespace FindTheTiles;

public partial class Shop
{
    private int _coins;
    private TapGestureRecognizer _recoSeal = new TapGestureRecognizer();
    private TapGestureRecognizer _recoSearch = new TapGestureRecognizer();
    private TapGestureRecognizer _recoBomb = new TapGestureRecognizer();
    private TapGestureRecognizer _recoTrie = new TapGestureRecognizer();
    private TapGestureRecognizer _recoMulti = new TapGestureRecognizer();

    private readonly int[] _triePrices = { 50, 100, 200, 350, 500 };
    private readonly int[] _multiPrices = { 30, 50, 100, 150, 250, 350, 450, 600, 750, 900 };
    public Shop()
    {
        InitializeComponent();
        NavigationPage.SetHasBackButton(this, false);
        _coins = Preferences.Get("Coins", 0);
        UpdateArticle();
        UpdateLanguage();
        CreateRecognizers();
        Temporary(null, null);
    }


    private void CreateRecognizers()
    {
        _recoSeal.Tapped += (s, e) => Buy_Seal(s, e);
        SealFrame.GestureRecognizers.Add(_recoSeal);
        _recoSearch.Tapped += (s, e) => Buy_Searcher(s, e);
        SearcherFrame.GestureRecognizers.Add(_recoSearch);
        _recoBomb.Tapped += (s, e) => Buy_Bomb(s, e);
        BombFrame.GestureRecognizers.Add(_recoBomb);
        _recoTrie.Tapped += (s, e) => Buy_Fails(s, e);
        TriesFrame.GestureRecognizers.Add(_recoTrie);
        _recoMulti.Tapped += (s, e) => Buy_Multiplyer(s, e);
        multiplyerFrame.GestureRecognizers.Add(_recoMulti);
    }
    private void UpdateLanguage()
    {
        Titel.Text = LanguageManager.GetText("Shop");
        seal.Text = LanguageManager.GetText("Seal");
        sealDescription.Text = LanguageManager.GetText("SealDescription");
        sealCost.Text = LanguageManager.GetText("Price");
        sealOwning.Text = LanguageManager.GetText("Owning");
        Product1.Text = LanguageManager.GetText("Searcher");
        Product1Description.Text = LanguageManager.GetText("SearcherDescription");
        Product1Owning.Text = LanguageManager.GetText("Owning");
        BuySearcherPrice.Text = LanguageManager.GetText("Price");
        Product2.Text = LanguageManager.GetText("Bomb");
        Product2Description.Text = LanguageManager.GetText("SearcherDescription");
        Product2Owning.Text = LanguageManager.GetText("Owning");
        BuyBombPrice.Text = LanguageManager.GetText("Price");
        Product3.Text = LanguageManager.GetText("Fails");
        Product3Description.Text = LanguageManager.GetText("FailDescription");
        Product3Owning.Text = LanguageManager.GetText("Owning");
        BuyFailPrice.Text = LanguageManager.GetText("Price");
        BuyMultiplyerPrice.Text = LanguageManager.GetText("Price");
        Product4.Text = LanguageManager.GetText("Multi");
        Product4Description.Text = LanguageManager.GetText("MultiDescription");
        Product4Owning.Text = LanguageManager.GetText("Owning");
        ToolTipProperties.SetText(HelpButton, LanguageManager.GetText("TooltipTutorial"));
        ToolTipProperties.SetText(LanguageButton, LanguageManager.GetText("TooltipLanguage"));
        ToolTipProperties.SetText(ExitButton, LanguageManager.GetText("TooltipExit"));
        if (Preferences.Get("language", "en") == "en")
            LanguageButton.Source = "english.png";
        else if (Preferences.Get("language", "en") == "de")
            LanguageButton.Source = "german.png";
        else if (Preferences.Get("language", "en") == "fr")
            LanguageButton.Source = "frensh.png";
    }

    private void UpdateArticle()
    {
        UpdateLanguage();
        TileCoinsCountLabel.Text = $"{_coins}";
        StarCountLabel.Text = Preferences.Get("Star", 0).ToString();
        sealCountLabel.Text = Preferences.Get("Seals", 0).ToString();
        BombCountLabel.Text = Preferences.Get("Bombs", 0).ToString();
        SearcherCountLabel.Text = Preferences.Get("Searchers", 0).ToString();
        FailCountLabel.Text = Preferences.Get("Tries", 1).ToString();
        MultiplyerLabelCountLabel.Text = Preferences.Get("Multiplyer", 1.0).ToString();

        sealCostLabel.Text = 2.ToString();
        BuySearcherCosts.Text = 10.ToString();
        BuyBombCosts.Text = 25.ToString();
        BuyFailCosts.Text = Preferences.Get("Tries", 1) <=5 ? _triePrices[Preferences.Get("Tries", 1) - 1].ToString() : "-------";
        BuyMultiplyerCosts.Text = Preferences.Get("MultiplyerBuys", 1) <= 5.0 ? _multiPrices[Preferences.Get("MultiplyerBuys", 1) - 1].ToString() : "-------";

        SealFrame.BackgroundColor = _coins >= 2 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");

        SearcherFrame.BackgroundColor = _coins >= 10 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");

        BombFrame.BackgroundColor = _coins >= 25 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");

        if (Preferences.Get("Tries", 1) < _triePrices.Length)
            TriesFrame.BackgroundColor = _coins >= _triePrices[Preferences.Get("Tries", 1) - 1] ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        else
            TriesFrame.BackgroundColor = Color.FromArgb("#e2c9f2ff");
        
        if (Preferences.Get("MultiplyerBuys", 1) < _multiPrices.Length)
            multiplyerFrame.BackgroundColor = _coins >= _multiPrices[Preferences.Get("MultiplyerBuys", 1) - 1] ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        else
            multiplyerFrame.BackgroundColor = Color.FromArgb("#e2c9f2ff");
    }
    private void ExitButton_Clicked(object? sender, EventArgs e) { Navigation.PopAsync(); }

    //Switch beetwheen the Different Shop Sectors
    private void Temporary(object? sender, EventArgs e)
    {
        SealFrame.IsVisible = true;
        SearcherFrame.IsVisible = true;
        BombFrame.IsVisible = true;
        TriesFrame.IsVisible = false;
        multiplyerFrame.IsVisible = false;
    }
    private void Permanently(object? sender, EventArgs e)
    {
        SealFrame.IsVisible = false;
        SearcherFrame.IsVisible = false;
        BombFrame.IsVisible = false;
        TriesFrame.IsVisible = true;
        multiplyerFrame.IsVisible = true;
    }
    private void Visualy(object? sender, EventArgs e)
    {
        SealFrame.IsVisible = false;
        SearcherFrame.IsVisible = false;
        BombFrame.IsVisible = false;
        TriesFrame.IsVisible = false;
        multiplyerFrame.IsVisible = false;
    }

    private void Buy_Seal(object? sender, EventArgs e)
    {
        if (_coins >= 2)
        {
            _coins -= 2;
            Preferences.Set("Coins", _coins);
            Preferences.Set("Seals", Preferences.Get("Seals", 0) + 1);
        }
        UpdateArticle();
    }
    private void Buy_Searcher(object? sender, EventArgs e)
    {
        if (_coins >= 10)
        {
            _coins -= 10;
            Preferences.Set("Coins", _coins);
            Preferences.Set("Searchers", Preferences.Get("Searchers", 0) + 1);
        }
        UpdateArticle();
    }

    private void Buy_Bomb(object? sender, EventArgs e)
    {
        if (_coins >= 25)
        {
            _coins -= 25;
            Preferences.Set("Coins", _coins);
            Preferences.Set("Bombs", Preferences.Get("Bombs", 0) + 1);
        }
        UpdateArticle();
    }

    private void Buy_Fails(object? sender, EventArgs e)
    {
        if (_coins >= _triePrices[Preferences.Get("Tries", 1) - 1] && Preferences.Get("Tries", 1) <=5)
        {
            _coins -= _triePrices[Preferences.Get("Tries", 1) - 1];
            Preferences.Set("Coins", _coins);
            Preferences.Set("Tries", Preferences.Get("Tries", 1)+1);
        }
        UpdateArticle();
    }

    private void Buy_Multiplyer(object? sender , EventArgs e)
    {
        switch(Preferences.Get("Multiplyer", 1.0))
        {
            case 1:
                if(_coins >= 30)
                {
                    _coins -= 30;
                    Preferences.Set("Coins", _coins);
                    Preferences.Set("Multiplyer", 1.25);
                    Preferences.Set("MultiplyerBuys", Preferences.Get("MultiplyerBuys", 1)+1);
                }
                break;
            case 1.25:
                if(_coins >= 50)
                {
                    _coins -= 50;
                    Preferences.Set("Coins", _coins);
                    Preferences.Set("Multiplyer", 1.5);
                    Preferences.Set("MultiplyerBuys", Preferences.Get("MultiplyerBuys", 1)+1);
                }
                break;
            case 1.5:
                if(_coins >= 100)
                {
                    _coins -= 100;
                    Preferences.Set("Coins", _coins);
                    Preferences.Set("Multiplyer", 1.75);
                    Preferences.Set("MultiplyerBuys", Preferences.Get("MultiplyerBuys", 1)+1);
                }
                break;
            case 1.75:
                if(_coins >= 150)
                {
                    _coins -= 150;
                    Preferences.Set("Coins", _coins);
                    Preferences.Set("Multiplyer", 2.0);
                    Preferences.Set("MultiplyerBuys", Preferences.Get("MultiplyerBuys", 1)+1);
                }
                break;
            case 2.0:
                if(_coins >= 250)
                {
                    _coins -= 250;
                    Preferences.Set("Coins", _coins);
                    Preferences.Set("Multiplyer", 2.5);
                    Preferences.Set("MultiplyerBuys", Preferences.Get("MultiplyerBuys", 1)+1);
                }
                break;
            case 2.5:
                if(_coins >= 350)
                {
                    _coins -= 350;
                    Preferences.Set("Coins", _coins);
                    Preferences.Set("Multiplyer", 3.0);
                    Preferences.Set("MultiplyerBuys", Preferences.Get("MultiplyerBuys", 1)+1);
                }
                break;
            case 3.0:
                if(_coins >= 450)
                {
                    _coins -= 450;
                    Preferences.Set("Coins", _coins);
                    Preferences.Set("Multiplyer", 3.5);
                    Preferences.Set("MultiplyerBuys", Preferences.Get("MultiplyerBuys", 1)+1);
                }
                break;
            case 3.5:
                if(_coins >= 600)
                {
                    _coins -= 600;
                    Preferences.Set("Coins", _coins);
                    Preferences.Set("Multiplyer", 4.0);
                    Preferences.Set("MultiplyerBuys", Preferences.Get("MultiplyerBuys", 1)+1);
                }
                break;
            case 4.0:
                if(_coins >= 750)
                {
                    _coins -= 750;
                    Preferences.Set("Coins", _coins);
                    Preferences.Set("Multiplyer", 4.5);
                    Preferences.Set("MultiplyerBuys", Preferences.Get("MultiplyerBuys", 1)+1);
                }
                break;
            case 4.5:
                if(_coins >= 900)
                {
                    _coins -= 900;
                    Preferences.Set("Coins", _coins);
                    Preferences.Set("Multiplyer", 5.0);
                    Preferences.Set("MultiplyerBuys", Preferences.Get("MultiplyerBuys", 1)+1);
                }
                break;
        }
        UpdateArticle();
    }

    private async void HelpButton_OnClicked(object? sender, EventArgs e)
    {
        try
        {
            if (Application.Current?.MainPage?.Navigation != null)
                await Application.Current.MainPage.Navigation.PushAsync(new Tutorial(), true);
        }
        catch (Exception) {/*inactive*/}
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
        UpdateLanguage();
    }
}