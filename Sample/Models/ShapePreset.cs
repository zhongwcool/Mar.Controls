namespace Sample.Models;

public class ShapePreset
{
    public ShapePreset(string value, string displayName)
    {
        Value = value;
        DisplayName = displayName;
    }

    public string Value { get; }

    public string DisplayName { get; }
}