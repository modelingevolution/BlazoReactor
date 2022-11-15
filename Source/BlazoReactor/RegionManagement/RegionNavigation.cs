using System.Collections.Concurrent;

namespace BlazoReactor.RegionManagement;

public class RegionNavigation : IRegionNavigation
{
    private readonly ConcurrentDictionary<string, Type> _indexByName;
    private readonly IRegionManager _regionManager;

    public RegionNavigation(IRegionManager regionManager)
    {
        _regionManager = regionManager;
        _indexByName = new ConcurrentDictionary<string, Type>();
    }


    public void Navigate(Uri url, params ControlParameter[] args)
    {
        if (url.Scheme != "app") throw new NotSupportedException("Unsupported schema");
        var regionName = url.Host;
        var controlName = url.LocalPath;

        if (_indexByName.TryGetValue(controlName, out var controlType))
        {
            _regionManager[regionName].Add(controlType, args);
        }
        else ThrowNoRegistrationFound(url.ToString());
    }

    private static void ThrowNoRegistrationFound(string url) =>
        throw new InvalidOperationException($"No registration found for url: {url}.");

    public void RegisterName<TControl>(string name)
    {
        if (!_indexByName.TryAdd(name, typeof(TControl)) && _indexByName[name] != typeof(TControl))
            throw new InvalidOperationException($"Name already registered to {_indexByName[name]}");
        if (!name.StartsWith("/"))
            throw new ArgumentException("Name should start with '/'");
    }
}