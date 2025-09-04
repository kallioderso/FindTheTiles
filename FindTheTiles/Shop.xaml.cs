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
        Product2.Text = LanguageManager.GetText("Searcher");
        Product2Description.Text = LanguageManager.GetText("SearcherDescription");
        Product2Owning.Text = LanguageManager.GetText("Owning");
        BuyBombButton.Text = LanguageManager.GetText("BombPrice");
        ToolTipProperties.SetText(LanguageButton, LanguageManager.GetText("TooltipLanguage"));
    }

    [Obsolete]
    private void UpdateArticle()
    {
        BombCountLabel.Text = Preferences.Get("Bombs", 0).ToString();
        SearcherCountLabel.Text = Preferences.Get("Searchers", 0).ToString();

        BuyBombButton.BackgroundColor = _coins >= 25 ? Color.FromHex("#FFFFFF") : Color.FromHex("#CCCCCC");
        BuySearcherButton.BackgroundColor = _coins >= 10 ? Color.FromHex("#FFFFFF") : Color.FromHex("#CCCCCC");
    }
    private void ExitButton_Clicked(object? sender, EventArgs e) { Navigation.PopAsync(); }

    [Obsolete]
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

    [Obsolete]
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

    private async void HelpButton_OnClicked(object? sender, EventArgs e)
    {
        try
        {
            if (Application.Current?.MainPage?.Navigation != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new Tutorial(), true);
            }
        }
        catch (Exception)
        {
            //inactive
        }
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