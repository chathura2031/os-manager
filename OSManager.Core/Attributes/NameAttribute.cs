namespace OSManager.Core.Attributes;

public class NameAttribute(string name) : Attribute
{
    public string Name { get; private set; } = name;
}