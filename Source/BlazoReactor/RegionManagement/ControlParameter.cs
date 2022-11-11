namespace BlazoReactor.RegionManagement;

public readonly struct ControlParameter
{
    public readonly string Name;
    public readonly object Value;

    public ControlParameter(string name, object value)
    {
        Name = name;
        Value = value;
    }
}