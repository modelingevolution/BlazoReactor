using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using ModelingEvolution.Plumberd.Logging;

namespace BlazoReactor.EventAggregator
{
    /// <summary>
    /// Implements <see cref="IEventAggregator"/>.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        private static readonly ILogger Log = LogFactory.GetLogger();

        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
            { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, };

        private readonly IJSRuntime _js;
        private readonly IJsInteropTypeRegister _typeRegister;

        private readonly Dictionary<Type, EventBase> _events;
        private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        private IJSObjectReference _proxy;

        public EventAggregator(IJSRuntime js, IJsInteropTypeRegister typeRegister)
        {
            _js = js;
            _typeRegister = typeRegister;
            _events = new Dictionary<Type, EventBase>();
            if (_js != null) ConnectJs();
        }

        [JSInvokable]
        public void Send(string eventType, string json)
        {
            Type eType = null;
            try
            {
                if (_typeRegister.TryGetTypeByName(eventType, out eType))
                {
                    var evt = JsonSerializer.Deserialize(json, eType, JsonSerializerOptions);
                    GetEvent(eType).Publish(evt);
                }
                else
                {
                    Log.LogWarning("Type {Typename} is unregistered and we cannot publish event in dotnet", eventType);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Could not publish event (JS => DotNet): {EventType} {Json}", eType?.Name ?? "unknown",
                             json);
            }
        }

        private async Task ConnectJs()
        {
            if (_proxy == null)
            {
                var reference = DotNetObjectReference.Create(this);
                _proxy = await _js.InvokeAsync<IJSObjectReference>("EventBus", reference);
            }
        }

        static JsonSerializerOptions serializeOptions = new JsonSerializerOptions
            { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        private void Receive(object obj)
        {
            if (!_typeRegister.TryGetNameByType(obj.GetType(), out var name)) return;

            var json = JsonSerializer.Serialize(obj, serializeOptions);
            _proxy.InvokeVoidAsync("send", name, json);
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
                EventBase existingEvent = null;

                if (!_events.TryGetValue(typeof(TEventType), out existingEvent))
                {
                    var newEvent = new PubSubEvent<TEventType>();
                    newEvent.SynchronizationContext = _syncContext;
                    newEvent.OnChannelEvent = this.OnChannelSend;
                    _events[typeof(TEventType)] = newEvent;

                    return newEvent;
                }
                else
                {
                    return (PubSubEvent<TEventType>)existingEvent;
                }
            }
        }

        private void OnChannelSend(object obj)
        {
            this.Receive(obj);
        }

        public EventBase GetEvent(Type value)
        {
            lock (_events)
            {
                EventBase existingEvent = null;

                if (!_events.TryGetValue(value, out existingEvent))
                {
                    var pubSubType = typeof(PubSubEvent<>).MakeGenericType(value);
                    var newEvent = (EventBase)Activator.CreateInstance(pubSubType);
                    newEvent.SynchronizationContext = _syncContext;
                    newEvent.OnChannelEvent = this.OnChannelSend;
                    _events[value] = newEvent;

                    return newEvent;
                }
                else
                {
                    return (EventBase)existingEvent;
                }
            }
        }
    }
}