namespace OSManager.Core.Attributes;

public class PrettyNameAttribute(string prettyName) : Attribute
{
    public string PrettyName { get; private set; } = prettyName;
}