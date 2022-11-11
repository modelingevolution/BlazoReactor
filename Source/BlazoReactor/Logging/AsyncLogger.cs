using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace BlazoReactor.Logging;

public class AsyncLogger : ILogger
{
    private readonly ILogger _core;
    private readonly ConcurrentQueue<Action> _queue;
    private int _count;
    public AsyncLogger(ILogger core)
    {
        _core = core;
        _queue = new ConcurrentQueue<Action>();
        _count = 0;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _queue.Enqueue(() => _core.Log(logLevel, eventId, state, exception, formatter));
        if (Interlocked.Increment(ref _count) == 1)
            Task.Run(SendLogs);
    }

    private void SendLogs()
    {
        while (_queue.TryDequeue(out var action))
        {
            action();
            if (Interlocked.Decrement(ref _count) == 0) 
                return;
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _core.IsEnabled(logLevel);
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return _core.BeginScope(state);
    }
}