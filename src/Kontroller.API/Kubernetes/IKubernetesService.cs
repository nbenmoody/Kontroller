using Kontroller.API.TargetVersions;

namespace Kontroller.API.Services;

internal interface IKubernetesService : IDisposable
{
    public Task<TargetVersion[]> GetDeployments();
}