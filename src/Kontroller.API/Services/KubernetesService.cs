using FluentResults;
using k8s;
using k8s.Models;
using Kontroller.API.Models;
using Kontroller.API.Services;
using Kontroller.API.TargetVersions;

namespace Kontroller.API.Kubernetes;

internal sealed class KubernetesService : IKubernetesService
{
    private readonly ILogger _logger;
    private readonly KubernetesClientConfiguration _config = KubernetesClientConfiguration.InClusterConfig();
    private readonly k8s.Kubernetes _client;
    private const string FIRST_VERSION_LABEL = "app.kubernetes.io/version";
    private const string SECOND_VERSION_LABEL = "app/version";
    private const string THIRD_VERSION_LABEL = "version";

    public KubernetesService(ILogger<IKubernetesService> logger)
    {
        _logger = logger;
        _client = new k8s.Kubernetes(_config);
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    private static Result<string> DiscernDeploymentVersion(V1Deployment deployment)
    {
        Result<string> result;
        
        var metadata = deployment.EnsureMetadata();
        if (metadata is null)
            result = Result.Fail("No metadata found for deployment");
        else if (!metadata.Labels.Any())
            result = Result.Fail("No labels found within the Deployment metadata.");
        else if (metadata.Labels.ContainsKey(FIRST_VERSION_LABEL))
            result = Result.Ok(metadata.Labels[FIRST_VERSION_LABEL]);
        else if (metadata.Labels.ContainsKey(SECOND_VERSION_LABEL))
            result = Result.Ok(metadata.Labels[SECOND_VERSION_LABEL]);
        else if (metadata.Labels.ContainsKey(THIRD_VERSION_LABEL))
            result = Result.Ok(metadata.Labels[THIRD_VERSION_LABEL]);

        result = Result.Fail("Not able to discern the version from the deployment");
        return result;
    }

    public async Task<KontrollerDeployment[]> GetDeployments()
    {
        _logger.LogWarning("Searching for Deployments...");
        var v1deployments = await _client.ListDeploymentForAllNamespacesAsync();
        _logger.LogInformation($"Found {v1deployments.Items.Count} Deployments...");
        
        KontrollerDeployment[] deployments = [];
        if (v1deployments.Items.Any())
        {
            foreach (var deployment in v1deployments)
            {
                deployments.Append(new KontrollerDeployment(deployment));
            }
        }
        else
        {
            _logger.LogWarning("No V1Deployments found!");
        }

        return deployments;
    }

    public async Task<TargetVersion[]> GetVersions()
    {
        _logger.LogInformation("Searching for Deployments...");
        var deployments = await _client.ListDeploymentForAllNamespacesAsync();
        _logger.LogInformation($"Found {deployments.Items.Count} Deployments...");
        
        // TODO: Gather more than just Deployments here. Helm Charts, ReplicaSets, Services, Etc.
        //  Sort these by some determined precedence.
        
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