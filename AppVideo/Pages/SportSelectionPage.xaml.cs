namespace AppVideo.Pages;

public partial class SportSelectionPage : ContentPage
{
    public SportSelectionPage()
    {
        InitializeComponent();
    }

    private async void OnBasketballClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new BasketballPage());
    }

    private async void OnVolleyballClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new VolleyballPage());
    }

    private async void OnFootballClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new FootballPage());
    }
}
