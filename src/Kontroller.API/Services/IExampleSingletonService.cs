namespace Kontroller.API.Services;

using Microsoft.Extensions.DependencyInjection;

public interface IExampleSingletonService : IReportServiceLifetime
{
    ServiceLifetime IReportServiceLifetime.Lifetime => ServiceLifetime.Singleton;
}