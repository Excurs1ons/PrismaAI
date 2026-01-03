using PrismaAI.Core.Models;
using System.Reactive;

namespace PrismaAI.Core.Audio;

/// <summary>
/// 音频捕获接口
/// </summary>
public interface IAudioCapture : IDisposable
{
    /// <summary>音频格式</summary>
    AudioFormat Format { get; }

    /// <summary>是否正在捕获</summary>
    bool IsCapturing { get; }

    /// <summary>音频数据流</summary>
    IObservable<AudioChunk> AudioStream { get; }

    /// <summary>开始捕获</summary>
    Task<Unit> StartAsync(CancellationToken cancellationToken = default);

    /// <summary>停止捕获</summary>
    Task<Unit> StopAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// 音频捕获工厂
/// </summary>
public interface IAudioCaptureFactory
{
    /// <summary>创建音频捕获器</summary>
    IAudioCapture Create(AudioCaptureConfig config);
}
