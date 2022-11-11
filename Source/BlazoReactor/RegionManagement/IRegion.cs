namespace BlazoReactor.RegionManagement
{
    public interface IRegion
    {
        ControlToken Add(Type controlType, params ControlParameter[] args);

        ControlToken Add(Type controlType, object dataContext) =>
            Add(controlType, new ControlParameter("DataContext", dataContext));

        ControlToken Add<TControl>(params ControlParameter[] args) => Add(typeof(TControl), args);
        ControlToken Add<TControl>(object dataContext) => Add(typeof(TControl), dataContext);
        void Remove(ControlToken token);
        string RegionName { get; }
        void Clear();
    }
}