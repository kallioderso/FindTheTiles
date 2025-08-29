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
        try
        {
            if (Application.Current?.MainPage?.Navigation != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new GameView(), true);
            }
        }
        catch (Exception)
        {
            //inactive
        }
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

    private async void ShopButton_OnClicked(object? sender, EventArgs e)
    {
        try
        {
            if (Application.Current?.MainPage?.Navigation != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new Shop(), true);
            }
        }
        catch (Exception)
        {
            //inactive
        }
    }
}