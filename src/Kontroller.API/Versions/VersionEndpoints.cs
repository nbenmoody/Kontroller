using Kontroller.API.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Kontroller.API.Versions;

internal static class VersionEndpoints
{
    internal static List<Version> fakeVersionDb =
    [
        new Version("Deployment 1", "1.1.0"),
        new Version("Deployment 2", "2.1.0"),
        new Version("Deployment 3", "1.4.10"),
        new Version("Deployment 4", "0.13.2"),
    ];

    internal static void RegisterEndpoints(WebApplication app)
    {
        var versions = app.MapGroup("/versions");
        versions.MapGet("/", GetVersions);
    }

    private static Results<Ok<Version[]>, NotFound> GetVersions()
    {
        // kubernetes.Dispose(); // TODO: This is just a placeholder. Obviously do actual things with this.
        
        return fakeVersionDb.Count > 0
            ? TypedResults.Ok(fakeVersionDb.ToArray())
            : TypedResults.NotFound();
    }
}

