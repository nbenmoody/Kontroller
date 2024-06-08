using k8s;
using k8s.Models;

namespace Kontroller.API.Services;

internal sealed class KubernetesService : IKubernetesService
{
    private readonly KubernetesClientConfiguration _config = k8s.KubernetesClientConfiguration.InClusterConfig();
    internal readonly Kubernetes client;

    public KubernetesService()
    {
        client = new Kubernetes(_config);
    }

    public void Dispose()
    {
        client.Dispose();
    }

    public async Task<V1DeploymentList> GetDeployments()
    {
        var deployments = await client.ListDeploymentForAllNamespacesAsync();
        return deployments;
    }

    // public async Task<V1DeploymentList> GetDeployments(string namespace)
    // {
    //     var deployments = await _client.ListNamespacedDeploymentAsync();
    // }
    
}