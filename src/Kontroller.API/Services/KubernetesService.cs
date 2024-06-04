using k8s;
using k8s.Models;

namespace Kontroller.API.Services;

internal class KubernetesService : IDisposable
{
    private readonly KubernetesClientConfiguration _config = k8s.KubernetesClientConfiguration.InClusterConfig();
    private readonly Kubernetes _client;

    public KubernetesService()
    {
        _client = new Kubernetes(_config);
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public async Task<V1DeploymentList> GetDeployments()
    {
        var deployments = await _client.ListDeploymentForAllNamespacesAsync();
        return deployments;
    }

    // public async Task<V1DeploymentList> GetDeployments(string namespace)
    // {
    //     var deployments = await _client.ListNamespacedDeploymentAsync();
    // }
    
}