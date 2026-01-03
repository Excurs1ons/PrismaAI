namespace PrismaAI.UI.Services;

/// <summary>
/// 设置服务接口
/// </summary>
public interface ISettingsService
{
    Task<string> GetCloudEndpointAsync();
    Task<string> GetApiKeyAsync();
    Task<string> GetApiProviderAsync();
    Task<bool> GetUseCloudAPIAsync();
    Task<string> GetModelPathAsync();
    Task SaveAllAsync(Dictionary<string, object> settings);
}

/// <summary>
/// 音频捕获服务接口
/// </summary>
public interface IAudioCaptureService
{
    Task<bool> RequestPermissionAsync();
    Task<bool> HasPermissionAsync();
    Task<string?> SelectAudioFileAsync();
}

/// <summary>
/// 模型服务接口
/// </summary>
public interface IModelService
{
    Task<bool> DownloadModelAsync(string modelName, CancellationToken ct = default);
    Task<bool> IsModelDownloadedAsync(string modelName);
    Task<string> GetModelPathAsync(string modelName);
    Task<long> GetModelSizeAsync(string modelName);
    Task<Dictionary<string, string>> GetAvailableModelsAsync();
}

/// <summary>
/// 设置服务实现
/// </summary>
public class SettingsService : ISettingsService
{
    private const string CloudEndpointKey = "cloud_endpoint";
    private const string ApiKeyKey = "api_key";
    private const string ApiProviderKey = "api_provider";
    private const string UseCloudAPIKey = "use_cloud_api";
    private const string ModelPathKey = "model_path";

    public Task<string> GetCloudEndpointAsync()
    {
        return Task.FromResult(Preferences.Default.Get(CloudEndpointKey, "https://api.openai.com/v1"));
    }

    public Task<string> GetApiKeyAsync()
    {
        return Task.FromResult(Preferences.Default.Get(ApiKeyKey, ""));
    }

    public Task<string> GetApiProviderAsync()
    {
        return Task.FromResult(Preferences.Default.Get(ApiProviderKey, "OpenAI"));
    }

    public Task<bool> GetUseCloudAPIAsync()
    {
        return Task.FromResult(Preferences.Default.Get(UseCloudAPIKey, false));
    }

    public Task<string> GetModelPathAsync()
    {
        return Task.FromResult(Preferences.Default.Get(ModelPathKey,
            FileSystem.Current.AppDataDirectory + "/models"));
    }

    public async Task SaveAllAsync(Dictionary<string, object> settings)
    {
        foreach (var setting in settings)
        {
            switch (setting.Value)
            {
                case bool boolValue:
                    Preferences.Default.Set(setting.Key, boolValue);
                    break;
                case string stringValue:
                    Preferences.Default.Set(setting.Key, stringValue);
                    break;
                case int intValue:
                    Preferences.Default.Set(setting.Key, intValue);
                    break;
                case double doubleValue:
                    Preferences.Default.Set(setting.Key, doubleValue);
                    break;
                case float floatValue:
                    Preferences.Default.Set(setting.Key, floatValue);
                    break;
                case long longValue:
                    Preferences.Default.Set(setting.Key, longValue);
                    break;
            }
        }
        await Task.CompletedTask;
    }
}

/// <summary>
/// 音频捕获服务实现
/// </summary>
public class AudioCaptureService : IAudioCaptureService
{
    public async Task<bool> RequestPermissionAsync()
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.Microphone>();
            return status == PermissionStatus.Granted;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> HasPermissionAsync()
        => await Permissions.CheckStatusAsync<Permissions.Microphone>() == PermissionStatus.Granted;

    public async Task<string?> SelectAudioFileAsync()
    {
        try
        {
            var options = new PickOptions
            {
                PickerTitle = "选择音频文件",
                FileTypes = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.audio" } },
                        { DevicePlatform.Android, new[] { "audio/*" } },
                        { DevicePlatform.WinUI, new[] { ".wav", ".mp3", ".m4a", ".flac" } },
                        { DevicePlatform.MacCatalyst, new[] { "wav", "mp3", "m4a", "flac" } }
                    })
            };

            var result = await FilePicker.Default.PickAsync(options);
            return result?.FullPath;
        }
        catch
        {
            return null;
        }
    }
}

/// <summary>
/// 模型服务实现
/// </summary>
public class ModelService : IModelService
{
    private readonly string _modelsDirectory;

    public ModelService()
    {
        _modelsDirectory = Path.Combine(FileSystem.Current.AppDataDirectory, "models");
        Directory.CreateDirectory(_modelsDirectory);
    }

    public async Task<bool> DownloadModelAsync(string modelName, CancellationToken ct = default)
    {
        // 实现模型下载逻辑
        // 可以从 Hugging Face 或其他源下载 GGUF/ONNX 模型
        await Task.Delay(100, ct);
        return true;
    }

    public Task<bool> IsModelDownloadedAsync(string modelName)
    {
        var modelPath = Path.Combine(_modelsDirectory, modelName);
        return Task.FromResult(File.Exists(modelPath));
    }

    public Task<string> GetModelPathAsync(string modelName)
    {
        return Task.FromResult(Path.Combine(_modelsDirectory, modelName));
    }

    public Task<long> GetModelSizeAsync(string modelName)
    {
        var modelPath = Path.Combine(_modelsDirectory, modelName);
        if (File.Exists(modelPath))
        {
            var info = new FileInfo(modelPath);
            return Task.FromResult(info.Length);
        }
        return Task.FromResult<long>(0);
    }

    public Task<Dictionary<string, string>> GetAvailableModelsAsync()
    {
        return Task.FromResult(new Dictionary<string, string>
        {
            { "whisper-large-v3-turbo", "Whisper Large V3 Turbo - 最快的速度" },
            { "whisper-large-v3", "Whisper Large V3 - 最高精度" },
            { "distil-whisper", "Distil Whisper - 轻量级" },
            { "nllb-200-distilled-600M", "NLLB 200 - 多语言翻译" },
            { "seamless-m4t-v2", "SeamlessM4T V2 - 多模态翻译" }
        });
    }
}

/// <summary>
/// 字幕服务实现
/// </summary>
public class SubtitleService : ISubtitleService
{
    private PrismaAI.Core.Pipeline.ISubtitlePipeline? _pipeline;

    public event EventHandler<SubtitleEventArgs>? SubtitleReceived;
    public event EventHandler<string>? StateChanged;

    public string CurrentState { get; private set; } = "未开始";

    public async Task StartAsync(PrismaAI.Core.Models.PipelineConfig config)
    {
        // TODO: 初始化并启动管道
        // 这里需要实现实际的管道创建和启动逻辑

        CurrentState = "运行中";
        StateChanged?.Invoke(this, CurrentState);

        await Task.Delay(100);

        // 模拟字幕输出
        await Task.Run(async () =>
        {
            await Task.Delay(1000);
            SubtitleReceived?.Invoke(this, new SubtitleEventArgs
            {
                Text = "测试字幕输出",
                IsFinal = true,
                Confidence = 0.95f,
                StartTime = TimeSpan.Zero,
                EndTime = TimeSpan.FromSeconds(2)
            });
        });
    }

    public async Task StopAsync()
    {
        if (_pipeline != null)
        {
            await _pipeline.StopAsync();
            _pipeline.Dispose();
            _pipeline = null;
        }
        CurrentState = "已停止";
        StateChanged?.Invoke(this, CurrentState);
    }
}
