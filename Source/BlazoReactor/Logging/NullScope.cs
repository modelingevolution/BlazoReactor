namespace BlazoReactor.Logging;

internal class NullScope : IDisposable
{
    public NullScope()
    {
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
}