using Microsoft.AspNetCore.Components;

namespace BlazoReactor.RegionManagement
{
    public class RegionManager : IRegionManager
    {
        private readonly Dictionary<string, IRegion> _regions;
        private readonly Dictionary<string, List<Type>> _registeredViews;

        public RegionManager()
        {
            _regions = new Dictionary<string, IRegion>();
            _registeredViews = new Dictionary<string, List<Type>>();
        }

        public IRegion this[string regionName] => _regions[regionName.ToLower()];

        public IRegionManager RegisterViewWithRegion<TControl>(string regionName) where TControl : IComponent
        {
            regionName = regionName.ToLower();
            if (!_registeredViews.TryGetValue(regionName, out var list))
                _registeredViews.Add(regionName, list = new List<Type>());
            list.Add(typeof(TControl));
            return this;
        }

        public IRegionManager SetRegionName(IItemsControl itemsControl, string regionName)
        {
            regionName = regionName.ToLower();
            var adapter = new ItemsControlRegionAdapter();
            IRegion region = adapter.Create(regionName);
            adapter.Associate(region, itemsControl);
            _regions.Add(regionName, region);

            if (!_registeredViews.TryGetValue(regionName, out var controls))
            {
                return this;
            }
            
            foreach (Type type in controls)
            {
                region.Add(type);
            }

            return this;
        }

        public IRegionManager SetRegionName(IContentControl contentControl, string regionName)
        {
            regionName = regionName.ToLower();
            var adapter = new ContentControlRegionAdapter();
            IRegion region = adapter.Create(regionName);
            adapter.Associate(region, contentControl);
            _regions.Add(regionName, region);

            if (!_registeredViews.TryGetValue(regionName, out var controls))
            {
                return this;
            }

            foreach (Type type in controls)
            {
                region.Add(type);
            }
            
            return this;
        }
        
        public IRegionManager RemoveRegion(string regionName)
        {
            regionName = regionName.ToLower();
            var region = _regions[regionName];
            region.Clear();
            _regions.Remove(regionName);
            return this;
        }
    }
}