using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DFWeb.BE.Tests;

public class MonitorControllerIntegrationTests
{
    [Fact]
    public async Task Ping_ReturnsPong()
    {
        using var host = BuildHost();
        using var client = host.CreateClient();

        var response = await client.GetAsync("/api/Monitor/Ping");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("PONG", body);
    }

    private static TestServer BuildHost()
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddControllers().AddApplicationPart(typeof(DFWeb.BE.Api.MonitorController).Assembly);
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            });

        return new TestServer(builder);
    }
}
