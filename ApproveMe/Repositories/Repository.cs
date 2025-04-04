using ApproveMe.Models;
using Marten;

namespace ApproveMe.Repositories;

public interface IRepository
{
    /// <summary>
    /// Loads an aggregate from the event store using the specified identifier, version, and timestamp.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate to load, which must inherit from AggregateBase and have a parameterless constructor.</typeparam>
    /// <param name="id">The unique identifier of the aggregate to load.</param>
    /// <param name="version">Optional version number to load the aggregate at. Defaults to 0, meaning the latest version.</param>
    /// <param name="timestamp">Optional timestamp to load the aggregate as it existed at a specific time. Defaults to null.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the requested aggregate.</returns>
    Task<TAggregate> Load<TAggregate>(Guid id, long version = 0, DateTimeOffset? timestamp = null)
        where TAggregate : AggregateBase, new();

    /// <summary>
    /// Saves the aggregate and commits it to the database
    /// </summary>
    Task Save(AggregateBase aggregate, bool orderEventsByTimestamp = false);

    /// <summary>
    /// Stores the aggregate, but doesn't commit to database
    /// </summary>
    void Store(AggregateBase aggregate, bool orderEventsByTimestamp = false);
    
    /// <summary>
    /// Commits changes to the database, only needed when Store has been used.
    /// </summary>
    Task SaveChanges();
}

public class Repository : IRepository
{
    private readonly IDocumentSession _session;

    public Repository(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<TAggregate> Load<TAggregate>(Guid id, long version = 0, DateTimeOffset? timestamp = null) where TAggregate : AggregateBase, new()
    {
        return await _session.Events.AggregateStreamAsync<TAggregate>(id, version, timestamp);
    }

    public void Store(AggregateBase aggregate, bool orderEventsByTimestamp = false)
    {
        var uncommittedEvents = aggregate.DequeueUncommittedEvents(orderEventsByTimestamp);
        if (!uncommittedEvents.Any())
        {
            return;
        }

        _session.Events.Append(aggregate.Id, aggregate.Version, uncommittedEvents);
    }

    public async Task Save(AggregateBase aggregate, bool orderEventsByTimestamp = false)
    {
        var uncommittedEvents = aggregate.DequeueUncommittedEvents(orderEventsByTimestamp);
        if (!uncommittedEvents.Any())
        {
            return;
        }

        _session.Events.Append(aggregate.Id, aggregate.Version, uncommittedEvents);

        await _session.SaveChangesAsync();
    }

    public async Task SaveChanges()
    {
        await _session.SaveChangesAsync();
    }
}