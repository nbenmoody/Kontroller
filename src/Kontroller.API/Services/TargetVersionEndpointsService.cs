using Kontroller.API.Models;
using Kontroller.API.TargetVersions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Kontroller.API.Services;

internal class TargetVersionEndpointsService : ITargetVersionEndpointsService
{
    private readonly ILogger<ITargetVersionEndpointsService> _logger;
    private readonly IKubernetesService _kubernetesService;

    public TargetVersionEndpointsService(ILogger<ITargetVersionEndpointsService> logger, IKubernetesService kubernetesService)
    {
        _logger = logger;
        _kubernetesService = kubernetesService;
    }

    public async Task<Results<Ok<List<KontrollerDeployment>>, NotFound>> GetDeployments()
    {
        _logger.LogWarning("Scanning for Deployments...");
        var results = await _kubernetesService.GetDeployments();
        _logger.LogWarning($"Found {results.Length} Deployments.");
        
        foreach (var result in results)
        {
            _logger.LogWarning($"Found a thing: {result.Name}");
        }

        return results.Length == 0 ? TypedResults.NotFound() : TypedResults.Ok(results.ToList());
    }

    // TODO: Ultimately, I would see this looking at more than just Deployments (Helm charts, ReplicaSets, Rollouts,
    //    or whatever else is set in the values.yaml file for the helm chart.
    public async Task<Results<Ok<List<TargetVersion>>, NotFound>> GetVersions()
    {
        // TODO: All these warnings need to be information instead.
        _logger.LogWarning($"Scanning for TargetVersions...");
        var results = await _kubernetesService.GetVersions();
        _logger.LogWarning($"Found {results.Count} versions.");
        
        foreach (var result in results)
        {
            _logger.LogWarning($"Found a thing: {result.Name} - {result.VersionNumber}");
        }
        return results.Count == 0 ? TypedResults.NotFound() : TypedResults.Ok(results);
    }

    public void Dispose()
    {
       _kubernetesService.Dispose(); 
    }
}
