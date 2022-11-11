namespace BlazoReactor.RegionManagement;

public interface IRegionNavigation
{
    void Navigate(Uri url, params ControlParameter[] args);
    void Navigate(string url, params ControlParameter[] args) => Navigate(new Uri(url), args);
    void RegisterName<TControl>(string name);
    void RegisterName<TControl>() => RegisterName<TControl>($"/{typeof(TControl).Name}");
    void Navigate(string regionName, string name, params ControlParameter[] args) => Navigate(new Uri($"app://{regionName}/{name}"), args);
}