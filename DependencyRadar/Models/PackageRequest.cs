using System.ComponentModel.DataAnnotations;

namespace DependencyRadar.Models;

public class PackageRequest
{
    [Required]
    [MinLength(1)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string Version { get; set; } = string.Empty;
}