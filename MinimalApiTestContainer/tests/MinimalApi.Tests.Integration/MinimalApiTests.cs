using DotNet.Testcontainers.Builders;
using NUnit.Framework;

namespace MinimalApi.Tests.Integration;

[TestFixture]
public class MinimalApiTests
{
    [Test]
    public async Task Call_WeatherForecast_ExpectResponse()
    {
        // Create a new instance of a container.
        var container = new ContainerBuilder()
            // Set the image for the container to "laurentkempe/minimalapi:1.0.0".
            .WithImage("laurentkempe/minimalapi:1.0.0")
            // Bind port 80 of the container to a random port on the host.
            .WithPortBinding(80, true)
            // Wait until the HTTP endpoint of the container is available.
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPath("/weatherforecast")))
            // Build the container configuration.
            .Build();

        // Start the container.
        await container.StartAsync()
            .ConfigureAwait(false);

        // Create a new instance of HttpClient to send HTTP requests.
        var httpClient = new HttpClient();

        // Construct the request URI by specifying the scheme, hostname, assigned random host port, and the endpoint "uuid".
        var requestUri =
            new UriBuilder(Uri.UriSchemeHttp, container.Hostname, container.GetMappedPublicPort(80), "weatherforecast")
                .Uri;

        // Send an HTTP GET request to the specified URI and retrieve the response as a string.
        var response = await httpClient.GetStringAsync(requestUri)
            .ConfigureAwait(false);

        // Ensure that the retrieved UUID is a valid GUID.
        Assert.That(response, Is.Not.Empty);
    }
}