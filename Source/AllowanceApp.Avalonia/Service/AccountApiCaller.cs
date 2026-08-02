using AllowanceApp.Avalonia.Models;
using AllowanceApp.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AllowanceApp.Avalonia.Service;

public class AccountApiCaller
{
    private static readonly Lazy<AccountApiCaller> _instance = new(() => new AccountApiCaller());
    private readonly HttpClient _http = new HttpClient();
    private List<Account> _cachedAccounts = [];
    private bool RefreshAccountsOnNextCall = false;
    
    public static AccountApiCaller Instance => _instance.Value;

    private AccountApiCaller()
    {
        _http.BaseAddress = new Uri("http://10.0.0.245:8080/");
    }

    public List<Account> GetAllAccounts(bool forceRefresh = false)
    {
        if (_cachedAccounts.Count == 0 || forceRefresh || RefreshAccountsOnNextCall)
        {
            RefreshAccountsOnNextCall = false;
            var response = _http.Send(new HttpRequestMessage(HttpMethod.Get, "accounts/read/all"));
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return [];
            }
            response.EnsureSuccessStatusCode();
            var dtos = response.Content.ReadFromJsonAsync<List<AccountDTO>>().Result;

            _cachedAccounts = dtos is null
                ? []
                : dtos.Select(dto => new Account(dto)).ToList();
        }
        return _cachedAccounts;
    }

    public async Task<List<Account>> GetAllAccountsAsync(bool forceRefresh = false)
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

            _cachedAccounts = dtos is null
                ? []
                : dtos.Select(dto => new Account(dto)).ToList();
        }

        return _cachedAccounts;
    }

    public async Task<Account?> GetAccountAsync(int id, bool forceRefresh = false)
    {
        if (_cachedAccounts.Count == 0 || forceRefresh || RefreshAccountsOnNextCall)
        {
            await GetAllAccountsAsync();
        }
        return _cachedAccounts.SingleOrDefault(a => a.ID == id);
    }

    public async Task<Account?> PayoutAllowance(int id)
    {
        var response = await _http.PutAsync($"accounts/update/{id}/transaction/payout", null);
        if (HttpStatusCode.OK != response.StatusCode) { return null; }

        var dto = await response.Content.ReadFromJsonAsync<AccountDTO>();
        if (dto is null) { return null; }

        var model = new Account(dto);
        RefreshAccountsOnNextCall = !TryUpdateAccountCache(model);
        return model;
    }

    public async Task<AllowancePoint?> IncrementPoint(int id, string category)
    {
        var response = await _http.PutAsync($"accounts/update/{id}/points/{category}/increment", null);
        if (HttpStatusCode.OK != response.StatusCode) { return null; }

        var dto = await response.Content.ReadFromJsonAsync<AllowancePointDTO>();
        if (dto is null) { return null; }
        var model = new AllowancePoint(dto);
        RefreshAccountsOnNextCall = !TryUpdateAccountCache(model, id);
        return model;
    }

    public async Task<AllowancePoint?> SetGrade(int id, string category, int value)
    {
        var response = await _http.PutAsJsonAsync($"accounts/update/{id}/points/{category}/setpoints", new PointUpdateRequest(value));
        if (HttpStatusCode.OK != response.StatusCode) { return null; }

        var dto = await response.Content.ReadFromJsonAsync<AllowancePointDTO>();
        if (dto is null) { return null; }
        var model = new AllowancePoint(dto);
        RefreshAccountsOnNextCall = !TryUpdateAccountCache(model, id);
        return model;
    }

    public async Task<Account?> RequestTransaction(int id, string action, TransactionRequest transaction)
    {
        var response = await _http.PutAsJsonAsync($"accounts/update/{id}/transaction/{action}", transaction);
        if (HttpStatusCode.OK != response.StatusCode) { return null; }

        var dto = await response.Content.ReadFromJsonAsync<AccountDTO>();
        if (dto is null) { return null; }

        var model = new Account(dto);
        RefreshAccountsOnNextCall = !TryUpdateAccountCache(model);
        return model;
    }

    public async Task<Account?> ApproveTransaction(int id, int transaction_id)
    {
        var response = await _http.PutAsJsonAsync($"accounts/update/{id}/transaction/approve", new TransactionStatusUpdateRequest() { TransactionID = transaction_id });
        if (HttpStatusCode.OK != response.StatusCode) { return null; }

        var dto = await response.Content.ReadFromJsonAsync<AccountDTO>();
        if (dto is null) { return null; }

        var model = new Account(dto);
        RefreshAccountsOnNextCall = !TryUpdateAccountCache(model);
        return model;
    }

    public async Task<Account?> DeclineTransaction(int id, int transaction_id)
    {
        var response = await _http.PutAsJsonAsync($"accounts/update/{id}/transaction/decline", new TransactionStatusUpdateRequest() { TransactionID = transaction_id });
        if (HttpStatusCode.OK != response.StatusCode) { return null; }

        var dto = await response.Content.ReadFromJsonAsync<AccountDTO>();
        if (dto is null) { return null; }

        var model = new Account(dto);
        RefreshAccountsOnNextCall = !TryUpdateAccountCache(model);
        return model;
    }

    public async Task<Account?> SetPoints(int id, string category, int value)
    {
        var response = await _http.PutAsJsonAsync($"/accounts/update/{id}/points/{category}/setpoints", new PointUpdateRequest(value));
        if (HttpStatusCode.OK != response.StatusCode) { return null; }

        var dto = await response.Content.ReadFromJsonAsync<AccountDTO>();
        if (dto is null) { return null; }

        var model = new Account(dto);
        RefreshAccountsOnNextCall = !TryUpdateAccountCache(model);
        return model;
    }

    public async Task<Account?> SetPrice(int id, string category, int value)
    {
        var response = await _http.PutAsJsonAsync($"/accounts/update/{id}/points/{category}/setprice", new TransactionRequest(value));
        if (HttpStatusCode.OK != response.StatusCode) { return null; }

        var dto = await response.Content.ReadFromJsonAsync<AccountDTO>();
        if (dto is null) { return null; }

        var model = new Account(dto);
        RefreshAccountsOnNextCall = !TryUpdateAccountCache(model);
        return model;
    }

    public void ClearCache() => _cachedAccounts = [];


    private bool TryUpdateAccountCache(Account model)
    {
        bool value = false;
        int index = _cachedAccounts.FindIndex(a => a.ID == model.ID);
        if (index >= 0)
        {
            _cachedAccounts[index] = model;
            value = true;
        }
        return value;
    }

    private bool TryUpdateAccountCache(AllowancePoint model, int id)
    {
        int accountIndex = _cachedAccounts.FindIndex(a => a.ID == id);
        if (accountIndex < 0) { return false; }
        var pointIndex = _cachedAccounts[accountIndex].AllowancePoints.FindIndex(a => a.Category == model.Category);
        if (pointIndex < 0) { return false; }
        _cachedAccounts[accountIndex].AllowancePoints[pointIndex] = model;

        return true;
    }
}