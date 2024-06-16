using k8s.Models;
using Kontroller.API.TargetVersions;

namespace Kontroller.API.Services;

internal interface IKubernetesService : IDisposable
{
    public Task<V1Deployment[]> GetDeployments();
    public Task<TargetVersion[]> GetDeploymentVersions();
}