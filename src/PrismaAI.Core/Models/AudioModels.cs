using System.Numerics;

namespace PrismaAI.Core.Models;

/// <summary>
/// 音频格式
/// </summary>
public record AudioFormat(int SampleRate, int Channels, int BitsPerSample = 16);

/// <summary>
/// 音频块
/// </summary>
public record AudioChunk(float[] Samples, long Timestamp, AudioFormat Format)
{
    public TimeSpan Duration => TimeSpan.FromSeconds((double)Samples.Length / (Format.SampleRate * Format.Channels));
    public int FrameCount => Samples.Length / Format.Channels;
}

/// <summary>
/// 音频源类型
/// </summary>
public enum AudioSourceType
{
    /// <summary>系统音频捕获</summary>
    SystemAudio,
    /// <summary>麦克风输入</summary>
    Microphone,
    /// <summary>音频文件</summary>
    File,
    /// <summary>网络流</summary>
    Stream
}

/// <summary>
/// 音频捕获配置
/// </summary>
public record AudioCaptureConfig
{
    public AudioSourceType SourceType { get; init; } = AudioSourceType.Microphone;
    public int SampleRate { get; init; } = 16000;
    public int Channels { get; init; } = 1;
    public int ChunkDurationMs { get; init; } = 1000; // 每块时长
    public string? FilePath { get; init; }
    public string? StreamUrl { get; init; }
}
