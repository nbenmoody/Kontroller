using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Kontroller.API.Models;
using Kontroller.API.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Kontroller.API;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
[ExcludeFromCodeCoverage]
[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
public static class Program
{
    public static int Main()
    {
        try
        {
            // Init
            var app = BuildWebHost();

            // Register
            app.MapHealthChecks("/healthz");
            app.MapVersionEndpoints();
            
            // Run
            Console.WriteLine("006");
            Console.WriteLine("noëlle moody"); // Save. Noëlle's first line of code.
            Console.WriteLine("bunny moody");
            Console.WriteLine($"Running the application as if it's in this env: {app.Environment.EnvironmentName}");
            app.Run();
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Host terminated unexpectedly:" + ex.Message);
            Console.WriteLine(ex.StackTrace);
            return 1;
        }
    }

    private static WebApplication BuildWebHost()
    {
        var builder = WebApplication.CreateSlimBuilder();
        
        // Web host config and settings
        var env = builder.Environment.EnvironmentName;
        builder.WebHost.UseKestrel(options => { options.ListenAnyIP(8080); });
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, SourceGenerationContext.Default);
            options.SerializerOptions.WriteIndented = true;
            options.SerializerOptions.IncludeFields = true;
        });
        builder.Configuration
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{env}.json", true,
                true)
            .AddEnvironmentVariables();
        
        // Configure logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        // DI Ref: https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-usage
        builder.Services.AddHealthChecks();
        builder.Services.AddSingleton<IKubernetesService, KubernetesService>();
        builder.Services.AddSingleton<IKontrollerEndpointsService, KontrollerEndpointsService>();
        
        return builder.Build();
    }
}

[JsonSerializable(typeof(KontrollerService))]
[JsonSerializable(typeof(List<KontrollerService>))]
[JsonSerializable(typeof(KontrollerDeployment))]
[JsonSerializable(typeof(List<KontrollerDeployment>))]
[JsonSerializable(typeof(KontrollerVersion))]
[JsonSerializable(typeof(List<KontrollerVersion>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext
{
}

internal static class VersionEndpointExtensions
{
    
    // TODO: Initially, I just want to get all the object types returning. Then I can refactor into more configurable/intelligent version-detection.
    internal static void MapVersionEndpoints(this WebApplication webApplication)
    {
        var group = webApplication.MapGroup("/k8s");
        
        // TODO: I want to pull from configuration on these calls, to determine namespace, or even regex against object names, and/or labels to match against, things like that.
        group.MapGet("/versions", async Task<Results<Ok<List<KontrollerVersion>>, NotFound>> (IKontrollerEndpointsService service) =>
        {
            var response = await service.GetVersions();
            return response;
        });
        
        group.MapGet("/deployments", async Task<Results<Ok<List<KontrollerDeployment>>, NotFound>> (IKontrollerEndpointsService service) =>
        {
            var response = await service.GetDeployments();
            return response;
        });
        
        group.MapGet("/services", async Task<Results<Ok<List<KontrollerService>>, NotFound>> (IKontrollerEndpointsService service) =>
        {
            var response = await service.GetServices();
            return response;
        });
        
        // TODO: Helm Charts
        group.MapGet("/charts", async Task<Results<Ok<List<KontrollerChart>>, NotFound>> (IKontrollerEndpointsService service) =>
        {
            var response = await service.GetHelmCharts();
            return response;
        });

        // TODO: Argo Rollouts

    }
}