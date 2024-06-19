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

    public async Task<Results<Ok<KontrollerDeployment[]>, NotFound>> GetDeployments()
    {
        _logger.LogWarning("Scanning for Deployments...");
        var results = await _kubernetesService.GetDeployments();
        _logger.LogWarning($"Found {results.Length} Deployments.");
        
        foreach (var result in results)
        {
            _logger.LogWarning($"Found a thing: {result.Name}");
        }

        return results.Length == 0 ? TypedResults.NotFound() : TypedResults.Ok(results);
    }

    // TODO: Ultimately, I would see this looking at more than just Deployments (Helm charts, ReplicaSets, Rollouts,
    //    or whatever else is set in the values.yaml file for the helm chart.
    public async Task<Results<Ok<TargetVersion[]>, NotFound>> GetVersions()
    {
        // TODO: All these warnings need to be information instead.
        _logger.LogWarning($"Scanning for TargetVersions...");
        var results = await _kubernetesService.GetVersions();
        _logger.LogWarning($"Found {results.Length} versions.");
        
        foreach (var result in results)
        {
            _logger.LogWarning($"Found a thing: {result.Name} - {result.VersionNumber}");
        }
        return results.Length == 0 ? TypedResults.NotFound() : TypedResults.Ok(results);
    }

    public void Dispose()
    {
       _kubernetesService.Dispose(); 
    }
}
