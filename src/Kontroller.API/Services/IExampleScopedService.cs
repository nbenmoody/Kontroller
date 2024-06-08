namespace Kontroller.API.Services;

using Microsoft.Extensions.DependencyInjection;

public interface IExampleScopedService : IReportServiceLifetime
{
    ServiceLifetime IReportServiceLifetime.Lifetime => ServiceLifetime.Scoped;
}