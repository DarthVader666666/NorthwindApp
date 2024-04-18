using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace Northwind.IntegrationTests
{
    public class EndPointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public EndPointTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/Employees/Index")]
        [InlineData("/Categories/Index")]
        public async Task EndpointIndex_Test(string url)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content);
        }

        private async Task<HttpClient> GetGuestHttpClientAsync()
        {
            var client = _factory.CreateClient();
            await client.GetAsync("/Guest/Register");

            return client;
        }
    }
}
