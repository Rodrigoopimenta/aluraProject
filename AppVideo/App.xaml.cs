using Microsoft.Extensions.DependencyInjection;
using AppVideo.Pages;

namespace AppVideo;

public partial class App : Microsoft.Maui.Controls.Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
        return new Window(new NavigationPage(loginPage));
    }
}
