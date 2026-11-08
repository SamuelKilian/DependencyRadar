using DependencyRadar.Models;
using DependencyRadar.Services;

namespace DependencyRadar.Tests;

public class FixedVersionExtractorTests
{
    [Fact]
    public void Extract_ShouldReturnFixedVersion()
    {
        // Arrange
        var osvResponse = new OsvResponse
        {
            Vulns = new List<OsvVulnerability>
            {
                new OsvVulnerability
                {
                    Affected = new List<OsvAffected>
                    {
                        new OsvAffected
                        {
                            Ranges = new List<OsvRange>
                            {
                                new OsvRange
                                {
                                    Events = new List<OsvEvent>
                                    {
                                        new OsvEvent
                                        {
                                            Introduced = "0"
                                        },
                                        new OsvEvent
                                        {
                                            Fixed = "13.0.1"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var extractor = new FixedVersionExtractor();

        // Act
        var result = extractor.Extract(osvResponse);

        // Assert
        Assert.Contains("13.0.1", result);
        Assert.Single(result);
    }

    [Fact]
    public void Extract_ShouldNotReturnDuplicateVersions()
    {
        var osvResponse = new OsvResponse
        {
            Vulns = new List<OsvVulnerability>
            {
                new OsvVulnerability
                {
                    Affected = new List<OsvAffected>
                    {
                        new OsvAffected
                        {
                            Ranges = new List<OsvRange>
                            {
                                new OsvRange
                                {
                                    Events = new List<OsvEvent>
                                    {
                                        new OsvEvent { Fixed = "13.0.1" },
                                        new OsvEvent { Fixed = "13.0.1" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var extractor = new FixedVersionExtractor();

        var result = extractor.Extract(osvResponse);

        Assert.Single(result);
    }

    [Fact]
    public void Extract_ShouldReturnEmptyListWhenNoFixedVersionExists()
    {
        var osvResponse = new OsvResponse();

        var extractor = new FixedVersionExtractor();

        var result = extractor.Extract(osvResponse);

        Assert.Empty(result);
    }

    [Fact]
    public void Extract_ShouldReturnFixedVersionsFromMultipleVulnerabilities()
    {
        var osvResponse = new OsvResponse
        {
            Vulns = new List<OsvVulnerability>
            {
                new OsvVulnerability
                {
                    Affected = new List<OsvAffected>
                    {
                        new OsvAffected
                        {
                            Ranges = new List<OsvRange>
                            {
                                new OsvRange
                                {
                                    Events = new List<OsvEvent>
                                    {
                                        new OsvEvent { Fixed = "13.0.1" }
                                    }
                                }
                            }
                        }
                    }
                },
                new OsvVulnerability
                {
                    Affected = new List<OsvAffected>
                    {
                        new OsvAffected
                        {
                            Ranges = new List<OsvRange>
                            {
                                new OsvRange
                                {
                                    Events = new List<OsvEvent>
                                    {
                                        new OsvEvent { Fixed = "14.0.0" }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var extractor = new FixedVersionExtractor();

        var result = extractor.Extract(osvResponse);

        Assert.Equal(2, result.Count);
        Assert.Contains("13.0.1", result);
        Assert.Contains("14.0.0", result);
    }
}