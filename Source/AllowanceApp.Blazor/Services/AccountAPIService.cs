using System.Net;
using AllowanceApp.Core.Models;
using AllowanceApp.Shared.DTO;

namespace AllowanceApp.Blazor.Services;

public class AccountApiService(HttpClient http)
{
    private readonly HttpClient _http = http;
    private List<AccountDTO>? _cachedAccounts;

    public async Task<List<AccountDTO>> GetAllAccountsAsync(bool forceRefresh = false)
    {
        if (_cachedAccounts is null || forceRefresh)
        {
            _cachedAccounts = await _http.GetFromJsonAsync<List<AccountDTO>>("accounts/read/all")
                              ?? [];
        }
        return _cachedAccounts;
    }

    public async Task<AccountDTO?> GetAccountAsync(int id, bool forceRefresh = false)
    {
        if (_cachedAccounts is null || forceRefresh)
        {
            _cachedAccounts = await _http.GetFromJsonAsync<List<AccountDTO>>("accounts/read/all")
                              ?? [];
        }
        return _cachedAccounts.SingleOrDefault(a => a.ID == id) ?? null;
    }

    public async Task<AccountDTO?> PayoutAllowance(int id)
    {
        var response = await _http.PutAsync($"accounts/update/{id}/transaction/payout", null);
        if (HttpStatusCode.OK == response.StatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AccountDTO>();
        }
        return null;
    }

    public async Task<AllowancePointDTO?> IncrementPoint(int id, string category)
    {
        var response = await _http.PutAsync($"accounts/update/{id}/points/{category}/increment", null);
        if (HttpStatusCode.OK == response.StatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AllowancePointDTO>();
        }
        return null;
    }

    public async Task<AllowancePointDTO?> SetGrade(int id, string category, int value)
    {
        var response = await _http.PutAsJsonAsync($"accounts/update/{id}/points/{category}/setpoints", new PointUpdateRequest(value));
        if (HttpStatusCode.OK != response.StatusCode) { return null; }
        return await response.Content.ReadFromJsonAsync<AllowancePointDTO>();
    }

    public void ClearCache() => _cachedAccounts = null;
}
