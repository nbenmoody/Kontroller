using Kontroller.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Kontroller.API.TargetVersions;

internal interface ITargetVersionEndpointsService : IDisposable
{
    public Task<Results<Ok<List<KontrollerDeployment>>, NotFound>> GetDeployments();
    public Task<Results<Ok<List<TargetVersion>>, NotFound>> GetVersions();
}