namespace BlazoReactor.EventAggregator;

/// Represents a reference to a <see cref="Delegate"/>.
/// </summary>
public interface IDelegateReference
{
    /// <summary>
    /// Gets the referenced <see cref="Delegate" /> object.
    /// </summary>
    /// <value>A <see cref="Delegate"/> instance if the target is valid; otherwise <see langword="null"/>.</value>
    Delegate Target { get; }
}
/// Represents a reference to a <see cref="Delegate"/>.
/// </summary>
public interface IDelegateReference<TDelegate> where TDelegate : Delegate
{
    /// <summary>
    /// Gets the referenced <see cref="Delegate" /> object.
    /// </summary>
    /// <value>A <see cref="Delegate"/> instance if the target is valid; otherwise <see langword="null"/>.</value>
    TDelegate Target { get; }
}