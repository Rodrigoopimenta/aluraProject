namespace AppVideo.Pages;

public partial class BasketballPage : ContentPage
{
    public BasketballPage()
    {
        InitializeComponent();
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
