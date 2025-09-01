using System.Net;
using System.Net.Http.Json;
using AllowanceApp.Core.Models;
using AllowanceApp.Shared.DTO;
using AllowanceApp.Tests.Common;
using AllowanceApp.Tests.ApiFixtures;

namespace AllowanceApp.Tests.Api
{
    public class AccountEndpointsTests(MockContextWebAppFactory factory) : IClassFixture<MockContextWebAppFactory>, IAsyncLifetime
    {
        #region TestUtilityCrap
        private readonly MockContextWebAppFactory _factory = factory;
        private readonly HttpClient _client = factory.CreateClient();

        private void DefaultDBSeed()
        {
            _factory.SeedDatabase(db =>
            {
                db.Accounts.Add(Methods.GetLivelyAccount("Adam"));
                db.Accounts.Add(Methods.GetLivelyAccount("Beth"));
                db.Accounts.Add(Methods.GetLivelyAccount("Carl"));
                db.Accounts.Add(Methods.GetLivelyAccount("Dave"));
                db.Accounts.Add(Methods.GetLivelyAccount("Erin"));
            });
        }

        public async Task InitializeAsync()
        {
            _factory.ResetDatabase();
            await Task.CompletedTask;
        }

        public async Task DisposeAsync() =>
            await Task.CompletedTask;
        #endregion


        #region MultipleEndpoints
        [Theory]
        [ClassData(typeof(BadIDTestCases))]
        public async Task MultipleEndpoints_ReturnsNotFound_WhenNonexistantID(BadIDCase test_case)
        {
            DefaultDBSeed();
            var rng = Methods.GetRandomGenerator();
            var id = test_case.NegativeID
                ? (rng.Next(1, 6) * -1)
                : rng.Next(100, 1000);

            HttpResponseMessage? response;
            switch (test_case.HttpAction)
            {
                case "get":
                    response = await _client.GetAsync(test_case.Route.Replace("{id}", id.ToString()));
                    break;

                case "put":
                    response = await _client.PutAsync(test_case.Route.Replace("{id}", id.ToString()), null);
                    break;

                case "jsonput":
                    var amount = rng.Next(5000);
                    response = await _client.PutAsJsonAsync(
                        test_case.Route.Replace("{id}", id.ToString()),
                        new TransactionRequest(amount, Guid.NewGuid().ToString())
                    );
                    break;

                default:
                    response = await _client.DeleteAsync(test_case.Route.Replace("{id}", id.ToString()));
                    break;

            }
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            string error_string = await response.Content.ReadAsStringAsync();
            Assert.Contains("No account found with ID number", error_string);
        }


        [Theory]
        [InlineData("increment", 1)]
        [InlineData("decrement", -1)]
        public async Task MultipleEndpoints_IncOrDecReturnsValidPoint(string operation, int difference)
        {
            DefaultDBSeed();
            var rng = Methods.GetRandomGenerator();
            var id = rng.Next(1, 6);

            var category = Methods.GetRandomBehaviorString(rng);
            var acct_response = await _client.GetAsync($"/accounts/read/{id}");
            Assert.Equal(HttpStatusCode.OK, acct_response.StatusCode);

            var acct_before = await acct_response.Content.ReadFromJsonAsync<AccountDTO>();
            Assert.NotNull(acct_before);

            var old_pointDTO = acct_before.Allowances
                .SingleOrDefault(a => a.Category == category);
            Assert.NotNull(old_pointDTO);

            var update_response = await _client.PutAsync($"/accounts/update/{id}/points/{category}/{operation}", null);
            Assert.Equal(HttpStatusCode.OK, update_response.StatusCode);
            var pointDTO = await update_response.Content.ReadFromJsonAsync<AllowancePointDTO>();

            Assert.NotNull(pointDTO);
            Assert.Equal(id, pointDTO.AccountID);
            Assert.Equal(category, pointDTO.Category);
            Assert.Equal(old_pointDTO.Points + difference, pointDTO.Points);
        }


