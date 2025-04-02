using System.Collections.Concurrent;
using System.Windows.Controls;
using System.Windows.Threading;
using TitanStudioWpf.Core.Interfaces;

namespace TitanStudioWpf.Core.Services;

public class Logger
{
    private TextBox? _textBox;
    private readonly ConcurrentQueue<string> _queuedLogs = new();
    private readonly DispatcherTimer _dispacherTimer;

    public Logger()
    {
        _dispacherTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _dispacherTimer.Tick += ProcessQueuedLogs;
    }
    public void Initialize(TextBox textBox)
    {
        _textBox = textBox;
        _dispacherTimer.Start();
    }

    public void Log(string message)
    {
        AddLogMessage($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
    }
    public void LogDebug(string message)
    {
        AddLogMessage($"[DEBUG] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
    }

    public void LogError(string message)
    {
        AddLogMessage($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
    }
    private void AddLogMessage(string message)
    {
        _queuedLogs.Enqueue(message);
    }

    private void ProcessQueuedLogs(object? sender, EventArgs e)
    {
        if (_textBox == null) return;

        while (_queuedLogs.TryDequeue(out string? message))
        {
            _textBox.AppendText($"{message}{Environment.NewLine}");
            _textBox.ScrollToEnd();
        }
    }
}
