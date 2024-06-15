namespace Kontroller.API.TargetVersions;

internal sealed class TargetVersion(string name, string version)
{
    public string Name { get; set; } = name;
    public string VersionNumber { get; set; } = version;
}
