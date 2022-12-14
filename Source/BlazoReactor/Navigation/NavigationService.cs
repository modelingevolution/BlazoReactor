using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazoReactor.Navigation;

public class NavigationService
{
    private readonly NavigationManager _navigationManager;
    private int _back;
    private readonly ObservableCollection<string> _history;

    public event EventHandler<LocationChangedEventArgs> LocationChanged
    {
        add => _navigationManager.LocationChanged += value;
        remove => _navigationManager.LocationChanged -= value;
    }
    
    public string BaseUri => _navigationManager.BaseUri;

    public string Uri => _navigationManager.Uri;

    public NavigationService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
            
        _history = new ObservableCollection<string>
        {
            _navigationManager.Uri.Substring(_navigationManager.BaseUri.Length)
        };
        _back = 0;
        _navigationManager.LocationChanged += OnLocationChanged;
    }

    public void NavigateTo(string uri, bool forceLoad = false)
    {
        _back = 0;
        _navigationManager.NavigateTo(uri, forceLoad);
    }

    public Uri ToAbsoluteUri(string relativeUri)
    {
        return _navigationManager.ToAbsoluteUri(relativeUri);
    }

    public string ToBaseRelativePath(string uri)
    {
        return _navigationManager.ToBaseRelativePath(uri);
    }
    
    public void GoBack()
    {
        var last = _history.Reverse().Skip(1+_back*2).FirstOrDefault();
        if (last == null) return;
        _back += 1;
        _navigationManager.NavigateTo(last);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        _history.Add(e.Location.Substring(BaseUri.Length));
    }
}