namespace PrismaAI.Core.Models;

/// <summary>
/// 语音识别结果
/// </summary>
public record TranscriptionResult
{
    public required string Text { get; init; }
    public string? Language { get; init; }
    public float Confidence { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public bool IsFinal { get; init; }
    public List<WordTimestamp>? Words { get; init; }
}

/// <summary>
/// 单词级时间戳
/// </summary>
public record WordTimestamp(string Word, TimeSpan StartTime, TimeSpan EndTime, float Probability);

/// <summary>
/// 翻译结果
/// </summary>
public record TranslationResult
{
    public required string SourceText { get; init; }
    public required string TranslatedText { get; init; }
    public required string SourceLanguage { get; init; }
    public required string TargetLanguage { get; init; }
    public float Confidence { get; init; }
}

/// <summary>
/// TTS 合成结果
/// </summary>
public record SynthesisResult
{
    public required byte[] AudioData { get; init; }
    public required string Format { get; init; } // "wav", "mp3", "pcm"
    public int SampleRate { get; init; }
    public TimeSpan Duration { get; init; }
}

/// <summary>
/// 推理引擎类型
/// </summary>
public enum InferenceEngineType
{
    /// <summary>本地 ONNX Runtime</summary>
    LocalOnnx,
    /// <summary>本地 GGUF (llama.cpp)</summary>
    LocalGGUF,
    /// <summary>云端 API</summary>
    CloudAPI
}

/// <summary>
/// 模型配置
/// </summary>
public record ModelConfig
{
    public required string ModelName { get; init; }
    public required InferenceEngineType EngineType { get; init; }
    public string? ModelPath { get; init; }
    public string? CloudEndpoint { get; init; }
    public string? ApiKey { get; init; }
    public int MaxTokens { get; init; } = 256;
    public float Temperature { get; init; } = 0.0f;
    public bool UseGPU { get; init; } = false;
}

/// <summary>
/// 支持的语言列表
/// </summary>
public static class SupportedLanguages
{
    public static readonly Dictionary<string, string> WhisperLanguages = new()
    {
        { "zh", "中文" },
        { "en", "English" },
        { "yue", "粤语" },
        { "ja", "日本語" },
        { "ko", "한국어" },
        { "es", "Español" },
        { "fr", "Français" },
        { "de", "Deutsch" },
        { "it", "Italiano" },
        { "pt", "Português" },
        { "ru", "Русский" },
        { "ar", "العربية" },
        { "hi", "हिन्दी" },
        { "th", "ไทย" },
        { "vi", "Tiếng Việt" }
    };

    public static readonly Dictionary<string, string> TranslationLanguages = new()
    {
        { "zh-CN", "中文 (简体)" },
        { "zh-TW", "中文 (繁體)" },
        { "en", "English" },
        { "ja", "日本語" },
        { "ko", "한국어" },
        { "es", "Español" },
        { "fr", "Français" },
        { "de", "Deutsch" },
        { "ru", "Русский" },
        { "ar", "العربية" },
        { "pt", "Português" },
        { "it", "Italiano" }
    };
}
