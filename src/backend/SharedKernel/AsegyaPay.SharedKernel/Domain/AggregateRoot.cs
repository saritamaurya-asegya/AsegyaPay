namespace AsegyaPay.SharedKernel.Domain;

/// <summary>
/// Base class for aggregate roots — top-level domain objects that maintain consistency boundaries.
/// </summary>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    protected AggregateRoot(TId id) : base(id) { }

    public uint Version { get; protected set; }
}
