using Microsoft.AspNetCore.Http.HttpResults;

namespace Kontroller.API.Versions;

internal interface IVersionEndpoints : IDisposable
{
    public Task<Results<Ok<TargetVersion[]>, NotFound>> GetVersions();
}