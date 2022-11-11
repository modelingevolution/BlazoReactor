using Microsoft.JSInterop;

namespace BlazoReactor.Logging;

public class ApplicationInsights : IApplicationInsights
{
    private IJSRuntime? _js { get; set; }
    private readonly Func<IApplicationInsights, Task> _init;

    public ApplicationInsights(Func<IApplicationInsights, Task> init)
    {
        _init = init;
    }

    public async Task Init(IJSRuntime jSRuntime)
    {
        _js = jSRuntime;
        if (_init != null) await _init(this);
    }

    public async Task TrackPageView(string? name = null, string? uri = null, string? refUri = null, string? pageType = null, bool? isLoggedIn = null, Dictionary<string, object>? properties = null)
    {
        await _js.InvokeVoidAsync("appInsights.trackPageView", new { name, uri, refUri, pageType, isLoggedIn }, properties);
    }

    public async Task TrackEvent(string name, Dictionary<string, object>? properties = null)
    {
        await _js.InvokeVoidAsync("appInsights.trackEvent", new object[] { new { name }, properties });
    }

    public async Task TrackTrace(string message, SeverityLevel? severityLevel = null, Dictionary<string, object>? properties = null)
    {
        await _js.InvokeVoidAsync("appInsights.trackTrace", new { message, severityLevel }, properties);
    }

    public async Task TrackException(Error exception, string? id = null, SeverityLevel? severityLevel = null, Dictionary<string, object>? properties = null)
    {
        await _js.InvokeVoidAsync("appInsights.trackException", new { id, exception, severityLevel }, properties);
    }

    public async Task StartTrackPage(string? name = null)
    {
        await _js.InvokeVoidAsync("appInsights.startTrackPage", name);
    }

    public async Task StopTrackPage(string? name = null, string? url = null, Dictionary<string, string>? properties = null, Dictionary<string, decimal>? measurements = null)
    {
        await _js.InvokeVoidAsync("appInsights.stopTrackPage", name, url, properties, measurements);
    }

    public async Task TrackMetric(string name, double average, double? sampleCount = null, double? min = null, double? max = null, Dictionary<string, object>? properties = null)
    {
        await _js.InvokeVoidAsync("appInsights.trackMetric", new { name, average, sampleCount, min, max }, properties);
    }

    public async Task TrackDependencyData(string id, string name, decimal? duration = null, bool? success = null, DateTime? startTime = null, int? responseCode = null, string? correlationContext = null, string? type = null, string? data = null, string? target = null)
    {
        await _js.InvokeVoidAsync("blazorApplicationInsights.trackDependencyData", new { id, name, duration, success, startTime = startTime?.ToString("yyyy-MM-ddTHH:mm:ss"), responseCode, correlationContext, type, data, target });
    }

    public async Task Flush(bool? async = true)
    {
        await _js.InvokeVoidAsync("appInsights.flush", async);
    }

    public async Task ClearAuthenticatedUserContext()
    {
        await _js.InvokeVoidAsync("appInsights.clearAuthenticatedUserContext");
    }

    public async Task SetAuthenticatedUserContext(string authenticatedUserId, string? accountId = null, bool storeInCookie = false)
    {
        await _js.InvokeVoidAsync("appInsights.setAuthenticatedUserContext", authenticatedUserId, accountId, storeInCookie);
    }

    public async Task AddTelemetryInitializer(TelemetryItem telemetryItem)
    {
        await _js.InvokeVoidAsync("blazorApplicationInsights.addTelemetryInitializer", telemetryItem);
    }

    public async Task TrackPageViewPerformance(PageViewPerformanceTelemetry pageViewPerformance)
    {
        await _js.InvokeVoidAsync("appInsights.trackPageViewPerformance", pageViewPerformance);
    }

    public async Task StartTrackEvent(string name)
    {
        await _js.InvokeVoidAsync("appInsights.startTrackEvent", name);
    }

    public async Task StopTrackEvent(string name, Dictionary<string, string>? properties = null, Dictionary<string, decimal>? measurements = null)
    {
        await _js.InvokeVoidAsync("appInsights.stopTrackEvent", name, properties, measurements);
    }

    public async Task SetInstrumentationKey(string key)
    {
        await _js.InvokeVoidAsync("blazorApplicationInsights.setInstrumentationKey", key);
    }

    public async Task LoadAppInsights()
    {
        await _js.InvokeVoidAsync("blazorApplicationInsights.loadAppInsights");
    }
}