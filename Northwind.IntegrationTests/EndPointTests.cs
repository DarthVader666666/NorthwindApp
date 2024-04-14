using Microsoft.AspNetCore.Mvc.Testing;
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
        [InlineData("/GuestEmployees/Index")]
        [InlineData("/GuestCategories/Index")]
        public async Task EndpointIndex_Test(string url)
        {
            var client = await GetGuestHttpClientAsync();

            var response = await client.GetAsync(url);

            Assert.True(response.IsSuccessStatusCode);
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
