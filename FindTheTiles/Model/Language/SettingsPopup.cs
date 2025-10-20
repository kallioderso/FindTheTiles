using System.Windows.Input;

namespace FindTheTiles.Model;

public class SettingsPopup : Frame
{
    StackLayout _stack = new StackLayout()
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Center
        };  
    ImageButton _germanButton = new ImageButton()
    {
        HeightRequest = 100,
        WidthRequest = 100,
        Source = "german.png",
        CornerRadius = 16,
        BackgroundColor = Color.FromArgb("#F3F7FF"),
        BorderColor = Color.FromArgb("#E0E8FF"),
        BorderWidth = 2,
        Command = new Command<ImageButton>(German)
    };
    ImageButton _frenshButton = new ImageButton()
    {
        HeightRequest = 100,
        WidthRequest = 100,
        Source = "frensh.png",
        CornerRadius = 16,
        BackgroundColor = Color.FromArgb("#F3F7FF"),
        BorderColor = Color.FromArgb("#E0E8FF"),
        BorderWidth = 2,
        Command = new Command<ImageButton>(Frensh)
    };
    ImageButton _englishButton = new ImageButton()
    {
        HeightRequest = 100,
        WidthRequest = 100,
        Source = "english.png",
        CornerRadius = 16,
        BackgroundColor = Color.FromArgb("#F3F7FF"),
        BorderColor = Color.FromArgb("#E0E8FF"),
        BorderWidth = 2,
        Command = new Command<ImageButton>(English)
    };
    public SettingsPopup()
    {
        BackgroundColor = Color.FromArgb("#FFFFFF");
        CornerRadius = 20;
        Padding = new Thickness(22, 14);
        BorderColor = Color.FromArgb("#a997d7");
        Content = _stack;
        _stack.Children.Add(_germanButton);
        _stack.Children.Add(_frenshButton);
        _stack.Children.Add(_englishButton);
    }

    private static void German(ImageButton button)
    {
        Preferences.Set("language", "de"); 
    } 
    private static void Frensh(ImageButton button) 
    {
        Preferences.Set("language", "fr");
        }
    private static void English(ImageButton button) 
    {
        Preferences.Set("language", "en");
    }
}