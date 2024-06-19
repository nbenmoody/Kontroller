using Kontroller.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Kontroller.API.TargetVersions;

internal interface ITargetVersionEndpointsService : IDisposable
{
    public Task<Results<Ok<KontrollerDeployment[]>, NotFound>> GetDeployments();
    public Task<Results<Ok<TargetVersion[]>, NotFound>> GetVersions();
}