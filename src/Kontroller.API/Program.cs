using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Kontroller.API.Todos;

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
            var app = BuildWebHost();
            TodoEndpoints.RegisterEndpoints(app);
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
        var builder = WebApplication.CreateBuilder();

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

        return builder.Build();
    }
}

[JsonSerializable(typeof(Todo))]
[JsonSerializable(typeof(Todo[]))]
[JsonSerializable(typeof(List<Todo>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext
{
}
