using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlazoReactor.Logging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationInsights(this IServiceCollection services, Func<IApplicationInsights,Task> config)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER")))
        {
            AddLoggerProvider(services);
        }

        return services.AddSingleton<IApplicationInsights>(_ => new ApplicationInsights(config));
    } 
    private static void AddLoggerProvider(IServiceCollection services)
    {
        services.AddSingleton<ILoggerProvider, ApplicationInsightsLoggerProvider>();
    }
}