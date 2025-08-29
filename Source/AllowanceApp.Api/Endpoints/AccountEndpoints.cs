using AllowanceApp.Api.Results;
using AllowanceApp.Core.Models;
using AllowanceApp.Core.Services;
using AllowanceApp.Data.Exceptions;
using AllowanceApp.Shared.DTO;
using AllowanceApp.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AllowanceApp.Api.Endpoints
{
    public static class AccountEndpoints
    {
        public static void SetAccountCreateEndpoints(this WebApplication app)
        {
            app.MapPost("/accounts/create", async (CreateAccountRequest request, AccountService accountService) =>
            {
                var dbResult = await TryDatabaseInteraction<Account>(async () =>
                {
                    var account = await accountService.AddAccountAsync(request.Name);
                    return account;
                });
                return dbResult.IsSuccess
                    ? Microsoft.AspNetCore.Http.Results.Ok(new AccountDTO(dbResult.Response!))
                    : Microsoft.AspNetCore.Http.Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("AddAccount")
            .WithOpenApi();
        }

        public static void SetAccountReadEndpoints(this WebApplication app)
        {
            app.MapGet("/accounts/read/all", async (AccountService accountService) =>
            {
                var dbResult = await TryDatabaseInteraction<List<Account>>(async () =>
                {
                    var accounts = await accountService.GetAllAccountsAsync();
                    return accounts;
                });
                return dbResult.IsSuccess
                    ? Microsoft.AspNetCore.Http.Results.Ok(DTOUtility.AccountListToDTOs(dbResult.Response!))
                    : Microsoft.AspNetCore.Http.Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("GetAllAccounts")
            .WithOpenApi();

            app.MapGet("/accounts/read/{id}", async (int id, AccountService accountService) =>
            {
                var dbResult = await TryDatabaseInteraction<Account>(async () =>
                {
                    var account = await accountService.GetAccountAsync(id);
                    return account;
                });
                return dbResult.IsSuccess
                    ? Microsoft.AspNetCore.Http.Results.Ok(new AccountDTO(dbResult.Response!))
                    : Microsoft.AspNetCore.Http.Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("GetOneAccount")
            .WithOpenApi();

            app.MapGet("/accounts/read/{id}/points/{category}", async (int id, string category, AccountService accountService) =>
            {
                var dbResult = await TryDatabaseInteraction<AllowancePoint>(async () =>
                {
                    var point = await accountService.GetAllowancePointAsync(id, category);
                    return point;
                });
                return dbResult.IsSuccess
                    ? Microsoft.AspNetCore.Http.Results.Ok(new AllowancePointDTO(dbResult.Response!))
                    : Microsoft.AspNetCore.Http.Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("GetAllowancePoint")
            .WithOpenApi();

            app.MapGet("/accounts/read/{id}/transactions", async (int id, AccountService accountService) =>
            {
                var dbResult = await TryDatabaseInteraction<List<Transaction>>(async () =>
                {
                    var account = await accountService.GetAccountAsync(id);
                    return account.Transactions;
                });
                return dbResult.IsSuccess && dbResult.Response!.Count != 0
                    ? Microsoft.AspNetCore.Http.Results.Ok(DTOUtility.TransactionListToDTOs(dbResult.Response))
                    : Microsoft.AspNetCore.Http.Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("GetAccountTransactions")
            .WithOpenApi();
        }

        public static void SetAccountUpdateEndpoints(this WebApplication app)
        {
            app.MapPut("/accounts/update/{id}/points/{category}/increment", async (int id, string category, AccountService accountService) =>
            {

                var dbResult = await TryDatabaseInteraction<AllowancePoint>(async () =>
                {
                    var point = await accountService.SinglePointAdjustAsync(id, category, PointOperation.Increment);
                    return point;
                });
                return dbResult.IsSuccess
                    ? Microsoft.AspNetCore.Http.Results.Ok(new AllowancePointDTO(dbResult.Response!))
                    : Microsoft.AspNetCore.Http.Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("IncrementPoint")
            .WithOpenApi();

            app.MapPut("/accounts/update/{id}/points/{category}/decrement", async (int id, string category, AccountService accountService) =>
            {
                var dbResult = await TryDatabaseInteraction<AllowancePoint>(async () =>
                {
                    var point = await accountService.SinglePointAdjustAsync(id, category, PointOperation.Decrement);
                    return point;
                });
                return dbResult.IsSuccess
                    ? Microsoft.AspNetCore.Http.Results.Ok(new AllowancePointDTO(dbResult.Response!))
                    : Microsoft.AspNetCore.Http.Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("DecrementPoint")
            .WithOpenApi();

            app.MapPut("/accounts/update/{id}/points/{category}/setprice", async (int id, string category, TransactionRequest request, AccountService accountService) =>
            {
                var dbResult = await TryDatabaseInteraction<AllowancePoint>(async () =>
                {
                    var point = await accountService.UpdateAllowancePriceAsync(id, category, request.Amount);
                    return point;
                });
                return dbResult.IsSuccess
                    ? Microsoft.AspNetCore.Http.Results.Ok(new AllowancePointDTO(dbResult.Response!))
                    : Microsoft.AspNetCore.Http.Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("SetAllowancePrice")
            .WithOpenApi();

            app.MapPut("/accounts/update/{id}/transaction/payout", async (int id, AccountService accountService) =>
            {
                var dbResult = await TryDatabaseInteraction<Account>(async () =>
                {
                    var account = await accountService.PayAllowanceAsync(id);
                    return account;
                });
                return dbResult.IsSuccess
                    ? Microsoft.AspNetCore.Http.Results.Ok(new AccountDTO(dbResult.Response!))
                    : Microsoft.AspNetCore.Http.Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("PayAllowance")
            .WithOpenApi();

            app.MapPut("/accounts/update/{id}/transaction/deposit", async (int id, TransactionRequest request, AccountService accountService) =>
            {
                if (request.Amount <= 0)
                {
                    return Microsoft.AspNetCore.Http.Results.Problem(
                        detail: $"Deposit amount must be more than zero, not {request.Amount}",
                        statusCode: StatusCodes.Status400BadRequest
                    );
                }
                var dbResult = await TryDatabaseInteraction<Account>(async () =>
                {
                    var account = await accountService.ApplyTransactionAsync(id, request.Amount, TransactionType.Deposit, request.Description);
                    return account;
                });
                return dbResult.IsSuccess
                    ? Microsoft.AspNetCore.Http.Results.Ok(new AccountDTO(dbResult.Response!))
                    : Microsoft.AspNetCore.Http.Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("DepositIntoAccount")
            .WithOpenApi();

            app.MapPut("/accounts/update/{id}/transaction/withdrawal", async (int id, TransactionRequest request, AccountService accountService) =>
            {
                if (request.Amount <= 0)
                {
                    return Microsoft.AspNetCore.Http.Results.Problem(
                        detail: $"Withdrawal amount must be more than zero, not {request.Amount}",
                        statusCode: StatusCodes.Status400BadRequest
                    );
                }
                var dbResult = await TryDatabaseInteraction<Account>(async () =>
                {
                    var account = await accountService.ApplyTransactionAsync(id, request.Amount, TransactionType.Withdraw, request.Description);
                    return account;
                });
                return dbResult.IsSuccess
                    ? Microsoft.AspNetCore.Http.Results.Ok(new AccountDTO(dbResult.Response!))
                    : Microsoft.AspNetCore.Http.Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("WithdrawFromAccount")
            .WithOpenApi();
        }

        public static void SetAccountDeleteEndpoints(this WebApplication app)
        {
            app.MapDelete("/accounts/delete/{id}", async (int id, AccountService accountService) =>
            {
                var dbResult = await TryDatabaseInteraction<string>(async () =>
                {
                    var name = await accountService.DeleteAccountAsync(id);
                    return name;
                });
                return dbResult.IsSuccess
                    ? Microsoft.AspNetCore.Http.Results.Ok($"Account with name {dbResult.Response!} successfully deleted.")
                    : Microsoft.AspNetCore.Http.Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("DeleteAccount")
            .WithOpenApi();
        }

        private static async Task<DatabaseResults<T>> TryDatabaseInteraction<T>(Func<Task<T>> Callback)
        {
            try
            {
                return new DatabaseResults<T>(await Callback(), null, 200);
            }
            catch (DataNotFoundException ex)
            {
                return new DatabaseResults<T>(default, ex.Message, 404);
            }
            catch (DbUpdateConcurrencyException)
            {
                return new DatabaseResults<T>(default, "Database Update Concurrency Exception", 500);
            }
            catch (DbUpdateException)
            {
                return new DatabaseResults<T>(default, "Database Update Exception", 500);
            }
            catch (Exception ex)
            {
                return new DatabaseResults<T>(default, ex.Message, 500);
            }
        }
    }
}
