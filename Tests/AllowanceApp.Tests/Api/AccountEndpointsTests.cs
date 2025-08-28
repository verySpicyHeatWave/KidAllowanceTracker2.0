using System.Net;
using System.Net.Http.Json;
using AllowanceApp.Api.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace AllowanceApp.Tests.Api
{
    public class AccountEndpointsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task AddAccount_ReturnsOk_WhenAccountIsCreated()
        {
            string name = Guid.NewGuid().ToString();            
            var response = await _client.PostAsync($"/accounts/create/{name}", null);            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var accountDto = await response.Content.ReadFromJsonAsync<AccountDTO>();
            Assert.NotNull(accountDto);
            Assert.Equal(name, accountDto.Name);
        }

        [Fact]
        public async Task AddAccount_ReturnsProblem_WhenDuplicateAccountName()
        {
            string name = Guid.NewGuid().ToString();

            var firstResponse = await _client.PostAsync($"/accounts/create/{name}", null);
            Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

            var secondResponse = await _client.PostAsync($"/accounts/create/{name}", null);
            Assert.Equal(HttpStatusCode.InternalServerError, secondResponse.StatusCode);
            string problemDetail = await secondResponse.Content.ReadAsStringAsync();
            Assert.Contains("Database Update Exception", problemDetail);
        }
    }
}