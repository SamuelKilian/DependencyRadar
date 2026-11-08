namespace DependencyRadar.Models;

public class OsvResponse
{
    public List<OsvVulnerability> Vulns { get; set; } = new();
}

public class OsvVulnerability
{
    public string Id { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string Details { get; set; } = string.Empty;

    public List<OsvAffected> Affected { get; set; } = new();

    public List<OsvReference> References { get; set; } = new();
}

public class OsvAffected
{
    public OsvPackage Package { get; set; } = new();

    public List<OsvRange> Ranges { get; set; } = new();
}

public class OsvPackage
{
    public string Name { get; set; } = string.Empty;

    public string Ecosystem { get; set; } = string.Empty;
}

public class OsvRange
{
    public string Type { get; set; } = string.Empty;

    public List<OsvEvent> Events { get; set; } = new();
}

public class OsvEvent
{
    public string? Introduced { get; set; }

    public string? Fixed { get; set; }

    public string? LastAffected { get; set; }

    public string? Limit { get; set; }
}

public class OsvReference
{
    public string Type { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
}