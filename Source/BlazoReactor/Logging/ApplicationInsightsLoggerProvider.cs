using Microsoft.Extensions.Logging;

namespace BlazoReactor.Logging;

public class ApplicationInsightsLoggerProvider : ILoggerProvider
{
    private readonly IApplicationInsights _appInsights;
    private ILogger _logger;
    private bool _disposed = false;

    public ApplicationInsightsLoggerProvider(IApplicationInsights appInsights)
    {
        _appInsights = appInsights;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _logger ??= new AsyncLogger(new ApplicationInsightsLogger(_appInsights));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _logger = null;
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }

}