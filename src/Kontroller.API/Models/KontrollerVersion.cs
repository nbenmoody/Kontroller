namespace Kontroller.API.Models;

internal class KontrollerVersion(string name, string version)
{
    public string Name { get; set; } = name;
    public string VersionNumber { get; set; } = version;
}
