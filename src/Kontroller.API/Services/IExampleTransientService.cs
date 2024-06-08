namespace Kontroller.API.Services;

using Microsoft.Extensions.DependencyInjection;

public interface IExampleTransientService : IReportServiceLifetime
{
    ServiceLifetime IReportServiceLifetime.Lifetime => ServiceLifetime.Transient;
}