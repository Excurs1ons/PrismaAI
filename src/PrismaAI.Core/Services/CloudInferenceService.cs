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

    /// <summary>流式语音识别</summary>
    IAsyncEnumerable<TranscriptionResult> StreamTranscribeAsync(IAsyncEnumerable<byte[]> audioStream, string language = "auto", CancellationToken ct = default);
}

/// <summary>
/// OpenAI 兼容 API 接口
/// </summary>
public interface IOpenAIAudioApi
{
    [Multipart]
    [Post("/v1/audio/transcriptions")]
    Task<AudioResponse> CreateTranscription([AliasAs("file")] StreamPart file, [AliasAs("model")] string model, [AliasAs("language")] string? language = null);

    [Post("/v1/audio/speech")]
    Task<byte[]> CreateSpeech([Body] SpeechRequest request);
}

/// <summary>
/// API 请求/响应模型
/// </summary>
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
/// 云端推理服务实现
/// </summary>
public class CloudInferenceService : ICloudInferenceService
{
    private readonly IOpenAIAudioApi _openAIApi;
    private readonly string _defaultModel;

    public CloudInferenceService(IOpenAIAudioApi openAIApi, string defaultModel = "whisper-1")
    {
        _openAIApi = openAIApi;
        _defaultModel = defaultModel;
    }

    public async Task<TranscriptionResult> TranscribeAsync(byte[] audioData, string language = "auto", CancellationToken ct = default)
    {
        using var audioStream = new MemoryStream(audioData);
        var file = new StreamPart(audioStream, "audio.wav", "audio/wav");

        var response = await _openAIApi.CreateTranscription(file, _defaultModel, language == "auto" ? null : language);

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
        // TODO: 实现实际的翻译 API 调用
        await Task.CompletedTask;
        return new TranslationResult
        {
            SourceText = text,
            TranslatedText = text,
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

        var audioData = await _openAIApi.CreateSpeech(request);

        return new SynthesisResult
        {
            AudioData = audioData,
            Format = "mp3",
            SampleRate = 24000,
            Duration = TimeSpan.Zero
        };
    }

    public async IAsyncEnumerable<TranscriptionResult> StreamTranscribeAsync(
        IAsyncEnumerable<byte[]> audioStream,
        string language = "auto",
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
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
