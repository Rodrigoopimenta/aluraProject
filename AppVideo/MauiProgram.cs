using AppVideo.Application.UseCases;
using AppVideo.Domain.Repositories;
using AppVideo.Infrastructure.Persistence;
using AppVideo.Pages;
using Microsoft.Extensions.Logging;

namespace AppVideo;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<IVideoRepository, InMemoryVideoRepository>();
        builder.Services.AddTransient<GetFeaturedVideosUseCase>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<SportSelectionPage>();
        builder.Services.AddTransient<BasketballPage>();
        builder.Services.AddTransient<VolleyballPage>();
        builder.Services.AddTransient<FootballPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
