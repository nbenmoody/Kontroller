namespace Kontroller.API.Models;

internal sealed class KontrollerVersion(string name, string version)
{
    public string Name { get; set; } = name;
    public string VersionNumber { get; set; } = version;
}
