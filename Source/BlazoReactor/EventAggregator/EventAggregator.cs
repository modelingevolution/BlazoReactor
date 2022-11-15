using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using ModelingEvolution.Plumberd.Logging;

namespace BlazoReactor.EventAggregator;

/// <summary>
/// Implements <see cref="IEventAggregator"/>.
/// </summary>
public class EventAggregator : IEventAggregator
{
    static readonly JsonSerializerOptions SerializeOptions = new JsonSerializerOptions
        { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    
    private static readonly ILogger Log = LogFactory.GetLogger();

    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, };

    private readonly IJSRuntime? _js;
    private readonly IJsInteropTypeRegister _typeRegister;

    private readonly Dictionary<Type, EventBase> _events;
    private readonly SynchronizationContext? _syncContext = SynchronizationContext.Current;

    private IJSObjectReference? _proxy;
    
    public EventAggregator(IJSRuntime js, IJsInteropTypeRegister typeRegister)
    {
        _js = js;
        _typeRegister = typeRegister;
        _events = new Dictionary<Type, EventBase>();
        // TODO: We want to not wait for it?
        TryConnectJs();
    }

    [JSInvokable]
    public void Send(string eventType, string json)
    {
        Type? eType = null;
        try
        {
            // TODO: another time is locked but here is not 
            if (!_typeRegister.TryGetTypeByName(eventType, out eType))
            {
                Log.LogWarning("Type {Typename} is unregistered and we cannot publish event in dotnet", eventType);

                return;
            }

            var evt = JsonSerializer.Deserialize(json, eType, JsonSerializerOptions);
            if (evt is null)
            {
                return;
            }

            GetEvent(eType).Publish(evt);
        }
        catch (Exception ex)
        {
            Log.LogError(ex, "Could not publish event (JS => DotNet): {EventType} {Json}", 
                         eType?.Name ?? "unknown",
                         json);
        }
    }

    /// <summary>
    /// Gets the single instance of the event managed by this EventAggregator. Multiple calls to this method with the same <typeparamref name="TEventType"/> returns the same event instance.
    /// </summary>
    /// <typeparam name="TEventType">The type of event to get. This must inherit from <see cref="EventBase"/>.</typeparam>
    /// <returns>A singleton instance of an event object of type <typeparamref name="TEventType"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
                                                     "CA1004:GenericMethodsShouldProvideTypeParameter")]
    public PubSubEvent<TEventType> GetEvent<TEventType>() where TEventType : new()
    {
        lock (_events)
        {
            if (_events.TryGetValue(typeof(TEventType), out var existingEvent))
            {
                return (PubSubEvent<TEventType>)existingEvent;
            }
            
            var newEvent = new PubSubEvent<TEventType>
            {
                SynchronizationContext = _syncContext,
                OnChannelEvent = OnChannelSend
            };
            _events[typeof(TEventType)] = newEvent;

            return newEvent;

        }
    }

    public EventBase GetEvent(Type value)
    {
        lock (_events)
        {
            if (_events.TryGetValue(value, out var existingEvent))
            {
                return existingEvent;
            }
            
            var pubSubType = typeof(PubSubEvent<>).MakeGenericType(value);
            var newEvent = (EventBase?)Activator.CreateInstance(pubSubType);

            if (newEvent is null)
            {
                Log.LogError("Cannot create instance of {Type}", pubSubType);
                throw new NullReferenceException();
            }
            
            newEvent.SynchronizationContext = _syncContext;
            newEvent.OnChannelEvent = OnChannelSend;
            _events[value] = newEvent;

            return newEvent;

        }
    }
    
    private async Task TryConnectJs()
    {
        if (_js == null)
        {
            return;
        }
        
        if (_proxy == null)
        {
            var reference = DotNetObjectReference.Create(this);
            _proxy = await _js.InvokeAsync<IJSObjectReference>("EventBus", reference);
        }    
    }
    
    private void OnChannelSend(object obj)
    {
        Receive(obj);
    }
    
    private void Receive(object obj)
    {
        if (!_typeRegister.TryGetNameByType(obj.GetType(), out var name)) return;

        var json = JsonSerializer.Serialize(obj, SerializeOptions);
        _proxy?.InvokeVoidAsync("send", name, json);
    }
}