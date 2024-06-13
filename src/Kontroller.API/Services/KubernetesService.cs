using FluentResults;
using k8s;
using k8s.Models;
using Kontroller.API.Versions;

namespace Kontroller.API.Services;

internal sealed class KubernetesService : IKubernetesService
{
    private readonly ILogger _logger;
    private readonly KubernetesClientConfiguration _config = KubernetesClientConfiguration.InClusterConfig();
    private readonly Kubernetes _client;
    private static readonly string FIRST_VERSION_LABEL = "app.kubernetes.io/version";
    private static readonly string SECOND_VERSION_LABEL = "app/version";
    private static readonly string THIRD_VERSION_LABEL = "version";
        

    public KubernetesService(ILogger<KubernetesService> logger)
    {
        _logger = logger;
        _client = new Kubernetes(_config);
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    private static Result<string> DiscernDeploymentVersion(V1Deployment deployment)
    {
        var metadata = deployment.EnsureMetadata();
        if (metadata is null)
            return Result.Fail("No metadata found for deployment");
        if (!metadata.Labels.Any())
            return Result.Fail("No labels found within the Deployment metadata.");
        if (metadata.Labels.Keys.Contains(FIRST_VERSION_LABEL))
            return Result.Ok(metadata.Labels[FIRST_VERSION_LABEL]);
        if (metadata.Labels.Keys.Contains(SECOND_VERSION_LABEL))
            return Result.Ok(metadata.Labels[SECOND_VERSION_LABEL]);
        if (metadata.Labels.Keys.Contains(THIRD_VERSION_LABEL))
            return Result.Ok(metadata.Labels[THIRD_VERSION_LABEL]);

        return Result.Fail("Not able to discern the version from the deployment");
    }

    public async Task<TargetVersion[]> GetDeployments()
    {
        _logger.LogInformation("Searching for Deployments...");
        var deployments = await _client.ListDeploymentForAllNamespacesAsync();
        _logger.LogInformation($"Found {deployments.Items.Count} Deployments...");
        
        TargetVersion[] versions = [];
        if (deployments.Items.Any())
        {
            foreach (var deployment in deployments.Items)
            {
                _logger.LogInformation($"Found a Deployment: {deployment.Name()}");
                var result = DiscernDeploymentVersion(deployment);
                if (result.IsSuccess)
                {
                    versions.Append(new TargetVersion(deployment.Name(), result.Value));
                }
                else
                {
                    _logger.LogWarning($"Could not discern version for Deployment: {deployment.Name}");
                    _logger.LogWarning($"{result.Errors}");
                }
            }
        }

        return versions;
    }
}