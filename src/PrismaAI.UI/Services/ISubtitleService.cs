using PrismaAI.Core.Models;

namespace PrismaAI.UI.Services;

/// <summary>
/// 字幕服务接口
/// </summary>
public interface ISubtitleService
{
    /// <summary>字幕接收事件</summary>
    event EventHandler<SubtitleEventArgs> SubtitleReceived;

    /// <summary>状态变化事件</summary>
    event EventHandler<string> StateChanged;

    /// <summary>开始处理</summary>
    Task StartAsync(PipelineConfig config);

    /// <summary>停止处理</summary>
    Task StopAsync();

    /// <summary>当前状态</summary>
    string CurrentState { get; }
}

/// <summary>
/// 字幕事件参数
/// </summary>
public class SubtitleEventArgs : EventArgs
{
    public required string Text { get; set; }
    public string? SourceLanguage { get; set; }
    public string? TargetLanguage { get; set; }
    public bool IsFinal { get; set; }
    public bool IsTranslation { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public float Confidence { get; set; }
}
