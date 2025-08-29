using System.Collections;

namespace AllowanceApp.Tests.ApiFixtures
{
    #region BadIDTestCases
    public record BadIDCase(string Route, string HttpAction, bool NegativeID);

    public class BadIDTestCases : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var routes = new[]
            {
                ("/accounts/read/{id}", "get"),
                ("/accounts/read/{id}/points/GoodBehavior", "get" ),
                ("/accounts/read/{id}/points/transactions", "get" ),
                ("/accounts/update/{id}/points/GoodBehavior/increment", "put" ),
                ("/accounts/update/{id}/points/GoodBehavior/decrement", "put" ),
                ("/accounts/update/{id}/points/GoodBehavior/setprice", "jsonput" ),
                ("/accounts/update/{id}/transaction/payout", "put" ),
                ("/accounts/update/{id}/transaction/deposit", "jsonput" ),
                ("/accounts/update/{id}/transaction/withdrawal", "jsonput" ),
                ("/accounts/delete/{id}", "delete" )
            };

            foreach (var (route, method) in routes)
            {
                yield return new object[] { new BadIDCase(route, method, true) };
                yield return new object[] { new BadIDCase(route, method, false) };
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    #endregion
}