        [Theory]
        [InlineData("deposit", 1)]
        [InlineData("withdrawal", -1)]
        public async Task MultipleEndpoints_Transaction_ReturnsValidPoint(string operation, int multiplier)
        {
            DefaultDBSeed();
            var rng = Methods.GetRandomGenerator();
            var id = rng.Next(1, 6);

            var acct_response = await _client.GetAsync($"/accounts/read/{id}");
            Assert.Equal(HttpStatusCode.OK, acct_response.StatusCode);
            var old_account = await acct_response.Content.ReadFromJsonAsync<AccountDTO>();
            Assert.NotNull(old_account);

            var amount = rng.Next(1, 2000);
            var description = Guid.NewGuid().ToString();

            var response = await _client.PutAsJsonAsync(
                $"/accounts/update/{id}/transaction/{operation}",
                new TransactionRequest(amount, description)
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var account = await response.Content.ReadFromJsonAsync<AccountDTO>();
            Assert.NotNull(account);

            var diff = amount * multiplier;
            Assert.Equal(Math.Max(0, old_account.Balance + diff), account.Balance);
            var transaction = account.Transactions.SingleOrDefault(t => string.Equals(t.Description, description, StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(transaction);
            int expected_amount = amount > old_account.Balance && multiplier == -1
                ? old_account.Balance
                : amount;
            Assert.Equal(expected_amount * multiplier, transaction.Amount);
        }


        [Theory]
        [InlineData("deposit", 0)]
        [InlineData("withdrawal", 0)]
        [InlineData("deposit", -1)]
        [InlineData("withdrawal", -1)]
        public async Task MultipleEndpoints_Transaction_BadRequestOnBadAmount(string operation, int multiplier)
        {
            DefaultDBSeed();
            var rng = Methods.GetRandomGenerator();
            var id = rng.Next(1, 6);

            var amount = rng.Next(1, 5000) * multiplier;
            var description = Guid.NewGuid().ToString();

            var response = await _client.PutAsJsonAsync(
                $"/accounts/update/{id}/transaction/{operation}",
                new TransactionRequest(amount, description)
            );
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            string error_string = await response.Content.ReadAsStringAsync();
            Assert.Contains("amount must be more than zero", error_string);
        }
        #endregion


        #region AddAccount
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
        #endregion


        #region GetAllAccounts
        [Fact]
        public async Task GetAllAccounts_ReturnsListofAccounts()
        {
            DefaultDBSeed();
            var response = await _client.GetAsync($"/accounts/read/all");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var accountDtos = await response.Content.ReadFromJsonAsync<List<AccountDTO>>();
            Assert.NotNull(accountDtos);
            Assert.Equal(5, accountDtos.Count);
        }
        #endregion


        #region GetOneAccount
        [Fact]
        public async Task GetOneAccount_ReturnsValidAccount()
        {
            DefaultDBSeed();
            var rng = Methods.GetRandomGenerator();
            var id = rng.Next(1, 6);
            var response = await _client.GetAsync($"/accounts/read/{id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var accountDto = await response.Content.ReadFromJsonAsync<AccountDTO>();
            Assert.NotNull(accountDto);
            Assert.Equal(id, accountDto.ID);
        }
        #endregion


        #region GetAllowancePoint
        [Fact]
        public async Task GetAllowancePoint_ReturnsValidPoint()
        {
            DefaultDBSeed();
            var rng = Methods.GetRandomGenerator();
            var id = rng.Next(1, 6);

            var category = Methods.GetRandomBehaviorString(rng);
            var response = await _client.GetAsync($"/accounts/read/{id}/points/{category}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var pointDTO = await response.Content.ReadFromJsonAsync<AllowancePointDTO>();
            Assert.NotNull(pointDTO);
            Assert.Equal(id, pointDTO.AccountID);
            Assert.Equal(category, pointDTO.Category);
        }
        #endregion


        #region GetAccountTransactions
        [Fact]
        public async Task GetAccountTransactions_ReturnsValidList()
        {
            DefaultDBSeed();
            var rng = Methods.GetRandomGenerator();
            var id = rng.Next(1, 6);

            var category = Methods.GetRandomBehaviorString(rng);
            var response = await _client.GetAsync($"/accounts/read/{id}/transactions");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var transactionDTOs = await response.Content.ReadFromJsonAsync<List<TransactionDTO>>();
            Assert.NotNull(transactionDTOs);
            Assert.True(transactionDTOs.Count > 1);
            Assert.Equal(id, transactionDTOs[0].AccountID);
        }
        #endregion


        #region SetAllowancePrice

        [Fact]
        public async Task SetAllowancePrice_ReturnsValidPoint()
        {
            DefaultDBSeed();
            var rng = Methods.GetRandomGenerator();
            var id = rng.Next(1, 6);
            var amount = rng.Next(-500, 500);

            var category = Methods.GetRandomBehaviorString(rng);

            var response = await _client.PutAsJsonAsync($"/accounts/update/{id}/points/{category}/setprice", new TransactionRequest(amount, null));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var pointDTO = await response.Content.ReadFromJsonAsync<AllowancePointDTO>();

            Assert.NotNull(pointDTO);
            Assert.Equal(id, pointDTO.AccountID);
            Assert.Equal(category, pointDTO.Category);
            Assert.Equal(amount, pointDTO.Price);
        }
        #endregion


        #region PayAllowance

        [Fact]
        public async Task PayAllowance_ReturnsValidAccount()
        {
            DefaultDBSeed();
            var rng = Methods.GetRandomGenerator();
            var id = rng.Next(1, 6);
            var old_response = await _client.GetAsync($"/accounts/read/{id}");
            Assert.Equal(HttpStatusCode.OK, old_response.StatusCode);

            var old_account = await old_response.Content.ReadFromJsonAsync<AccountDTO>();
            Assert.NotNull(old_account);

            var response = await _client.PutAsync($"/accounts/update/{id}/transaction/payout", null);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var account = await response.Content.ReadFromJsonAsync<AccountDTO>();
            Assert.NotNull(account);

            Assert.Equal(old_account.Balance + old_account.AllowanceTotal, account.Balance);
            Assert.Equal(0, account.AllowanceTotal);
        }
        #endregion


        #region DeleteAccount

        [Fact]
        public async Task DeleteAccount_ReturnsValidName()
        {
            DefaultDBSeed();
            List<string> account_names = ["Adam", "Beth", "Carl", "Dave", "Erin"];
            var rng = Methods.GetRandomGenerator();
            var id = rng.Next(1, 6);
            var response = await _client.DeleteAsync($"/accounts/delete/{id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var message = await response.Content.ReadAsStringAsync();
            Assert.NotNull(message);
            Assert.Contains("successfully deleted", message);

            bool found = false;
            foreach (var name in account_names)
            {
                if (message.Contains(name))
                {
                    found = true;
                    break;
                }
            }

            Assert.True(found);
        }
        #endregion
    }
}