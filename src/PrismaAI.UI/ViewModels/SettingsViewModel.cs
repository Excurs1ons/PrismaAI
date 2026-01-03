using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrismaAI.UI.Services;
using System.Collections.ObjectModel;

namespace PrismaAI.UI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private bool _useCloudAPI;

    [ObservableProperty]
    private string _cloudEndpoint = "https://api.openai.com/v1";

    [ObservableProperty]
    private string _apiKey = "";

    [ObservableProperty]
    private string _selectedApiProvider = "OpenAI";

    [ObservableProperty]
    private string _selectedWhisperModel = "whisper-large-v3-turbo";

    [ObservableProperty]
    private string _selectedTranslationModel = "nllb-200-distilled-600M";

    [ObservableProperty]
    private string _modelPath = "";

    [ObservableProperty]
    private bool _enableTranslation = true;

    [ObservableProperty]
    private bool _enableTTS;

    [ObservableProperty]
    private bool _enableWordTimestamps = true;

    [ObservableProperty]
    private bool _autoSave = true;

    [ObservableProperty]
    private string _selectedSampleRate = "16000";

    [ObservableProperty]
    private double _bufferSize = 1000;

    public ObservableCollection<string> ApiProviders { get; } = new()
    {
        "OpenAI",
        "Groq",
        "DeepSeek",
        "自定义"
    };

    public ObservableCollection<string> WhisperModels { get; } = new()
    {
        "whisper-large-v3-turbo (推荐)",
        "whisper-large-v3",
        "whisper-medium",
        "whisper-small",
        "whisper-base",
        "distil-whisper"
    };

    public ObservableCollection<string> TranslationModels { get; } = new()
    {
        "nllb-200-distilled-600M (推荐)",
        "nllb-200-1.3B",
        "seamless-m4t-v2",
        "madlad400"
    };

    public ObservableCollection<string> SampleRates { get; } = new()
    {
        "8000",
        "16000 (推荐)",
        "44100",
        "48000"
    };

    public SettingsViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        LoadSettings();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await _settingsService.SaveAllAsync(new Dictionary<string, object>
        {
            { "use_cloud_api", UseCloudAPI },
            { "cloud_endpoint", CloudEndpoint },
            { "api_key", ApiKey },
            { "api_provider", SelectedApiProvider },
            { "whisper_model", SelectedWhisperModel },
            { "translation_model", SelectedTranslationModel },
            { "model_path", ModelPath },
            { "enable_translation", EnableTranslation },
            { "enable_tts", EnableTTS },
            { "enable_word_timestamps", EnableWordTimestamps },
            { "auto_save", AutoSave },
            { "sample_rate", int.Parse(SelectedSampleRate.Replace(" (推荐)", "")) },
            { "buffer_size", (int)BufferSize }
        });

        await Shell.Current.DisplayAlert("成功", "设置已保存", "确定");
    }

    private void LoadSettings()
    {
        UseCloudAPI = Preferences.Default.Get("use_cloud_api", false);
        CloudEndpoint = Preferences.Default.Get("cloud_endpoint", "https://api.openai.com/v1");
        ApiKey = Preferences.Default.Get("api_key", "");
        SelectedApiProvider = Preferences.Default.Get("api_provider", "OpenAI");
        SelectedWhisperModel = Preferences.Default.Get("whisper_model", "whisper-large-v3-turbo (推荐)");
        SelectedTranslationModel = Preferences.Default.Get("translation_model", "nllb-200-distilled-600M (推荐)");
        ModelPath = Preferences.Default.Get("model_path", "");
        EnableTranslation = Preferences.Default.Get("enable_translation", true);
        EnableTTS = Preferences.Default.Get("enable_tts", false);
        EnableWordTimestamps = Preferences.Default.Get("enable_word_timestamps", true);
        AutoSave = Preferences.Default.Get("auto_save", true);
        var sr = Preferences.Default.Get("sample_rate", 16000);
        SelectedSampleRate = sr == 16000 ? "16000 (推荐)" : sr.ToString();
        BufferSize = Preferences.Default.Get("buffer_size", 1000);
    }
}
