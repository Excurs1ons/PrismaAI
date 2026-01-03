using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrismaAI.Core.Models;
using PrismaAI.UI.Services;
using System.Collections.ObjectModel;

namespace PrismaAI.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ISubtitleService _subtitleService;
    private readonly ISettingsService _settingsService;
    private readonly IAudioCaptureService _audioService;

    [ObservableProperty]
    private string _statusText = "就绪";

    [ObservableProperty]
    private Color _statusColor = Colors.Green;

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private bool _isNotRunning = true;

    [ObservableProperty]
    private AudioSourceType _selectedAudioSourceType = AudioSourceType.Microphone;

    [ObservableProperty]
    private string _selectedSourceLanguage = "中文";

    [ObservableProperty]
    private string _selectedTargetLanguage = "English";

    [ObservableProperty]
    private bool _useCloudAPI;

    [ObservableProperty]
    private bool _enableTranslation = true;

    [ObservableProperty]
    private bool _enableTTS;

    public ObservableCollection<string> AudioSourceTypes { get; } = new()
    {
        "麦克风输入",
        "系统音频",
        "音频文件"
    };

    public ObservableCollection<string> SourceLanguages { get; } = new()
    {
        "自动检测",
        "中文",
        "English",
        "日本語",
        "한국어",
        "Español",
        "Français"
    };

    public ObservableCollection<string> TargetLanguages { get; } = new()
    {
        "English",
        "中文",
        "日本語",
        "한국어",
        "Español",
        "Français",
        "Deutsch"
    };

    public MainViewModel(
        ISubtitleService subtitleService,
        ISettingsService settingsService,
        IAudioCaptureService audioService)
    {
        _subtitleService = subtitleService;
        _settingsService = settingsService;
        _audioService = audioService;

        // 加载设置
        LoadSettings();

        // 订阅状态变化
        _subtitleService.StateChanged += OnStateChanged;
    }

    partial void OnIsRunningChanged(bool value)
    {
        IsNotRunning = !value;
        StatusText = value ? "运行中" : "已停止";
        StatusColor = value ? Colors.Green : Colors.Orange;
    }

    partial void OnSelectedAudioSourceTypeChanging(string value)
    {
        if (value == "麦克风输入")
            SelectedAudioSourceType = AudioSourceType.Microphone;
        else if (value == "系统音频")
            SelectedAudioSourceType = AudioSourceType.SystemAudio;
        else
            SelectedAudioSourceType = AudioSourceType.File;
    }

    [RelayCommand]
    private async Task StartAsync()
    {
        try
        {
            var config = new PipelineConfig
            {
                AudioConfig = new AudioCaptureConfig
                {
                    SourceType = SelectedAudioSourceType,
                    SampleRate = 16000,
                    Channels = 1
                },
                AsrModel = new ModelConfig
                {
                    ModelName = "whisper-large-v3-turbo",
                    EngineType = UseCloudAPI ? InferenceEngineType.CloudAPI : InferenceEngineType.LocalOnnx,
                    CloudEndpoint = await _settingsService.GetCloudEndpointAsync(),
                    ApiKey = await _settingsService.GetApiKeyAsync()
                },
                SourceLanguage = MapLanguageCode(SelectedSourceLanguage),
                TargetLanguage = MapLanguageCode(SelectedTargetLanguage),
                EnableTranslation = EnableTranslation,
                EnableTTS = EnableTTS
            };

            await _subtitleService.StartAsync(config);
            IsRunning = true;
        }
        catch (Exception ex)
        {
            StatusText = $"错误: {ex.Message}";
            StatusColor = Colors.Red;
        }
    }

    [RelayCommand]
    private async Task StopAsync()
    {
        await _subtitleService.StopAsync();
        IsRunning = false;
    }

    [RelayCommand]
    private async Task GoToSubtitleAsync()
    {
        await Shell.Current.GoToAsync("//subtitle");
    }

    [RelayCommand]
    private async Task GoToSettingsAsync()
    {
        await Shell.Current.GoToAsync("//settings");
    }

    public void OnAppearing()
    {
        // 刷新设置
        LoadSettings();
    }

    private void LoadSettings()
    {
        UseCloudAPI = Preferences.Default.Get("use_cloud_api", false);
        EnableTranslation = Preferences.Default.Get("enable_translation", true);
        EnableTTS = Preferences.Default.Get("enable_tts", false);
    }

    private void OnStateChanged(object? sender, string state)
    {
        StatusText = state;
    }

    private static string MapLanguageCode(string language)
    {
        return language switch
        {
            "自动检测" => "auto",
            "中文" => "zh",
            "English" => "en",
            "日本語" => "ja",
            "한국어" => "ko",
            "Español" => "es",
            "Français" => "fr",
            "Deutsch" => "de",
            _ => "auto"
        };
    }
}
