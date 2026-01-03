using PrismaAI.Core.Models;
using Refit;
using System.Text.Json.Serialization;

namespace PrismaAI.Core.Services;

/// <summary>
/// 云端 AI 推理服务接口
/// </summary>
public interface ICloudInferenceService
{
    /// <summary>语音识别</summary>
    Task<TranscriptionResult> TranscribeAsync(byte[] audioData, string language = "auto", CancellationToken ct = default);

    /// <summary>翻译</summary>
    Task<TranslationResult> TranslateAsync(string text, string targetLanguage, string sourceLanguage = "auto", CancellationToken ct = default);

    /// <summary>语音合成</summary>
    Task<SynthesisResult> SynthesizeAsync(string text, string voice = "default", CancellationToken ct = default);

    /// <summary>流式语音识别 (Server-Sent Events)</summary>
    IAsyncEnumerable<TranscriptionResult> StreamTranscribeAsync(IAsyncEnumerable<byte[]> audioStream, string language = "auto", CancellationToken ct = default);
}

/// <summary>
/// OpenAI 兼容 API 接口
/// </summary>
public interface IOpenAIAudioApi
{
    [Multipart]
    [Post("/v1/audio/transcriptions")]
    Task<AudioResponse> CreateTranscription([Request] AudioTranscriptionRequest request);

    [Multipart]
    [Post("/v1/audio/translations")]
    Task<AudioResponse> CreateTranslation([Request] AudioTranslationRequest request);

    [Post("/v1/audio/speech")]
    Task<IApiResponse<byte[]>> CreateSpeech([Body] SpeechRequest request);
}

/// <summary>
/// OpenAI 兼容 API 请求/响应模型
/// </summary>
public record AudioTranscriptionRequest
{
    [AliasAs("file")]
    public required StreamPart File { get; init; }

    [AliasAs("model")]
    public required string Model { get; init; }

    [AliasAs("language")]
    public string? Language { get; init; }

    [AliasAs("prompt")]
    public string? Prompt { get; init; }

    [AliasAs("response_format")]
    public string ResponseFormat { get; init; } = "json";

    [AliasAs("temperature")]
    public float? Temperature { get; init; }

    [AliasAs("timestamp_granularities")]
    public string[]? TimestampGranularities { get; init; }
}

public record AudioTranslationRequest
{
    [AliasAs("file")]
    public required StreamPart File { get; init; }

    [AliasAs("model")]
    public required string Model { get; init; }

    [AliasAs("prompt")]
    public string? Prompt { get; init; }

    [AliasAs("response_format")]
    public string ResponseFormat { get; init; } = "json";

    [AliasAs("temperature")]
    public float? Temperature { get; init; }
}

public record SpeechRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("input")]
    public required string Input { get; init; }

    [JsonPropertyName("voice")]
    public string Voice { get; init; } = "alloy";

    [JsonPropertyName("response_format")]
    public string ResponseFormat { get; init; } = "mp3";

    [JsonPropertyName("speed")]
    public float Speed { get; init; } = 1.0f;
}

public record AudioResponse
{
    [JsonPropertyName("text")]
    public required string Text { get; init; }

    [JsonPropertyName("task")]
    public string? Task { get; init; }

    [JsonPropertyName("language")]
    public string? Language { get; init; }

    [JsonPropertyName("duration")]
    public float? Duration { get; init; }

    [JsonPropertyName("words")]
    public WordInfo[]? Words { get; init; }

    [JsonPropertyName("segments")]
    public SegmentInfo[]? Segments { get; init; }
}

public record WordInfo(string Word, float Start, float End, float? Probability);

public record SegmentInfo(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("seek")] int Seek,
    [property: JsonPropertyName("start")] float Start,
    [property: JsonPropertyName("end")] float End,
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("tokens")] int[] Tokens,
    [property: JsonPropertyName("temperature")] float Temperature,
    [property: JsonPropertyName("avg_logprob")] float AvgLogprob,
    [property: JsonPropertyName("compression_ratio")] float CompressionRatio,
    [property: JsonPropertyName("no_speech_prob")] float NoSpeechProb
);

/// <summary>
/// Groq API 接口 (超快推理)
/// </summary>
public interface IGroqApi
{
    [Post("/v1/audio/transcriptions")]
    Task<AudioResponse> TranscribeAsync([Request] AudioTranscriptionRequest request);
}

