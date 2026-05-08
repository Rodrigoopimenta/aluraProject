namespace AppVideo.Pages;

public partial class FootballPage : ContentPage
{
    public FootballPage()
    {
        InitializeComponent();
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
