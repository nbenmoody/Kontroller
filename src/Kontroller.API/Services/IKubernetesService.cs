using Kontroller.API.Versions;

namespace Kontroller.API.Services;

internal interface IKubernetesService : IDisposable
{
    public Task<TargetVersion[]> GetDeployments();
}