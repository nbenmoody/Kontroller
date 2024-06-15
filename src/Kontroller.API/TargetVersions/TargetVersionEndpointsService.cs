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

    public async Task<Results<Ok<TargetVersion[]>, NotFound>> GetVersions()
    {
        _logger.LogWarning($"Scanning for TargetVersions...");
        var results = await _kubernetesService.GetDeployments();
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
