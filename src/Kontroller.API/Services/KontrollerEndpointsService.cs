using System.Text.Json;
using Kontroller.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Kontroller.API.Services;

internal sealed class KontrollerEndpointsService : IKontrollerEndpointsService
{
    private readonly ILogger<IKontrollerEndpointsService> _logger;
    private readonly IKubernetesService _kubernetesService;

    public KontrollerEndpointsService(ILogger<IKontrollerEndpointsService> logger, IKubernetesService kubernetesService)
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
    public async Task<Results<Ok<List<KontrollerVersion>>, NotFound>> GetVersions()
    {
        // TODO: All these warnings need to be information instead. Just here for convenience right now.
        _logger.LogWarning($"Scanning for TargetVersions...");
        var results = await _kubernetesService.GetVersions();
        _logger.LogWarning($"Found {results.Count} versions.");
        
        foreach (var result in results)
        {
            _logger.LogWarning($"Found a thing: {result.Name} - {result.VersionNumber}");
        }
        
        // var returnValue = results.Count == 0 
        //     ? TypedResults.NotFound() 
        //     : TypedResults.Ok(results.ToList());

        return TypedResults.Ok(new List<KontrollerVersion>{
            new("test1", "5.4.3"),
            new("test2", "8.7.6")
        });
    }

    public void Dispose()
    {
       _kubernetesService.Dispose(); 
    }
}
