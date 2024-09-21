namespace OSManager.Core.Packages;

public class TemplatePackage: Package
{
    // TODO: Update the variable type
    public static readonly TemplatePackage Instance = new();
    
    // TODO: Update the name
    public override string Name { get; } = "TemplatePackage";

    // TODO: Update the name
    public override string SafeName { get; } = "template-package";

    // TODO: Update the constructor name
    private TemplatePackage() {}
    
    protected override int Install(int verbosity)
    {
        throw new NotImplementedException();
    }

    protected override int Configure(int verbosity)
    {
        throw new NotImplementedException();
    }
}