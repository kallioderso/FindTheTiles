using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace FindTheTiles;

public partial class MainMenu : ContentPage
{
    public MainMenu()
    {
        InitializeComponent();
        if(Preferences.Get("FirstStart", true))
            Application.Current.MainPage.Navigation.PushAsync(new Tutorial(), true);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        int highscore = Preferences.Get("Highscore", 0);
        int lastScore = Preferences.Get("LastScore", 0);
        HighscoreLabel.Text = highscore.ToString();
        CurrentScoreLabel.Text = lastScore.ToString();
        UpdateTileCoins();
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
    }



    private void UpdateTileCoins()
    {
        int tileCoins = Preferences.Get("XP", 0);
        TileCoinsCountLabel.Text = tileCoins.ToString();
    }
    
    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new GameView(), true);
    }

    private void HelpButton_OnClicked(object? sender, EventArgs e)
    {
    }

    private void ShopButton_OnClicked(object? sender, EventArgs e)
    {
    }
}