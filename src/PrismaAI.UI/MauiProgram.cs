using Microsoft.Extensions.Logging;
using PrismaAI.Core.Models;
using PrismaAI.Core.Services;
using PrismaAI.UI.Services;
using PrismaAI.UI.ViewModels;

namespace PrismaAI.UI;

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
                fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
                fonts.AddFont("NotoSansSC-Regular.ttf", "NotoSansSC");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // 注册服务
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<SubtitlePage>();

        // ViewModels
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<SubtitleViewModel>();

        // Core Services
        builder.Services.AddSingleton<ISubtitleService, SubtitleService>();
        builder.Services.AddSingleton<IAudioCaptureService, AudioCaptureService>();
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<IModelService, ModelService>();

        // AI Services
        builder.Services.AddSingleton<ICloudInferenceService, CloudInferenceService>();

        return builder.Build();
    }
}
