using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using TroubleTrack.Services.Exploration;
using TroubleTrack.Services.Installer;
using TroubleTrack.Services.Presentation;
using TroubleTrack.Services.Utilities;

namespace TroubleTrack
{
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
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            builder.Services.AddMudServices();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<UserPreferencesService>();
            builder.Services.AddScoped<LayoutService>();
            builder.Services.AddScoped<AppExploreService>();
            builder.Services.AddScoped<AppStateService>();
            builder.Services.AddScoped<DotnetExplorerService>();
            builder.Services.AddSingleton<StepRecorderService>();
            builder.Services.AddMemoryCache();
            return builder.Build();
        }
    }
}