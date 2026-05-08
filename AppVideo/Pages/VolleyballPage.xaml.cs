namespace AppVideo.Pages;

public partial class VolleyballPage : ContentPage
{
    public VolleyballPage()
    {
        InitializeComponent();
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
