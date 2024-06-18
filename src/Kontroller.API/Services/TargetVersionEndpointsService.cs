using k8s.Models;
using Kontroller.API.Models;
using Kontroller.API.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Kontroller.API.TargetVersions;

internal class TargetVersionEndpointsService : ITargetVersionEndpointsService
{
    private readonly ILogger<ITargetVersionEndpointsService> _logger;
    private readonly IKubernetesService _kubernetesService;

    public TargetVersionEndpointsService(ILogger<ITargetVersionEndpointsService> logger, IKubernetesService kubernetesService)
    {
        _logger = logger;
        _kubernetesService = kubernetesService;
    }

    public async Task<Results<Ok<Deployment[]>, NotFound>> GetDeployments()
    {
        _logger.LogInformation("Scanning for V1Deployments...");
        var results = await _kubernetesService.GetDeployments();
        _logger.LogWarning($"Found {results.Length} Deployments.");
        Deployment[] deployments = [];
        foreach (var result in results)
        {
            deployments.Append(new Deployment(result));
        }

        return deployments.Length == 0 ? TypedResults.NotFound() : TypedResults.Ok(deployments);

    }

    // TODO: Ultimately, I would see this looking at more than just Deployments (Helm charts, ReplicaSets, Rollouts,
    //    or whatever else is set int he values.yaml file for the helm chart.
    public async Task<Results<Ok<TargetVersion[]>, NotFound>> GetVersions()
    {
        // TODO: All these warnings need to be information instead.
        _logger.LogWarning($"Scanning for TargetVersions...");
        var results = await _kubernetesService.GetDeploymentVersions();
        _logger.LogInformation($"Found {results.Length} versions.");
        foreach (var result in results)
        {
            _logger.LogInformation($"{result.Name} - {result.VersionNumber}");
        }
        return results.Length == 0 ? TypedResults.NotFound() : TypedResults.Ok(results);
    }

    public void Dispose()
    {
       _kubernetesService.Dispose(); 
    }
}
