using System.Text.Json.Serialization;

namespace ApproveMe.Models;

public abstract class AggregateBase
{
    public Guid Id { get; protected set; }

    // For protecting the state, i.e. conflict prevention
    // The setter is only public for setting up test conditions
    public long Version { get; set; }

    [JsonIgnore] private readonly List<object> _uncommittedEvents = new List<object>();

    public IEnumerable<object> GetUncommittedEvents()
    {
        return _uncommittedEvents;
    }

    public void ClearUncommittedEvents()
    {
        _uncommittedEvents.Clear();
    }

    protected void AddUncommittedEvent(object @event)
    {
        _uncommittedEvents.Add(@event);
    }
}