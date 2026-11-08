using DependencyRadar.Models;
using DependencyRadar.Services;

var builder = WebApplication.CreateBuilder(args);


// Register application services
builder.Services.AddOpenApi();
builder.Services.AddHttpClient<OsvService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10); // Set a timeout for the HTTP client
});

builder.Services.AddSingleton<FixedVersionExtractor>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Health check endpoint
app.MapGet("/api/health", () =>
{
    return Results.Ok(new
    {
        status = "Healthy",
        application = "Dependency Radar"
    });
})
.WithName("GetHealth");

// Receive a list of packages
app.MapPost("/api/scans", async (PackageScanRequest request, OsvService osvService, CancellationToken cancellationToken, FixedVersionExtractor fixedVersionExtractor) =>
{
    // Check if the package list is empty
    if (request.Packages is null || request.Packages.Count == 0)
    {
        return Results.BadRequest(new
        {
            error = "At least one package is required."
        });
    }

    if (request.Packages.Count > 50)
    {
        return Results.BadRequest(new
        {
            error = "A maximum of 50 packages is allowed per scan."
        });
    }

    // Validate each package
    List<string> errors = new();

    for (int i = 0; i < request.Packages.Count; i++)
    {
        var package = request.Packages[i];

        if (package is null)
        {
            errors.Add($"Package at index {i} cannot be null.");
            continue;
        }

        if (string.IsNullOrWhiteSpace(package.Name))
        {
            errors.Add($"Package at index {i}: Name is required.");
        }

        if (string.IsNullOrWhiteSpace(package.Version))
        {
            errors.Add($"Package at index {i}: Version is required.");
        }
    }

    // Return validation errors if any
    if (errors.Count > 0)
    {
        return Results.BadRequest(new
        {
            errors
        });
    }

    // Return the received packages

    var results = new List<PackageScanResult>();

    try
    {
        foreach (var package in request.Packages)
        {
            var osvResponse = await osvService.CheckPackageAsync(package, cancellationToken);

            results.Add(new PackageScanResult
            {
                Package = package.Name,
                Version = package.Version,
                VulnerabilityCount = osvResponse.Vulns.Count,
                Vulnerabilities = osvResponse.Vulns,
                FixedVersions = fixedVersionExtractor.Extract(osvResponse),
            });
        }

        return Results.Ok(results);
    }
    catch (OsvServiceException ex)
    {
        return Results.Problem(
            title: "External service error",
            detail: ex.Message,
            statusCode: StatusCodes.Status502BadGateway
        );
    }
    catch (OsvTimeoutException ex)
    {
        return Results.Problem(
            title: "External service timeout",
            detail: ex.Message,
            statusCode: StatusCodes.Status504GatewayTimeout
        );
    }
})
.WithName("CreateScan");

app.Run();