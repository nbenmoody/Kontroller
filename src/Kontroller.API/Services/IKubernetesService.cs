using k8s.Models;

namespace Kontroller.API.Services;

internal interface IKubernetesService : IDisposable
{
    public Task<V1DeploymentList> GetDeployments();
}