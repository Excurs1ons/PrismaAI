using PrismaAI.Core.Audio;
using PrismaAI.Core.Inference.Engines;
using PrismaAI.Core.Models;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace PrismaAI.Core.Pipeline;

/// <summary>
/// 字幕生成管道
/// </summary>
public interface ISubtitlePipeline : IDisposable
{
    /// <summary>字幕输出流</summary>
    IObservable<TranscriptionResult> Subtitles { get; }

    /// <summary>翻译输出流</summary>
    IObservable<TranslationResult>? Translations { get; }

    /// <summary>处理状态</summary>
    PipelineState State { get; }

    /// <summary>开始处理</summary>
    Task StartAsync(CancellationToken ct = default);

    /// <summary>停止处理</summary>
    Task StopAsync(CancellationToken ct = default);

    /// <summary>更新配置</summary>
    void UpdateConfig(PipelineConfig config);
}

/// <summary>
/// 管道状态
/// </summary>
public enum PipelineState
{
    Idle,
    Starting,
    Running,
    Stopping,
    Error
}

/// <summary>
/// 管道配置
/// </summary>
public record PipelineConfig
{
    public AudioCaptureConfig AudioConfig { get; init; } = new();
    public ModelConfig AsrModel { get; init; } = new();
    public ModelConfig? TranslationModel { get; init; }
    public ModelConfig? TTSModel { get; init; }
    public string SourceLanguage { get; init; } = "auto";
    public string? TargetLanguage { get; init; }
    public bool EnableTranslation { get; init; }
    public bool EnableTTS { get; init; }
    public bool EnableWordTimestamps { get; init; } = true;
}

/// <summary>
/// 字幕生成管道实现
/// </summary>
public class SubtitlePipeline : ISubtitlePipeline
{
    private readonly IAudioCaptureFactory _audioFactory;
    private readonly IInferenceEngineFactory _inferenceFactory;
    private readonly BehaviorSubject<PipelineState> _state = new(PipelineState.Idle);
    private readonly Subject<TranscriptionResult> _subtitles = new();
    private readonly Subject<TranslationResult>? _translations;
    private readonly Subject<Unit> _stopSignal = new();

    private IAudioCapture? _audioCapture;
    private IWhisperModel? _asrModel;
    private ITranslationModel? _translationModel;
    private ITTSModel? _ttsModel;
    private CancellationTokenSource? _processingCts;
    private PipelineConfig _config = new();

    public IObservable<TranscriptionResult> Subtitles => _subtitles.AsObservable();
    public IObservable<TranslationResult>? Translations => _translations?.AsObservable();
    public PipelineState State => _state.Value;

    public SubtitlePipeline(
        IAudioCaptureFactory audioFactory,
        IInferenceEngineFactory inferenceFactory)
    {
        _audioFactory = audioFactory;
        _inferenceFactory = inferenceFactory;
        _translations = new Subject<TranslationResult>();
    }

    public void UpdateConfig(PipelineConfig config)
    {
        _config = config;
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        if (_state.Value != PipelineState.Idle)
            throw new InvalidOperationException($"Cannot start from state: {_state.Value}");

        _state.OnNext(PipelineState.Starting);
        _processingCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

        try
        {
            // 1. 加载模型
            await LoadModelsAsync(_processingCts.Token);

            // 2. 初始化音频捕获
            _audioCapture = _audioFactory.Create(_config.AudioConfig);

            // 3. 启动处理循环
            _state.OnNext(PipelineState.Running);

            _ = Task.Run(() => ProcessingLoop(_processingCts.Token), _processingCts.Token);

            await _audioCapture.StartAsync(ct);
        }
        catch (Exception ex)
        {
            _state.OnNext(PipelineState.Error);
            _subtitles.OnError(ex);
            throw;
        }
    }

