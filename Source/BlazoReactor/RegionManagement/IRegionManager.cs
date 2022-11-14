using Microsoft.AspNetCore.Components;

namespace BlazoReactor.RegionManagement
{
    public interface IRegionManager
    {
        IRegion this[string regionName] { get; }
        IRegionManager RegisterViewWithRegion<TControl>(string regionName) where TControl : IComponent;
        IRegionManager SetRegionName(IContentControl contentControl, string regionName);
        IRegionManager RemoveRegion(string regionName);
    }
}