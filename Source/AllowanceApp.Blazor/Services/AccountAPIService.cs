using System.Net;
using AllowanceApp.Blazor.Models;
using AllowanceApp.Shared.DTO;

namespace AllowanceApp.Blazor.Services;

public class AccountApiService(HttpClient http)
{
    private readonly HttpClient _http = http;
    private List<AccountViewModel> _cachedAccounts = [];
    private bool RefreshAccountsOnNextCall = false;

    public async Task<List<AccountViewModel>> GetAllAccountsAsync(bool forceRefresh = false)
    {
        if (_cachedAccounts.Count == 0 || forceRefresh || RefreshAccountsOnNextCall)
        {
            RefreshAccountsOnNextCall = false;
            var response = await _http.GetAsync("accounts/read/all");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return [];
            }
            response.EnsureSuccessStatusCode();
            var dtos = await response.Content.ReadFromJsonAsync<List<AccountDTO>>();
            _cachedAccounts = [];
            if (dtos is not null)
            {
                foreach (var dto in dtos)
                {
                    _cachedAccounts.Add(new AccountViewModel(dto));
                }
            }
        }
        return _cachedAccounts;
    }

    public async Task<AccountViewModel?> GetAccountAsync(int id, bool forceRefresh = false)
    {
        if (_cachedAccounts.Count == 0 || forceRefresh || RefreshAccountsOnNextCall)
        {
            _cachedAccounts = await GetAllAccountsAsync();
        }
        return _cachedAccounts.SingleOrDefault(a => a.ID == id);
    }

    public async Task<AccountViewModel?> PayoutAllowance(int id)
    {
        var response = await _http.PutAsync($"accounts/update/{id}/transaction/payout", null);
        if (HttpStatusCode.OK != response.StatusCode) { return null; }

        var dto = await response.Content.ReadFromJsonAsync<AccountDTO>();
        if (dto is null) { return null; }

        var model = new AccountViewModel(dto);
        RefreshAccountsOnNextCall = UpdateAccountCacheFailed(model);
        return model;
    }

    public async Task<SinglePointModel?> IncrementPoint(int id, string category)
    {
        var response = await _http.PutAsync($"accounts/update/{id}/points/{category}/increment", null);
        if (HttpStatusCode.OK != response.StatusCode) { return null; }

        var dto = await response.Content.ReadFromJsonAsync<AllowancePointDTO>();
        if (dto is null) { return null; }
        var model = new SinglePointModel(dto);
        RefreshAccountsOnNextCall = !TryUpdateAccountCache(model, id);
        return model;
    }

    public async Task<SinglePointModel?> SetGrade(int id, string category, int value)
    {
        var response = await _http.PutAsJsonAsync($"accounts/update/{id}/points/{category}/setpoints", new PointUpdateRequest(value));
        if (HttpStatusCode.OK != response.StatusCode) { return null; }

        var dto = await response.Content.ReadFromJsonAsync<AllowancePointDTO>();
        if (dto is null) { return null; }
        var model = new SinglePointModel(dto);
        RefreshAccountsOnNextCall = !TryUpdateAccountCache(model, id);
        return model;
    }

    public async Task<AccountViewModel?> RequestTransaction(int id, string action, TransactionRequest transaction)
    {
        var response = await _http.PutAsJsonAsync($"accounts/update/{id}/transaction/{action}", transaction);
        if (HttpStatusCode.OK != response.StatusCode) { return null; }

        var dto = await response.Content.ReadFromJsonAsync<AccountDTO>();
        if (dto is null) { return null; }

        var model = new AccountViewModel(dto);
        RefreshAccountsOnNextCall = UpdateAccountCacheFailed(model);
        return model;
    }

    public async Task<AccountViewModel?> ApproveTransaction(int id, int transaction_id)
    {
        var response = await _http.PutAsJsonAsync($"accounts/update/{id}/transaction/approve", new TransactionStatusUpdateRequest() { TransactionID = transaction_id });
        if (HttpStatusCode.OK != response.StatusCode) { return null; }

        var dto = await response.Content.ReadFromJsonAsync<AccountDTO>();
        if (dto is null) { return null; }

        var model = new AccountViewModel(dto);
        RefreshAccountsOnNextCall = UpdateAccountCacheFailed(model);
        return model;
    }

    public async Task<AccountViewModel?> DeclineTransaction(int id, int transaction_id)
    {
        var response = await _http.PutAsJsonAsync($"accounts/update/{id}/transaction/decline", new TransactionStatusUpdateRequest() { TransactionID = transaction_id });
        if (HttpStatusCode.OK != response.StatusCode) { return null; }

        var dto = await response.Content.ReadFromJsonAsync<AccountDTO>();
        if (dto is null) { return null; }

        var model = new AccountViewModel(dto);
        RefreshAccountsOnNextCall = UpdateAccountCacheFailed(model);
        return model;
    }

    public async Task<AccountViewModel?> SetPoints(int id, string category, int value)
    {
        var response = await _http.PutAsJsonAsync($"/accounts/update/{id}/points/{category}/setpoints", new PointUpdateRequest(value));
        if (HttpStatusCode.OK != response.StatusCode) { return null; }

        var dto = await response.Content.ReadFromJsonAsync<AccountDTO>();
        if (dto is null) { return null; }

        var model = new AccountViewModel(dto);
        RefreshAccountsOnNextCall = UpdateAccountCacheFailed(model);
        return model;
    }

    public async Task<AccountViewModel?> SetPrice(int id, string category, int value)
    {
        var response = await _http.PutAsJsonAsync($"/accounts/update/{id}/points/{category}/setprice", new TransactionRequest(value));
        if (HttpStatusCode.OK != response.StatusCode) { return null; }

        var dto = await response.Content.ReadFromJsonAsync<AccountDTO>();
        if (dto is null) { return null; }

        var model = new AccountViewModel(dto);
        RefreshAccountsOnNextCall = UpdateAccountCacheFailed(model);
        return model;
    }

    public void ClearCache() => _cachedAccounts = [];


    private bool UpdateAccountCacheFailed(AccountViewModel model)
    {
        bool value = true;
        int index = _cachedAccounts.FindIndex(a => a.ID == model.ID);
        if (index >= 0)
        {
            _cachedAccounts[index] = model;
            value = false;
        }
        return value;
    }

    private bool TryUpdateAccountCache(SinglePointModel model, int id)
    {
        int accountIndex = _cachedAccounts.FindIndex(a => a.ID == id);
        if (accountIndex < 0) { return false; }
        var pointIndex = _cachedAccounts[accountIndex].Allowances.PointList.FindIndex(a => a.Category == model.Category);
        if (pointIndex < 0) { return false; }
        _cachedAccounts[accountIndex].Allowances.PointList[pointIndex] = model;

        return true;
    }
}
