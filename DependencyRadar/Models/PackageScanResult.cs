namespace DependencyRadar.Models;

public class PackageScanResult
{
    public string Package { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public int VulnerabilityCount { get; set; }

    public List<OsvVulnerability> Vulnerabilities { get; set; } = new();

    public List<string> FixedVersions { get; set; } = new();
}