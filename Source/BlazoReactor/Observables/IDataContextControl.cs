namespace BlazoReactor.Observables;

public interface IDataContextControl<T>
{
    public T DataContext { get; set; }
}