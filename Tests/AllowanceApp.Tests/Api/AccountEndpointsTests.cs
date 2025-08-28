using System.Net;
using System.Net.Http.Json;
using AllowanceApp.Shared.DTO;
using Microsoft.AspNetCore.Mvc.Testing;

// BCOBB: These tests are using the real database, which I don't want.

namespace AllowanceApp.Tests.Api
{
    public class AccountEndpointsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task AddAccount_ReturnsOk_WhenAccountIsCreated()
        {
            string name = Guid.NewGuid().ToString();
            var response = await _client.PostAsJsonAsync($"/accounts/create", new CreateAccountRequest(name));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var accountDto = await response.Content.ReadFromJsonAsync<AccountDTO>();
            Assert.NotNull(accountDto);
            Assert.Equal(name, accountDto.Name);
        }

        [Fact]
        public async Task AddAccount_ReturnsProblem_WhenDuplicateAccountName()
        {
            string name = Guid.NewGuid().ToString();

            var firstResponse = await _client.PostAsJsonAsync($"/accounts/create", new CreateAccountRequest(name));
            Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

            var secondResponse = await _client.PostAsJsonAsync($"/accounts/create", new CreateAccountRequest(name));
            Assert.Equal(HttpStatusCode.InternalServerError, secondResponse.StatusCode);
            string problemDetail = await secondResponse.Content.ReadAsStringAsync();
            Assert.Contains("Database Update Exception", problemDetail);
        }

        [Fact]
        public async Task GetAllAccounts_ReturnsListofAccounts()
        {
            var response = await _client.GetAsync($"/accounts/read/all");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var accountDtos = await response.Content.ReadFromJsonAsync<List<AccountDTO>>();
            Assert.NotNull(accountDtos);
            Console.WriteLine($"Item: {accountDtos}\nCount: {accountDtos.Count}");
        }
    }
}