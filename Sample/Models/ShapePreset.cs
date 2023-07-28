namespace Sample.Models;

public class ShapePreset
{
    public ShapePreset(string value, string name)
    {
        Value = value;
        Name = name;
    }

    public string Value { get; }

    public string Name { get; }
}