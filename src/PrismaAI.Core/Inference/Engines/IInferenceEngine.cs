using PrismaAI.Core.Models;

namespace PrismaAI.Core.Inference.Engines;

/// <summary>
/// AI 推理引擎接口
/// </summary>
public interface IInferenceEngine : IDisposable
{
    /// <summary>引擎类型</summary>
    InferenceEngineType EngineType { get; }

    /// <summary>是否支持 GPU</summary>
    bool SupportsGPU { get; }

    /// <summary>当前是否使用 GPU</summary>
    bool IsUsingGPU { get; }

    /// <summary>加载模型</summary>
    Task LoadModelAsync(string modelPath, CancellationToken ct = default);

    /// <summary>卸载模型</summary>
    Task UnloadModelAsync(CancellationToken ct = default);

    /// <summary>模型是否已加载</summary>
    bool IsModelLoaded { get; }
}

/// <summary>
/// Whisper 模型接口
/// </summary>
public interface IWhisperModel : IInferenceEngine
{
    /// <summary>语音识别</summary>
    Task<TranscriptionResult> TranscribeAsync(
        float[] audioSamples,
        int sampleRate,
        string? language = null,
        CancellationToken ct = default);

    /// <summary>流式语音识别 (带 VAD)</summary>
    IAsyncEnumerable<TranscriptionResult> StreamTranscribeAsync(
        IAsyncEnumerable<float[]> audioChunks,
        int sampleRate,
        string? language = null,
        CancellationToken ct = default);

    /// <summary>获取支持的语言列表</summary>
    IReadOnlyList<string> SupportedLanguages { get; }

    /// <summary>检测语言</summary>
    Task<string> DetectLanguageAsync(float[] audioSamples, int sampleRate, CancellationToken ct = default);
}

/// <summary>
/// 翻译模型接口
/// </summary>
public interface ITranslationModel : IInferenceEngine
{
    /// <summary>翻译文本</summary>
    Task<TranslationResult> TranslateAsync(
        string text,
        string sourceLanguage,
        string targetLanguage,
        CancellationToken ct = default);

    /// <summary>批量翻译</summary>
    Task<TranslationResult[]> TranslateBatchAsync(
        string[] texts,
        string sourceLanguage,
        string targetLanguage,
        CancellationToken ct = default);

    /// <summary>获取支持的语言列表</summary>
    IReadOnlyList<string> SupportedLanguages { get; }

    /// <summary>是否支持自动检测源语言</summary>
    bool SupportsAutoDetection { get; }
}

/// <summary>
/// TTS 模型接口
/// </summary>
public interface ITTSModel : IInferenceEngine
{
    /// <summary>语音合成</summary>
    Task<SynthesisResult> SynthesizeAsync(
        string text,
        string? voice = null,
        float speed = 1.0f,
        CancellationToken ct = default);

    /// <summary>获取可用的语音列表</summary>
    IReadOnlyList<TTSVoice> AvailableVoices { get; }

    /// <summary>设置默认语音</summary>
    void SetDefaultVoice(string voiceId);
}

/// <summary>
/// TTS 语音信息
/// </summary>
public record TTSVoice(
    string Id,
    string Name,
    string Language,
    TTSVoiceGender Gender = TTSVoiceGender.Unknown,
    string? Description = null
)
{
    public string DisplayName => $"{Name} ({Language})";
}

/// <summary>
/// 语音性别
/// </summary>
public enum TTSVoiceGender
{
    Unknown,
    Male,
    Female,
    Neutral
}
