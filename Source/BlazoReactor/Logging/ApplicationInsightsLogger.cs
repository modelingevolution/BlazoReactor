using System.Globalization;
using Microsoft.Extensions.Logging;

namespace BlazoReactor.Logging;

public class ApplicationInsightsLogger : ILogger
{
    private static NullScope Scope { get; } = new NullScope();
    private readonly IApplicationInsights _appInsights;
        
    public ApplicationInsightsLogger(IApplicationInsights appInsights)
    {
        _appInsights = appInsights;
            
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return Scope;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        SeverityLevel severityLevel = SeverityLevel.Verbose;
        var msg = formatter(state, exception);
        var properties = GetProperties(state);

        switch (logLevel)
        {
            case LogLevel.Trace:
            case LogLevel.Debug:
                severityLevel = SeverityLevel.Verbose;
                break;
            case LogLevel.Information:
                severityLevel = SeverityLevel.Information;
                break;
            case LogLevel.Warning:
                severityLevel = SeverityLevel.Warning;
                break;
            case LogLevel.Error:
                severityLevel = SeverityLevel.Error;
                break;
            case LogLevel.Critical:
            case LogLevel.None:
                severityLevel = SeverityLevel.Critical;
                break;
        }

        if (exception != null)
        {
            _appInsights.TrackException(new Error { Name = exception.GetType().Name, Message = exception.ToString() }, null, severityLevel, properties)
                        .GetAwaiter()
                        .GetResult();
        }
        else
        {
            _appInsights.TrackTrace(msg, severityLevel, properties)
                        .GetAwaiter()
                        .GetResult();
        }
    }

    private Dictionary<string, object?>? GetProperties<TState>(TState state)
    {
        if (state is not IReadOnlyList<KeyValuePair<string, object>> properties)
        {
            return null;
        }

        Dictionary<string, object?> dict = new Dictionary<string, object?>();
        foreach (KeyValuePair<string, object> property in properties)
        {
            dict[property.Key] = Convert.ToString(property.Value, CultureInfo.InvariantCulture);
        }

        return dict;
    }
}