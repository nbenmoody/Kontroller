using k8s.Models;

namespace Kontroller.API.Models;

internal class Deployment
{
    private string Name { get; set; }

    public Deployment(V1Deployment v1Deployment)
    {
        Name = v1Deployment.Name();
    }
}