    public async Task StopAsync(CancellationToken ct = default)
    {
        if (_state.Value != PipelineState.Running)
            return;

        _state.OnNext(PipelineState.Stopping);
        _stopSignal.OnNext(Unit.Default);

        try
        {
            if (_audioCapture != null)
                await _audioCapture.StopAsync(ct);

            _processingCts?.Cancel();

            _state.OnNext(PipelineState.Idle);
        }
        catch (Exception ex)
        {
            _state.OnNext(PipelineState.Error);
            throw;
        }
    }

    private async Task LoadModelsAsync(CancellationToken ct)
    {
        // 根据 EngineType 创建模型
        _asrModel = await _inferenceFactory.CreateWhisperModelAsync(_config.AsrModel, ct);

        if (_config.EnableTranslation && _config.TranslationModel != null)
        {
            _translationModel = await _inferenceFactory.CreateTranslationModelAsync(_config.TranslationModel, ct);
        }

        if (_config.EnableTTS && _config.TTSModel != null)
        {
            _ttsModel = await _inferenceFactory.CreateTTSModelAsync(_config.TTSModel, ct);
        }
    }

    private async Task ProcessingLoop(CancellationToken ct)
    {
        if (_audioCapture == null || _asrModel == null)
            return;

        try
        {
            var audioBuffer = new List<float>();
            var bufferSize = _config.AsrModel.MaxTokens * 16000 / 50; // 近似缓冲区大小

            await foreach (var chunk in _audioCapture.AudioStream.ToAsyncEnumerable().WithCancellation(ct))
            {
                if (_state.Value != PipelineState.Running)
                    break;

                audioBuffer.AddRange(chunk.Samples);

                if (audioBuffer.Count >= bufferSize)
                {
                    // 处理音频
                    var audio = audioBuffer.ToArray();
                    audioBuffer.Clear();

                    // ASR
                    if (_asrModel.EngineType == InferenceEngineType.CloudAPI)
                    {
                        // 云端 API 模式
                        var result = await _asrModel.TranscribeAsync(
                            audio,
                            chunk.Format.SampleRate,
                            _config.SourceLanguage == "auto" ? null : _config.SourceLanguage,
                            ct);

                        _subtitles.OnNext(result);

                        // 翻译
                        if (_config.EnableTranslation && _translationModel != null)
                        {
                            var translation = await _translationModel.TranslateAsync(
                                result.Text,
                                result.Language ?? _config.SourceLanguage,
                                _config.TargetLanguage ?? "en",
                                ct);

                            _translations?.OnNext(translation);
                        }
                    }
                    else
                    {
                        // 本地模型模式 - 使用流式 API
                        await foreach (var result in _asrModel.StreamTranscribeAsync(
                            GetAsyncEnumerable(new[] { audio }),
                            chunk.Format.SampleRate,
                            _config.SourceLanguage == "auto" ? null : _config.SourceLanguage,
                            ct))
                        {
                            _subtitles.OnNext(result);
                        }
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // 正常取消
        }
        catch (Exception ex)
        {
            _state.OnNext(PipelineState.Error);
            _subtitles.OnError(ex);
        }
    }

    private static async IAsyncEnumerable<float[]> GetAsyncEnumerable(IEnumerable<float[]> chunks)
    {
        foreach (var chunk in chunks)
        {
            await Task.Yield();
            yield return chunk;
        }
    }

    public void Dispose()
    {
        _audioCapture?.Dispose();
        _asrModel?.Dispose();
        _translationModel?.Dispose();
        _ttsModel?.Dispose();
        _processingCts?.Dispose();
        _state.OnCompleted();
        _subtitles.OnCompleted();
        _translations?.OnCompleted();
        _stopSignal.OnCompleted();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// 推理引擎工厂
/// </summary>
public interface IInferenceEngineFactory
{
    Task<IWhisperModel> CreateWhisperModelAsync(ModelConfig config, CancellationToken ct = default);
    Task<ITranslationModel> CreateTranslationModelAsync(ModelConfig config, CancellationToken ct = default);
    Task<ITTSModel> CreateTTSModelAsync(ModelConfig config, CancellationToken ct = default);
}
