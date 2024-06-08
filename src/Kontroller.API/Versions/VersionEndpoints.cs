using Kontroller.API.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Kontroller.API.Versions;

internal class VersionEndpoints(IKubernetesService kubernetesService) // TODO: How do I make use of the DI'd service, here?`
{
    internal static List<TargetVersion> fakeVersionDb =
    [
        new TargetVersion("Deployment 1", "1.1.0"),
        new TargetVersion("Deployment 2", "2.1.0"),
        new TargetVersion("Deployment 3", "1.4.10"),
        new TargetVersion("Deployment 4", "0.13.2"),
    ];

    internal static void RegisterEndpoints(WebApplication app)
    {
        var versions = app.MapGroup("/versions");
        versions.MapGet("/", GetVersions);
    }

    private static Results<Ok<TargetVersion[]>, NotFound> GetVersions()
    {
        
        return fakeVersionDb.Count > 0
            ? TypedResults.Ok(fakeVersionDb.ToArray())
            : TypedResults.NotFound();
    }
}

