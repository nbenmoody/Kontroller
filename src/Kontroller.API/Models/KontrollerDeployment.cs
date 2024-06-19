using k8s.Models;

namespace Kontroller.API.Models;

internal class KontrollerDeployment
{
    public string Name { get; set; }

    public KontrollerDeployment(V1Deployment v1Deployment)
    {
        Name = v1Deployment.Name();
    }
}