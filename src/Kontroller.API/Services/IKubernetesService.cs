using k8s.Models;
using Kontroller.API.Models;

namespace Kontroller.API.Services;

internal interface IKubernetesService : IDisposable
{
    public Task<List<KontrollerChart>> GetHelmCharts();
    public Task<List<KontrollerService>> GetServices();
    public Task<List<KontrollerDeployment>> GetDeployments();
    public Task<List<KontrollerVersion>> GetVersions();
}