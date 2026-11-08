using System.Net;
using DependencyRadar.Models;
using DependencyRadar.Services;

namespace DependencyRadar.Tests;

public class OsvServiceTests
{
    [Fact]
    public async Task CheckPackageAsync_ShouldThrowOsvServiceException_WhenOsvReturns500()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(
            new HttpResponseMessage(HttpStatusCode.InternalServerError)
        );

        var httpClient = new HttpClient(handler);

        var osvService = new OsvService(httpClient);

        var package = new PackageRequest
        {
            Name = "Newtonsoft.Json",
            Version = "9.0.1"
        };

        // Act & Assert
        await Assert.ThrowsAsync<OsvServiceException>(
            () => osvService.CheckPackageAsync(package, CancellationToken.None)
        );
    }

    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;
        private readonly int _delayMilliseconds;

        public FakeHttpMessageHandler(
            HttpResponseMessage response,
            int delayMilliseconds = 0)
        {
            _response = response;
            _delayMilliseconds = delayMilliseconds;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            await Task.Delay(_delayMilliseconds, cancellationToken);

            return _response;
        }
    }

    [Fact]
    public async Task CheckPackageAsync_ShouldThrowOsvServiceException_WhenJsonIsInvalid()
    {
    // Arrange
    var handler = new FakeHttpMessageHandler(
        new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("This is not valid JSON")
        }
    );

    var httpClient = new HttpClient(handler);
    var osvService = new OsvService(httpClient);

    var package = new PackageRequest
    {
        Name = "Newtonsoft.Json",
        Version = "9.0.1"
    };

    // Act & Assert
    await Assert.ThrowsAsync<OsvServiceException>(
        () => osvService.CheckPackageAsync(package, CancellationToken.None)
    );
    }

    [Fact]
    public async Task CheckPackageAsync_ShouldThrowOsvTimeoutException_WhenRequestTimesOut()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(
        new HttpResponseMessage(HttpStatusCode.OK),
        delayMilliseconds: 15000
        );

        var httpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromMilliseconds(100)
        };

        var osvService = new OsvService(httpClient);

        var package = new PackageRequest
        {
            Name = "Newtonsoft.Json",
            Version = "9.0.1"
        };

        // Act & Assert
        await Assert.ThrowsAsync<OsvTimeoutException>(
            () => osvService.CheckPackageAsync(package, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CheckPackageAsync_ShouldPropagateCancellation_WhenClientCancelsRequest()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(
            new HttpResponseMessage(HttpStatusCode.OK),
            delayMilliseconds: 15000
        );

        var httpClient = new HttpClient(handler)
        {
            Timeout = Timeout.InfiniteTimeSpan
        };

        var osvService = new OsvService(httpClient);

        var package = new PackageRequest
        {
            Name = "Newtonsoft.Json",
            Version = "9.0.1"
        };

        using var cts = new CancellationTokenSource();

        // Act
        cts.CancelAfter(100);

        // Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => osvService.CheckPackageAsync(package, cts.Token)
        );
    }
}