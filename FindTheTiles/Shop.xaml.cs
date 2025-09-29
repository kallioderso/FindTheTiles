namespace FindTheTiles;

public partial class Shop
{
    private int _coins;
    public Shop()
    {
        InitializeComponent();
        NavigationPage.SetHasBackButton(this, false);
        _coins = Preferences.Get("Coins", 0);
        UpdateArticle();
        UpdateLanguage();
    }

    private void UpdateLanguage()
    {
        Titel.Text = LanguageManager.GetText("Shop");
        Product1.Text = LanguageManager.GetText("Searcher");
        Product1Description.Text = LanguageManager.GetText("SearcherDescription");
        Product1Owning.Text = LanguageManager.GetText("Owning");
        BuySearcherButton.Text = LanguageManager.GetText("SearcherPrice");
        Product2.Text = LanguageManager.GetText("Bomb");
        Product2Description.Text = LanguageManager.GetText("SearcherDescription");
        Product2Owning.Text = LanguageManager.GetText("Owning");
        BuyBombButton.Text = LanguageManager.GetText("BombPrice");
        Product3.Text = LanguageManager.GetText("Fails");
        Product3Description.Text = LanguageManager.GetText("FailDescription");
        Product3Owning.Text = LanguageManager.GetText("Owning");
        Product4.Text = LanguageManager.GetText("Multi");
        Product4Description.Text = LanguageManager.GetText("MultiDescription");
        Product4Owning.Text = LanguageManager.GetText("Owning");
        BuyFailButton.Text = LanguageManager.GetText($"FailPrice{Preferences.Get("Tries", 1)}");
        BuyMultiplyerButton.Text = LanguageManager.GetText($"MultiPrice{Preferences.Get("MultiplyerBuys", 1)}");
        ToolTipProperties.SetText(HelpButton, LanguageManager.GetText("TooltipTutorial"));
        ToolTipProperties.SetText(LanguageButton, LanguageManager.GetText("TooltipLanguage"));
        ToolTipProperties.SetText(BuySearcherButton, LanguageManager.GetText("TooltipSearcherBuy"));
        ToolTipProperties.SetText(BuyBombButton , LanguageManager.GetText("TooltipBombBuy"));
        ToolTipProperties.SetText(BuyFailButton, LanguageManager.GetText("TooltipFailBuy"));
        ToolTipProperties.SetText(BuyMultiplyerButton, LanguageManager.GetText("TooltipMultiBuy"));
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
        BombCountLabel.Text = Preferences.Get("Bombs", 0).ToString();
        SearcherCountLabel.Text = Preferences.Get("Searchers", 0).ToString();
        FailCountLabel.Text = Preferences.Get("Tries", 1).ToString();
        MultiplyerLabelCountLabel.Text = Preferences.Get("Multiplyer", 1.0).ToString();

        BuyBombButton.BackgroundColor = _coins >= 25 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        BuySearcherButton.BackgroundColor = _coins >= 10 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        if(Preferences.Get("Tries", 1) == 1)
            BuyFailButton.BackgroundColor = _coins >= 50 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        else if(Preferences.Get("Tries", 1) == 2)
            BuyFailButton.BackgroundColor = _coins >= 100 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        else if(Preferences.Get("Tries", 1) == 3)
            BuyFailButton.BackgroundColor = _coins >= 200 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        else if(Preferences.Get("Tries", 1) == 4)
            BuyFailButton.BackgroundColor = _coins >= 350 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        else if(Preferences.Get("Tries", 1) == 5)
            BuyFailButton.BackgroundColor = _coins >= 500 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        else
            BuyFailButton.BackgroundColor = Color.FromArgb("#e2c9f2ff");

        if(Preferences.Get("MultiplyerBuys", 1) == 1)
            BuyMultiplyerButton.BackgroundColor = _coins >= 30 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        else if(Preferences.Get("MultiplyerBuys", 1) == 2)
            BuyMultiplyerButton.BackgroundColor = _coins >= 50 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        else if(Preferences.Get("MultiplyerBuys", 1) == 3)
            BuyMultiplyerButton.BackgroundColor = _coins >= 100 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        else if(Preferences.Get("MultiplyerBuys", 1) == 4)
            BuyMultiplyerButton.BackgroundColor = _coins >= 150 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        else if(Preferences.Get("MultiplyerBuys", 1) == 5)
            BuyMultiplyerButton.BackgroundColor = _coins >= 250 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        else if(Preferences.Get("MultiplyerBuys", 1) == 6)
            BuyMultiplyerButton.BackgroundColor = _coins >= 350 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        else if(Preferences.Get("MultiplyerBuys", 1) == 7)
            BuyMultiplyerButton.BackgroundColor = _coins >= 450 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        else if(Preferences.Get("MultiplyerBuys", 1) == 8)
            BuyMultiplyerButton.BackgroundColor = _coins >= 600 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        else if(Preferences.Get("MultiplyerBuys", 1) == 9)
            BuyMultiplyerButton.BackgroundColor = _coins >= 750 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        else if(Preferences.Get("MultiplyerBuys", 1) == 10)
            BuyMultiplyerButton.BackgroundColor = _coins >= 900 ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#e2c9f2ff");
        else
            BuyMultiplyerButton.BackgroundColor = Color.FromArgb("#e2c9f2ff");
    }
    private void ExitButton_Clicked(object? sender, EventArgs e) { Navigation.PopAsync(); }

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
        switch(Preferences.Get("Tries", 1))
        {
            case 1:
                if(_coins >= 50)
                {
                    _coins -= 50;
                    Preferences.Set("Coins", _coins);
                    Preferences.Set("Tries", 2);
                }
                break;
            case 2:
                if(_coins >= 100)
                {
                    _coins -= 100;
                    Preferences.Set("Coins", _coins);
                    Preferences.Set("Tries", 3);
                }
                break;
            case 3:
                if(_coins >= 200)
                {
                    _coins -= 200;
                    Preferences.Set("Coins", _coins);
                    Preferences.Set("Tries", 4);
                }
                break;
            case 4:
                if(_coins >= 350)
                {
                    _coins -= 350;
                    Preferences.Set("Coins", _coins);
                    Preferences.Set("Tries", 5);
                }
                break;
            case 5:
                if(_coins >= 500)
                {
                    _coins -= 500;
                    Preferences.Set("Coins", _coins);
                    Preferences.Set("Tries", 6);
                }
                break;
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
        catch (Exception){/*inactive*/}
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