using System.Reflection;
using ApproveMe.InternalEvents;

namespace ApproveMe.Models;

public abstract class AggregateBase
{
    private readonly IList<BaseEvent> _uncommittedEvents = new List<BaseEvent>();

    public Guid Id { get; set; }
    public int Version { get; set; }

    protected void RaiseEvent(BaseEvent @event)
    {
        ApplyEvent(@event);

        _uncommittedEvents.Add(@event);
    }

    private void ApplyEvent(BaseEvent @event)
    {
        var applyMethod = GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(m => m.Name == "Apply" && m.GetParameters().Length == 1 && m.ReturnParameter.ParameterType == typeof(void))
            .SingleOrDefault(m => m.GetParameters().Single().ParameterType == @event.GetType());

        if (applyMethod != null)
        {
            applyMethod.Invoke(this, new[] { @event });

            Version++;
        }
    }

    public BaseEvent[] DequeueUncommittedEvents(bool orderEventsByTimestamp)
    {
        var dequeuedEvents = _uncommittedEvents.ToList();

        if (orderEventsByTimestamp)
        {
            dequeuedEvents = dequeuedEvents.OrderBy(x => x.Timestamp).ToList();
        }

        _uncommittedEvents.Clear();

        return dequeuedEvents.ToArray();
    }
}