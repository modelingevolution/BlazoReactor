using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using ModelingEvolution.Plumberd.Logging;

namespace BlazoReactor.Logging;

public class ApplicationInsights : IApplicationInsights
{
    private readonly Func<IApplicationInsights, Task>? _init;
    private readonly ILogger _log = LogFactory.GetLogger();

    public ApplicationInsights(Func<IApplicationInsights, Task> init)
    {
        _init = init;
    }

    private IJSRuntime? Js { get; set; }

    public async Task Init(IJSRuntime jSRuntime)
    {
        Js = jSRuntime;
        if (_init != null) await _init(this);
    }

    public async Task TrackPageView(string? name = null, string? uri = null, string? refUri = null, string? pageType = null, bool? isLoggedIn = null, Dictionary<string, object>? properties = null)
    {
        if (Js is null)
        {
            LogThatJsIsNotInitializedYet();
            return;
        }
        
        await Js.InvokeVoidAsync("appInsights.trackPageView", new { name, uri, refUri, pageType, isLoggedIn }, properties);
    }

    public async Task TrackEvent(string name, Dictionary<string, object>? properties = null)
    {
        if (Js is null)
        {
            LogThatJsIsNotInitializedYet();
            return;
        }
        
        await Js.InvokeVoidAsync("appInsights.trackEvent", new { name }, properties);
    }

    public async Task TrackTrace(string message, SeverityLevel? severityLevel = null,
                                 Dictionary<string, object?>? properties = null)
    {
        if (Js is null)
        {
            LogThatJsIsNotInitializedYet();
            return;
        }
        
        await Js.InvokeVoidAsync("appInsights.trackTrace", new { message, severityLevel }, properties);
    }

    public async Task TrackException(Error exception, string? id = null, SeverityLevel? severityLevel = null,
                                     Dictionary<string, object?>? properties = null)
    {
        if (Js is null)
        {
            LogThatJsIsNotInitializedYet();
            return;
        }
        
        await Js.InvokeVoidAsync("appInsights.trackException", new { id, exception, severityLevel }, properties);
    }

    public async Task StartTrackPage(string? name = null)
    {
        if (Js is null)
        {
            LogThatJsIsNotInitializedYet();
            return;
        }
        
        await Js.InvokeVoidAsync("appInsights.startTrackPage", name);
    }

    public async Task StopTrackPage(string? name = null, string? url = null, Dictionary<string, string>? properties = null, Dictionary<string, decimal>? measurements = null)
    {
        if (Js is null)
        {
            LogThatJsIsNotInitializedYet();
            return;
        }
        
        await Js.InvokeVoidAsync("appInsights.stopTrackPage", name, url, properties, measurements);
    }

    public async Task TrackMetric(string name, double average, double? sampleCount = null, double? min = null, double? max = null, Dictionary<string, object>? properties = null)
    {
        if (Js is null)
        {
            LogThatJsIsNotInitializedYet();
            return;
        }
        
        await Js.InvokeVoidAsync("appInsights.trackMetric", new { name, average, sampleCount, min, max }, properties);
    }

    public async Task TrackDependencyData(string id, string name, decimal? duration = null, bool? success = null, DateTime? startTime = null, int? responseCode = null, string? correlationContext = null, string? type = null, string? data = null, string? target = null)
    {
        if (Js is null)
        {
            LogThatJsIsNotInitializedYet();
            return;
        }
        
        await Js.InvokeVoidAsync("blazorApplicationInsights.trackDependencyData", new { id, name, duration, success, startTime = startTime?.ToString("yyyy-MM-ddTHH:mm:ss"), responseCode, correlationContext, type, data, target });
    }

    public async Task Flush(bool? async = true)
    {
        if (Js is null)
        {
            LogThatJsIsNotInitializedYet();
            return;
        }
        
        await Js.InvokeVoidAsync("appInsights.flush", async);
    }

    public async Task ClearAuthenticatedUserContext()
    {
        if (Js is null)
        {
            LogThatJsIsNotInitializedYet();
            return;
        }
        
        await Js.InvokeVoidAsync("appInsights.clearAuthenticatedUserContext");
    }

    public async Task SetAuthenticatedUserContext(string authenticatedUserId, string? accountId = null, bool storeInCookie = false)
    {
        if (Js is null)
        {
            LogThatJsIsNotInitializedYet();
            return;
        }
        
        await Js.InvokeVoidAsync("appInsights.setAuthenticatedUserContext", authenticatedUserId, accountId, storeInCookie);
    }

    public async Task AddTelemetryInitializer(TelemetryItem telemetryItem)
    {
        if (Js is null)
        {
            LogThatJsIsNotInitializedYet();
            return;
        }
        
        await Js.InvokeVoidAsync("blazorApplicationInsights.addTelemetryInitializer", telemetryItem);
    }

    public async Task TrackPageViewPerformance(PageViewPerformanceTelemetry pageViewPerformance)
    {
        if (Js is null)
        {
            LogThatJsIsNotInitializedYet();
            return;
        }
        
        await Js.InvokeVoidAsync("appInsights.trackPageViewPerformance", pageViewPerformance);
    }

    public async Task StartTrackEvent(string name)
    {
        if (Js is null)
        {
            LogThatJsIsNotInitializedYet();
            return;
        }
        
        await Js.InvokeVoidAsync("appInsights.startTrackEvent", name);
    }

    public async Task StopTrackEvent(string name, Dictionary<string, string>? properties = null, Dictionary<string, decimal>? measurements = null)
    {
        if (Js is null)
        {
            LogThatJsIsNotInitializedYet();
            return;
        }
        
        await Js.InvokeVoidAsync("appInsights.stopTrackEvent", name, properties, measurements);
    }

    public async Task SetInstrumentationKey(string key)
    {
        if (Js is null)
        {
            LogThatJsIsNotInitializedYet();
            return;
        }
        
        await Js.InvokeVoidAsync("blazorApplicationInsights.setInstrumentationKey", key);
    }

    public async Task LoadAppInsights()
    {
        if (Js is null)
        {
            LogThatJsIsNotInitializedYet();
            return;
        }
        
        await Js.InvokeVoidAsync("blazorApplicationInsights.loadAppInsights");
    }

    private void LogThatJsIsNotInitializedYet()
    {
        _log.LogInformation("JSRuntime is not initialized yet. Call {InitMethodName} method", nameof(Init));
    }
}