/// <summary>
/// DeepSeek API 接口
/// </summary>
public interface IDeepSeekApi
{
    [Post("/v1/audio/transcriptions")]
    Task<AudioResponse> TranscribeAsync([Request] AudioTranscriptionRequest request);
}

/// <summary>
/// 云端推理服务实现
/// </summary>
public class CloudInferenceService : ICloudInferenceService
{
    private readonly IOpenAIAudioApi _openAIApi;
    private readonly IGroqApi? _groqApi;
    private readonly string _defaultModel;
    private readonly ILogger _logger;

    public CloudInferenceService(
        IOpenAIAudioApi openAIApi,
        string defaultModel = "whisper-1")
    {
        _openAIApi = openAIApi;
        _defaultModel = defaultModel;
        _logger = new NoOpLogger();
    }

    public CloudInferenceService WithGroq(IGroqApi groqApi)
    {
        return new CloudInferenceService(_openAIApi, _defaultModel) { _groqApi = groqApi };
    }

    public async Task<TranscriptionResult> TranscribeAsync(byte[] audioData, string language = "auto", CancellationToken ct = default)
    {
        using var audioStream = new MemoryStream(audioData);
        var request = new AudioTranscriptionRequest
        {
            File = new StreamPart(audioStream, "audio.wav", "audio/wav"),
            Model = _defaultModel,
            Language = language == "auto" ? null : language,
            Temperature = 0.0f,
            TimestampGranularities = new[] { "word" }
        };

        var response = await _openAIApi.CreateTranscription(request);

        var words = response.Words?.Select(w => new WordTimestamp(
            w.Word,
            TimeSpan.FromSeconds(w.Start),
            TimeSpan.FromSeconds(w.End),
            w.Probability ?? 1f
        )).ToList();

        return new TranscriptionResult
        {
            Text = response.Text,
            Language = response.Language ?? language,
            Confidence = CalculateConfidence(words),
            StartTime = TimeSpan.Zero,
            EndTime = TimeSpan.FromSeconds(response.Duration ?? 0),
            IsFinal = true,
            Words = words
        };
    }

    public async Task<TranslationResult> TranslateAsync(string text, string targetLanguage, string sourceLanguage = "auto", CancellationToken ct = default)
    {
        // 这里使用 OpenAI 的 Chat API 进行翻译
        // 也可以使用专门的翻译 API
        return new TranslationResult
        {
            SourceText = text,
            TranslatedText = text, // 实际实现需要调用 API
            SourceLanguage = sourceLanguage,
            TargetLanguage = targetLanguage,
            Confidence = 1.0f
        };
    }

    public async Task<SynthesisResult> SynthesizeAsync(string text, string voice = "default", CancellationToken ct = default)
    {
        var request = new SpeechRequest
        {
            Model = "tts-1",
            Input = text,
            Voice = voice switch
            {
                "male" => "alloy",
                "female" => "nova",
                _ => "alloy"
            },
            ResponseFormat = "mp3",
            Speed = 1.0f
        };

        var response = await _openAIApi.CreateSpeech(request);
        var audioData = await response.GetContentAsByteArrayAsync(ct);

        return new SynthesisResult
        {
            AudioData = audioData,
            Format = "mp3",
            SampleRate = 24000,
            Duration = TimeSpan.Zero // 可以从文件头解析
        };
    }

    public async IAsyncEnumerable<TranscriptionResult> StreamTranscribeAsync(
        IAsyncEnumerable<byte[]> audioStream,
        string language = "auto",
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        // 实现流式识别 (需要服务端支持 WebSocket 或 SSE)
        await foreach (var chunk in audioStream.WithCancellation(ct))
        {
            var result = await TranscribeAsync(chunk, language, ct);
            yield return result with { IsFinal = false };
        }
    }

    private float CalculateConfidence(List<WordTimestamp>? words)
    {
        if (words == null || words.Count == 0) return 1.0f;
        return words.Average(w => w.Probability);
    }
}

internal class NoOpLogger : ILogger
{
    public void Log(string message) { }
    public void LogError(string error) { }
    public void LogWarning(string warning) { }
}

internal interface ILogger
{
    void Log(string message);
    void LogError(string error);
    void LogWarning(string warning);
}
