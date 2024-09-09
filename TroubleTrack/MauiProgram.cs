using Blazored.LocalStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using MudBlazor.Services;
using System.Reflection;
using TroubleTrack.Services;

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
            var serviceProvider = builder.Services.BuildServiceProvider();
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            ConfigureAppSettings(builder);

            builder.Services.AddKernel();
            builder.Services.AddAzureOpenAIChatCompletion(
                     deploymentName: config["OpenAI:Deployment"]!,
                     endpoint: config["OpenAI:Endpoint"]!,
                     apiKey: config["OpenAI:Key"]!);

            builder.Services.AddMudServices();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<UserPreferencesService>();
            builder.Services.AddScoped<LayoutService>();
            builder.Services.AddScoped<AppStateService>();
            builder.Services.AddSingleton<StepRecorderService>();
            builder.Services.AddSingleton<ImageAssistantService>();
            builder.Services.AddMemoryCache();
            
            return builder.Build();
        }

        private static void ConfigureAppSettings(MauiAppBuilder builder)
        {
            string configFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.appsettings.json";
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(configFileName);
            builder.Configuration.AddJsonStream(stream!);
        }
    }
}