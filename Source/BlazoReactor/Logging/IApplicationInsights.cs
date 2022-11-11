using Microsoft.JSInterop;

namespace BlazoReactor.Logging;

public interface IApplicationInsights
{
    Task Init(IJSRuntime jSRuntime);
    Task TrackPageView(string? name = null, string? uri = null, string? refUri = null, string? pageType = null, bool? isLoggedIn = null, Dictionary<string, object>? properties = null);
    Task TrackEvent(string name, Dictionary<string, object>? properties = null);
    Task TrackTrace(string message, SeverityLevel? severityLevel = null, Dictionary<string, object>? properties = null);
    Task TrackException(Error exception, string? id = null, SeverityLevel? severityLevel = null, Dictionary<string, object>? properties = null);
    Task StartTrackPage(string? name = null);
    Task StopTrackPage(string? name = null, string? url = null, Dictionary<string, string>? properties = null, Dictionary<string, decimal>? measurements = null);
    Task TrackMetric(string name, double average, double? sampleCount = null, double? min = null, double? max = null, Dictionary<string, object>? properties = null);
    Task TrackDependencyData(string id, string name, decimal? duration = null, bool? success = null, DateTime? startTime = null, int? responseCode = null, string? correlationContext = null, string? type = null, string? data = null, string? target = null);
    Task Flush(bool? async = true);
    Task ClearAuthenticatedUserContext();
    Task SetAuthenticatedUserContext(string authenticatedUserId, string? accountId = null, bool storeInCookie = false);
    Task AddTelemetryInitializer(TelemetryItem telemetryItem);
    Task TrackPageViewPerformance(PageViewPerformanceTelemetry pageViewPerformance);
    Task StartTrackEvent(string name);
    Task StopTrackEvent(string name, Dictionary<string, string>? properties = null, Dictionary<string, decimal>? measurements = null);
    Task SetInstrumentationKey(string key);
    Task LoadAppInsights();
}