using Kontroller.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Kontroller.API.Services;

internal interface IKontrollerEndpointsService : IDisposable
{
    public Task<Results<Ok<List<KontrollerChart>>, NotFound>> GetHelmCharts();
    public Task<Results<Ok<List<KontrollerVersion>>, NotFound>> GetVersions();
    public Task<Results<Ok<List<KontrollerDeployment>>, NotFound>> GetDeployments();

    public Task<Results<Ok<List<KontrollerService>>, NotFound>> GetServices();
}