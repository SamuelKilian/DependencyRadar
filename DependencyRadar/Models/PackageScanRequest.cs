namespace DependencyRadar.Models;

public class PackageScanRequest
{
    //Demande contenant plusieurs packages
    public List<PackageRequest> Packages { get; set; } = new();
}