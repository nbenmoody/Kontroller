using k8s.Models;

namespace Kontroller.API.Models;

internal class KontrollerDeployment(string name)
{
    public string Name { get; set; } = name;
}