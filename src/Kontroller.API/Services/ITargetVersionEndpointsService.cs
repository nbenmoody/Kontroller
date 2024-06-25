using Kontroller.API.Models;
using Kontroller.API.TargetVersions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Kontroller.API.Services;

internal interface ITargetVersionEndpointsService : IDisposable
{
    public Task<Results<Ok<List<TargetVersion>>, NotFound>> GetVersions();
    public Task<Results<Ok<List<KontrollerDeployment>>, NotFound>> GetDeployments();
}