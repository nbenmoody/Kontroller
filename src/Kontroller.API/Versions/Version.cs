namespace Kontroller.API.Versions;

internal sealed class Version(string name, string version)
{
    public string Name { get; set; } = name;
    public string VersionNumber { get; set; } = version;
}
