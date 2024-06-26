using k8s.Models;
using Kontroller.API.Models;

namespace Kontroller.API.Services;

internal interface IKubernetesService : IDisposable
{
    public Task<KontrollerDeployment[]> GetDeployments();
    public Task<List<KontrollerVersion>> GetVersions();
}