using DependencyRadar.Models;

namespace DependencyRadar.Services;

public class OsvService
{
    private readonly HttpClient _httpClient;

    public OsvService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<OsvResponse> CheckPackageAsync(
        PackageRequest package,
        CancellationToken cancellationToken)
    {
        //objet c#
        var osvRequest = new
        
        {
            package = new
            {
                name = package.Name,
                ecosystem = "NuGet"
            },
            version = package.Version
        };
        
        //serialisation c#
        var json = System.Text.Json.JsonSerializer.Serialize(osvRequest);

        //on prepare le json pour l'envoyer dans une requete http
        var content = new StringContent(
            json,
            System.Text.Encoding.UTF8,
            "application/json"
        );

        // on envoie content à OSV et on attend sa réponse
        // on la recupere dans response
        try
        {
            var response = await _httpClient.PostAsync(
                "https://api.osv.dev/v1/query",
                content,
                cancellationToken
            );

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            var osvResponse = System.Text.Json.JsonSerializer.Deserialize<OsvResponse>(
                responseBody,
                new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

            return osvResponse ?? new OsvResponse();
        }
        catch (HttpRequestException ex)
        {
            throw new OsvServiceException(
                "Unable to communicate with the OSV API.",
                ex
            );
        }
        catch (System.Text.Json.JsonException ex)
        {
            throw new OsvServiceException(
                "The OSV API returned an invalid JSON response.",
                ex
            );
        }
        catch (OperationCanceledException ex)
            when (!cancellationToken.IsCancellationRequested)
        {
            throw new OsvTimeoutException(
                "The OSV API request timed out.",
                ex
            );
        }
               
    }
}
