using System.Text.Json;
using FluentResults;
using k8s;
using k8s.Models;
using Kontroller.API.Models;
using Microsoft.VisualBasic;

namespace Kontroller.API.Services;

internal sealed class KubernetesService : IKubernetesService
{
    private readonly ILogger _logger;
    private readonly KubernetesClientConfiguration _config = KubernetesClientConfiguration.InClusterConfig();
    private readonly k8s.Kubernetes _client;
    private JsonSerializerOptions _jsonoptions;
    private const string FIRST_VERSION_LABEL = "app.kubernetes.io/version";
    private const string SECOND_VERSION_LABEL = "app/version";
    private const string THIRD_VERSION_LABEL = "version";

    public KubernetesService(ILogger<IKubernetesService> logger)
    {
        _logger = logger;
        _client = new k8s.Kubernetes(_config);
        _jsonoptions = new JsonSerializerOptions { WriteIndented = true }; 
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    private Result<string> DiscernDeploymentVersion(V1Deployment deployment)
    {
        
        _logger.LogWarning($"Attempting to discern a version for {deployment.Name()}");
        Result<string> result = Result.Fail("Not able to discern the version from the deployment");
        
        var metadata = deployment.EnsureMetadata();
        _logger.LogWarning($"The labels on {deployment.Name()} are...");
        foreach (var label in metadata.Labels)
        {
            _logger.LogWarning($"{label.Key}: {label.Value}");
            
        }
        
        if (metadata is null)
        {
            _logger.LogWarning($"No metadata found for deployment {deployment.Name()}");
            result = Result.Fail("No metadata found for deployment");
        }
        else if (!metadata.Labels.Any())
        {
            _logger.LogWarning("No labels found within the Deployment metadata.");
            result = Result.Fail("No labels found within the Deployment metadata.");
        }
        else if (metadata.Labels.ContainsKey(FIRST_VERSION_LABEL))
        {
            _logger.LogWarning($"Matched {FIRST_VERSION_LABEL}");
            result = Result.Ok(metadata.Labels[FIRST_VERSION_LABEL]);
        }
        else if (metadata.Labels.ContainsKey(SECOND_VERSION_LABEL))
        {
            _logger.LogWarning($"Matched {SECOND_VERSION_LABEL}");
            result = Result.Ok(metadata.Labels[SECOND_VERSION_LABEL]);
        }
        else if (metadata.Labels.ContainsKey(THIRD_VERSION_LABEL))
        {
            _logger.LogWarning($"Matched {THIRD_VERSION_LABEL}");
            result = Result.Ok(metadata.Labels[THIRD_VERSION_LABEL]);
        }

        return result;
    }
    
    public async Task<List<KontrollerService>> GetServices()
    {
        _logger.LogWarning("Searching for Services...");
        var v1Services = await _client.ListServiceForAllNamespacesAsync();
        _logger.LogWarning($"Found {v1Services.Items.Count} Services...");

        var services = new List<KontrollerService>();
        if (v1Services.Items.Any())
        {
            foreach (var service in v1Services.Items)
            {
                _logger.LogWarning($"Found a Service: {service.Name()}");
                services.Add(new KontrollerService(service.Name()));
            }
        }
        else
        {
            _logger.LogWarning("No Services found!");
        }

        _logger.LogWarning($"Returning {services.Count} Services...");
        return services;
    }

    public async Task<List<KontrollerDeployment>> GetDeployments()
    {
        _logger.LogWarning("Searching for Deployments...");
        var v1Deployments = await _client.ListDeploymentForAllNamespacesAsync();
        _logger.LogWarning($"Found {v1Deployments.Items.Count} Deployments...");

        var deployments = new List<KontrollerDeployment>();
        if (v1Deployments.Items.Any())
        {
            foreach (var deployment in v1Deployments.Items)
            {
                _logger.LogWarning($"Found a Deployment: {deployment.Name()}");
                deployments.Add(new KontrollerDeployment(deployment.Name()));
            }
        }
        else
        {
            _logger.LogWarning("No V1Deployments found!");
        }

        _logger.LogWarning($"Returning {deployments.Count} deployments...");
        return deployments;
    }

    public async Task<List<KontrollerVersion>> GetVersions()
    {
        _logger.LogWarning("Searching for Deployments...");
        var deployments = await _client.ListDeploymentForAllNamespacesAsync();
        _logger.LogWarning($"Found {deployments.Items.Count} Deployments...");
        
        // TODO: Gather more than just Deployments here. Helm Charts, ReplicaSets, Services, Etc.
        //  Sort these by some determined precedence.

        var versions = new List<KontrollerVersion>();
        if (deployments.Items.Any())
        {
            foreach (var deployment in deployments.Items)
            {
                _logger.LogWarning($"Found a Deployment: {deployment.Name()}");
                var result = DiscernDeploymentVersion(deployment);
                if (result.IsSuccess)
                {
                    versions.Add(new KontrollerVersion(deployment.Name(), result.Value));
                }
                else
                {
                    _logger.LogWarning($"Could not discern version for Deployment: {deployment.Name()}");
                }
            }
        }

        _logger.LogWarning($"Returning {versions.Count} versions...");
        return versions;
    }

    public async Task<List<KontrollerChart>> GetHelmChartVersions()
    {
        var charts = new List<KontrollerChart>();
        
        // Query for ConfigMaps with a label indicating they are Helm releases
        var configMaps = await _client.ListConfigMapForAllNamespacesAsync(labelSelector: "OWNER=TILLER");

        if (configMaps.Items.Any())
        {
            foreach (var configMap in configMaps.Items)
            {
                Console.WriteLine($"Release Name: {configMap.Metadata.Name}");
                Console.WriteLine($"Namespace: {configMap.Metadata.NamespaceProperty}");
                Console.WriteLine($"Labels: {string.Join(", ", configMap.Metadata.Labels)}");
                Console.WriteLine($"Annotations: {string.Join(", ", configMap.Metadata.Annotations)}");
                Console.WriteLine();
                charts.Add(new KontrollerChart(configMap.Metadata.Name));
            }
        }

        // Query for Secrets with a label indicating they are Helm releases
        var secrets = await _client.ListSecretForAllNamespacesAsync(labelSelector: "owner=helm");

        if (secrets.Items.Any())
        {
            foreach (var secret in secrets.Items)
            {
                Console.WriteLine($"Release Name: {secret.Metadata.Name}");
                Console.WriteLine($"Namespace: {secret.Metadata.NamespaceProperty}");
                Console.WriteLine($"Labels: {string.Join(", ", secret.Metadata.Labels)}");
                Console.WriteLine($"Annotations: {string.Join(", ", secret.Metadata.Annotations)}");
                Console.WriteLine();
                charts.Add(new KontrollerChart(secret.Metadata.Name));
            }
        }

        return charts;
    }
    
}