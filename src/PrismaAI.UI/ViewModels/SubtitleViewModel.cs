using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrismaAI.Core.Models;
using PrismaAI.UI.Services;
using System.Collections.ObjectModel;

namespace PrismaAI.UI.ViewModels;

public partial class SubtitleViewModel : ObservableObject
{
    private readonly ISubtitleService _subtitleService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private string _currentSubtitle = "等待输入...";

    [ObservableProperty]
    private string _statusText = "未开始";

    [ObservableProperty]
    private Color _statusColor = Colors.Gray;

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private bool _isNotRunning = true;

    [ObservableProperty]
    private string _sourceLanguage = "中文";

    [ObservableProperty]
    private string _targetLanguage = "English";

    public ObservableCollection<SubtitleItem> SubtitleHistory { get; } = new();

    public SubtitleViewModel(ISubtitleService subtitleService, ISettingsService settingsService)
    {
        _subtitleService = subtitleService;
        _settingsService = settingsService;

        _subtitleService.SubtitleReceived += OnSubtitleReceived;
        _subtitleService.StateChanged += OnStateChanged;

        LoadSettings();
    }

    partial void OnIsRunningChanged(bool value)
    {
        IsNotRunning = !value;
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
                    SourceType = AudioSourceType.Microphone,
                    SampleRate = 16000,
                    Channels = 1
                },
                AsrModel = new ModelConfig
                {
                    ModelName = "whisper-large-v3-turbo",
                    EngineType = Preferences.Default.Get("use_cloud_api", false)
                        ? InferenceEngineType.CloudAPI
                        : InferenceEngineType.LocalOnnx,
                    CloudEndpoint = Preferences.Default.Get("cloud_endpoint", "https://api.openai.com/v1"),
                    ApiKey = Preferences.Default.Get("api_key", "")
                },
                SourceLanguage = MapLanguageCode(SourceLanguage),
                TargetLanguage = MapLanguageCode(TargetLanguage),
                EnableTranslation = Preferences.Default.Get("enable_translation", true)
            };

            await _subtitleService.StartAsync(config);
            IsRunning = true;
        }
        catch (Exception ex)
        {
            CurrentSubtitle = $"错误: {ex.Message}";
            StatusColor = Colors.Red;
        }
    }

    [RelayCommand]
    private async Task PauseAsync()
    {
        await _subtitleService.StopAsync();
        IsRunning = false;
        StatusText = "已暂停";
        StatusColor = Colors.Orange;
    }

    [RelayCommand]
    private void Clear()
    {
        SubtitleHistory.Clear();
        CurrentSubtitle = "等待输入...";
    }

    private void OnSubtitleReceived(object? sender, SubtitleEventArgs e)
    {
       MainThread.BeginInvokeOnMainThread(() =>
        {
            CurrentSubtitle = e.IsTranslation
                ? $"[{e.TargetLanguage}] {e.Text}"
                : e.Text;

            if (e.IsFinal)
            {
                SubtitleHistory.Insert(0, new SubtitleItem
                {
                    Text = CurrentSubtitle,
                    Timestamp = DateTime.Now,
                    IsTranslation = e.IsTranslation
                });

                // 限制历史记录数量
                if (SubtitleHistory.Count > 100)
                {
                    SubtitleHistory.RemoveAt(SubtitleHistory.Count - 1);
                }
            }
        });
    }

    private void OnStateChanged(object? sender, string state)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            StatusText = state;
            StatusColor = state switch
            {
                "运行中" => Colors.Green,
                "已暂停" => Colors.Orange,
                "错误" => Colors.Red,
                _ => Colors.Gray
            };
        });
    }

    private void LoadSettings()
    {
        SourceLanguage = Preferences.Default.Get("source_language", "中文");
        TargetLanguage = Preferences.Default.Get("target_language", "English");
    }

    private static string MapLanguageCode(string language)
    {
        return language switch
        {
            "中文" => "zh",
            "English" => "en",
            "日本語" => "ja",
            "한국어" => "ko",
            _ => "auto"
        };
    }
}

/// <summary>
/// 字幕项
/// </summary>
public class SubtitleItem
{
    public required string Text { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsTranslation { get; set; }
}
