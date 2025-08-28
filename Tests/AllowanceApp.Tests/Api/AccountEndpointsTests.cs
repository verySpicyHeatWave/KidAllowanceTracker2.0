namespace AllowanceApp.Tests.Api
{
    public class AccountEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AccountEndpointsTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }
    }
}