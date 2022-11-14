namespace BlazoReactor.RegionManagement;

internal class ContentControlRegionAdapter
{
    class Region : IRegion
    {
        private int _id;
        public IContentControl? Control;
        
        public Region(string regionName)
        {
            RegionName = regionName;
        }

        public string RegionName { get; }
        public void Clear()
        {
            if (Control is null)
            {
                return;
            }
            
            Control.Control = null;
        }

        public ControlToken Add(Type controlType, params ControlParameter[] args)
        {
            Control?.SetContent(controlType, ++_id, args);
            return new ControlToken(_id);
        }


        public void Remove(ControlToken token)
        {
            if (token.Id != _id)
            {
                return;
            }

            if (Control is null)
            {
                return;
            }
            
            Control.Control = null;
        }
    }
    
    public IRegion Create(string regionName)
    {
        return new Region(regionName);
    }

    public void Associate(IRegion region, IContentControl contentControl)
    {
        ((Region) region).Control = contentControl;
    }
}