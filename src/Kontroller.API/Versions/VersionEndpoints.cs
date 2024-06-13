using Kontroller.API.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Kontroller.API.Versions;

internal class VersionEndpoints(IKubernetesService _kubernetesService, ILogger<VersionEndpoints> _logger)
{
    
    
    private readonly List<TargetVersion> _fakeVersionDb =
    [
        new TargetVersion("Deployment 1", "1.1.0"),
        new TargetVersion("Deployment 2", "2.1.0"),
        new TargetVersion("Deployment 3", "1.4.10"),
        new TargetVersion("Deployment 4", "0.13.2"),
    ];

    internal void RegisterEndpoints(WebApplication app)
    {
        var versions = app.MapGroup("/versions");
        versions.MapGet("/", GetVersions);
    }

    private Results<Ok<TargetVersion[]>, NotFound> GetFakeVersions()
    {
        return _fakeVersionDb.Count > 0
            ? TypedResults.Ok(_fakeVersionDb.ToArray())
            : TypedResults.NotFound();
    }

    private async Task<Results<Ok<TargetVersion[]>, NotFound>> GetVersions()
    {
        _logger.LogWarning($"Scanning for TargetVersions...");
        // var results = await _kubernetesService.GetDeployments();
        // _logger.LogInformation($"Found {results.Length} versions.");
        // foreach (var result in results)
        // {
        //     _logger.LogInformation($"{result.Name} - {result.VersionNumber}");
        // }
        // return results.Any() ? TypedResults.Ok(results) : TypedResults.NotFound();
        TargetVersion[] versions = [];
        return TypedResults.Ok(versions);
    }
}
