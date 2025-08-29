using System.Net;
using System.Net.Http.Json;
using AllowanceApp.Core.Models;
using AllowanceApp.Shared.DTO;
using AllowanceApp.Tests.Common;
using Microsoft.AspNetCore.Mvc.Testing;

// BCOBB: These tests are using the real database, which I don't want.

namespace AllowanceApp.Tests.Api
{
    public class AccountEndpointsTests(MockContextWebAppFactory factory) : IClassFixture<MockContextWebAppFactory>, IAsyncLifetime
    {
        private readonly MockContextWebAppFactory _factory = factory;
        private readonly HttpClient _client = factory.CreateClient();

        public async Task InitializeAsync()
        {
            _factory.ResetDatabase();
            _factory.SeedDatabase(db =>
            {
                db.Accounts.Add(new Account("Adam"));
                db.Accounts.Add(new Account("Beth"));
                db.Accounts.Add(new Account("Carl"));
                db.Accounts.Add(new Account("Dave"));
                db.Accounts.Add(new Account("Erin"));
            });
            await Task.CompletedTask;
        }

        public async Task DisposeAsync() => 
            await Task.CompletedTask;

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