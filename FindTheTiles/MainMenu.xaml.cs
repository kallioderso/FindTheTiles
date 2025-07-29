using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace FindTheTiles;

public partial class MainMenu : ContentPage
{
    public MainMenu()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        int highscore = Preferences.Get("Highscore", 0);
        int lastScore = Preferences.Get("LastScore", 0);
        HighscoreLabel.Text = highscore.ToString();
        CurrentScoreLabel.Text = lastScore.ToString();
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        Navigation.PushAsync(new GameView());
    }
}