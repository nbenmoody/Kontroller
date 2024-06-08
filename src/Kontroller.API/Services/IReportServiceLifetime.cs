namespace Kontroller.API.Services;
using Microsoft.Extensions.DependencyInjection;


public interface IReportServiceLifetime
{
    Guid Id { get; }

    ServiceLifetime Lifetime { get; }
}