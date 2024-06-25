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

    // TODO: This should be the aggregate method, calling down to the specific ones (Deployments, Services, Rollouts, etc...) and returning them all.
    public async Task<Results<Ok<List<TargetVersion>>, NotFound>> GetVersions()
    {
        // TODO: All these warnings need to be information instead. Just here for convenience right now.
        _logger.LogWarning($"Scanning for TargetVersions...");
        var results = await _kubernetesService.GetVersions();
        _logger.LogWarning($"Found {results.Count} versions.");
        
        foreach (var result in results)
        {
            _logger.LogWarning($"Found a thing: {result.Name} - {result.VersionNumber}");
        }
        return results.Count == 0 ? TypedResults.NotFound() : TypedResults.Ok(results.ToList());
    }

    public void Dispose()
    {
       _kubernetesService.Dispose(); 
    }
}
