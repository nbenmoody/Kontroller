using k8s.Models;
using Kontroller.API.Models;
using Kontroller.API.TargetVersions;

namespace Kontroller.API.Services;

internal interface IKubernetesService : IDisposable
{
    public Task<KontrollerDeployment[]> GetDeployments();
    public Task<TargetVersion[]> GetVersions();
}