using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Kontroller.API.Kubernetes;
using Kontroller.API.Services;
using Kontroller.API.TargetVersions;

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
        builder.Services.AddSingleton<KubernetesService>();
        builder.Services.AddTransient<TargetVersionEndpointsService>();
        
        return builder.Build();
    }
}

[JsonSerializable(typeof(TargetVersion))]
[JsonSerializable(typeof(TargetVersion[]))]
[JsonSerializable(typeof(List<TargetVersion>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext
{
}

internal static class VersionEndpointExtensions
{
    internal static void MapVersionEndpoints(this WebApplication webApplication)
    {
        var group = webApplication.MapGroup("/versions");
        group.MapGet("/", async (context) =>
        {
            var service = context.RequestServices.GetRequiredService<TargetVersionEndpointsService>();
            await service.GetVersions();
        });
    }
}


// Host terminated unexpectedly:Some services are not able to be constructed (Error while validating the service descriptor 'ServiceType: Kontroller.API.Versions.VersionEndpoint │
//     │    at Microsoft.Extensions.DependencyInjection.ServiceProvider..ctor(ICollection`1, ServiceProviderOptions) + 0x45c                                                            │
//     │    at Microsoft.Extensions.DependencyInjection.ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(IServiceCollection, ServiceProviderOptions) + 0x3c             │
//     │    at Microsoft.Extensions.Hosting.HostApplicationBuilder.Build() + 0x44                                                                                                       │
//     │    at Microsoft.AspNetCore.Builder.WebApplicationBuilder.Build() + 0x94                                                                                                        │
//     │    at Kontroller.API.Program.Main() + 0x14