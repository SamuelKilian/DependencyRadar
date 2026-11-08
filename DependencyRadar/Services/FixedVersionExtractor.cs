using DependencyRadar.Models;

namespace DependencyRadar.Services;

public class FixedVersionExtractor
{
    public List<string> Extract(OsvResponse osvResponse)
    {
        var fixedVersions = new List<string>();

        foreach (var vulnerability in osvResponse.Vulns)
        {
            foreach (var affected in vulnerability.Affected)
            {
                foreach (var range in affected.Ranges)
                {
                    foreach (var osvEvent in range.Events)
                    {
                        if (!string.IsNullOrWhiteSpace(osvEvent.Fixed))
                        {
                            fixedVersions.Add(osvEvent.Fixed);
                        }
                    }
                }
            }
        }

        return fixedVersions.Distinct().ToList();
    }
}