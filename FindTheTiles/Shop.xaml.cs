namespace FindTheTiles;

public partial class Shop
{
    public Shop()
    {
        InitializeComponent();
        NavigationPage.SetHasBackButton(this, false);
    }

    private void ExitButton_Clicked(object? sender, EventArgs e) { Navigation.PopAsync(); }
}