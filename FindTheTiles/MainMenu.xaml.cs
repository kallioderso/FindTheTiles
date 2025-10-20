using System.Diagnostics;
using FindTheTiles.Model;
using FindTheTiles.View;

namespace FindTheTiles;

public partial class MainMenu
{
    public MainMenu()
    {
        InitializeComponent();
        if(Preferences.Get("FirstStart", true))
            Application.Current?.MainPage?.Navigation?.PushAsync(new Tutorial(), true);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        int highscore = Preferences.Get("Highscore", 0);
        int lastScore = Preferences.Get("LastScore", 0);
        if (Preferences.Get("Resume", false))
            ResumeButton.IsVisible = true;
        else
            ResumeButton.IsVisible = false;
        HighscoreLabel.Text = highscore.ToString();
        CurrentScoreLabel.Text = lastScore.ToString();
        UpdateTileCoins();
        SetLanguage();
    }

    private void SetLanguage()
    {
        DifficultyLabelTitel.Text = LanguageManager.GetText("Difficulty");
        TileCoinsLabel.Text = LanguageManager.GetText("Coins");
        LabelCurrentScoreTitel.Text = LanguageManager.GetText("Score");
        LabelHighScoreTitel.Text = LanguageManager.GetText("Highscore");
        StartButton.Text = LanguageManager.GetText("StartGame");
        ResumeButton.Text = LanguageManager.GetText("ResumeGame");
        ToolTipProperties.SetText(HelpButton, LanguageManager.GetText("TooltipTutorial"));
        ToolTipProperties.SetText(ShopButton, LanguageManager.GetText("TooltipShop"));
        ToolTipProperties.SetText(LanguageButton, LanguageManager.GetText("TooltipLanguage"));
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
        
        VisualStateManager.GoToState(HighscoreLabel, orientation);
        VisualStateManager.GoToState(CurrentScoreLabel, orientation);
        VisualStateManager.GoToState(LabelCurrentScoreTitel, orientation);
        VisualStateManager.GoToState(LabelHighScoreTitel, orientation);
        VisualStateManager.GoToState(StackLayoutOrientation1, orientation);
        VisualStateManager.GoToState(StackLayoutOrientation2, orientation);
        VisualStateManager.GoToState(Titel, orientation);
        VisualStateManager.GoToState(MainStackLayout, orientation);
        VisualStateManager.GoToState(DifficultyFrame, orientation);

    }



    private void UpdateTileCoins()
    {
        int tileCoins = Preferences.Get("Coins", 0);
        TileCoinsCountLabel.Text = tileCoins.ToString();
    }
    
    private async void Start_OnClicked(object? sender, EventArgs e)
    {
        try
        {
            if (Application.Current?.MainPage?.Navigation != null)
            {
                Preferences.Set("Resume", false);
                await Application.Current.MainPage.Navigation.PushAsync(new Level1View(), true);
            }
        }
        catch (Exception){/*inactive*/}
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

    private async void ShopButton_OnClicked(object? sender, EventArgs e)
    {
        try
        {
            if (Application.Current?.MainPage?.Navigation != null)
                await Application.Current.MainPage.Navigation.PushAsync(new Shop(), true);
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
        SetLanguage(); // <-- Texte neu laden!
    }

    private void LogoButton_OnClicked(object? sender, EventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "http://github.com/kallioderso/FindTheTiles",
            UseShellExecute = true
        });
    }

    private async void Resume_OnClicked(object? sender, EventArgs e)
    {
        try
        {
            if (Application.Current?.MainPage?.Navigation != null)
                await Application.Current.MainPage.Navigation.PushAsync(new Level1View(), true);
        }
        catch (Exception){/*inactive*/}
    }
}