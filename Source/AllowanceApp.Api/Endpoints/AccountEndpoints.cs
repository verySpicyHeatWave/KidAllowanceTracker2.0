using AllowanceApp.Api.DTO;
using AllowanceApp.Api.Records;
using AllowanceApp.Api.Utilities;
using AllowanceApp.Core.Models;
using AllowanceApp.Core.Services;
using AllowanceApp.Data.Contexts;
using AllowanceApp.Data.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AllowanceApp.Api.Endpoints
{
    public static class AccountEndpoints
    {
        private const bool WITHDRAW = true;
        private const bool DEPOSIT = false;
        private const bool INCREMENT = true;
        private const bool DECREMENT = false;

        public static void SetAccountCreateEndpoints(this WebApplication app)
        {
            app.MapPost("/accounts/create/{name}", async (string name, AccountService accountService) =>
            {
                var dbResult = await TryDatabaseInteraction<Account>(async () =>
                {
                    var account = await accountService.AddAccountAsync(name);
                    return account;
                });
                return dbResult.IsSuccess
                    ? Results.Ok(new AccountDTO(dbResult.Response!))
                    : Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
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
                    ? Results.Ok(DTOUtility.AccountListToDTOs(dbResult.Response!))
                    : Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
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
                    ? Results.Ok(new AccountDTO(dbResult.Response!))
                    : Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("GetOneAccount")
            .WithOpenApi();

            app.MapGet("/accounts/read/{id}/point/{category}", async (int id, string category, AccountService accountService) =>
            {
                var dbResult = await TryDatabaseInteraction<AllowancePoint>(async () =>
                {
                    var point = await accountService.GetAllowancePointAsync(id, category);
                    return point;
                });
                return dbResult.IsSuccess
                    ? Results.Ok(new AllowancePointDTO(dbResult.Response!))
                    : Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("GetAccountPoints")
            .WithOpenApi();

            app.MapGet("/accounts/read/{id}/transactions", async (int id, AccountService accountService) =>
            {
                var dbResult = await TryDatabaseInteraction<List<Transaction>>(async () =>
                {
                    var account = await accountService.GetAccountAsync(id);
                    return account.Transactions;
                });
                return dbResult.IsSuccess && dbResult.Response!.Count != 0
                    ? Results.Ok(DTOUtility.TransactionListToDTOs(dbResult.Response))
                    : Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("GetAccountTransactions")
            .WithOpenApi();
        }

        public static void SetAccountUpdateEndpoints(this WebApplication app)
        {
            app.MapPut("/accounts/update/{id}/increment/{category}", async (int id, string category, AccountService accountService) =>
            {

                var dbResult = await TryDatabaseInteraction<AllowancePoint>(async () =>
                {
                    var point = await accountService.SinglePointAdjustAsync(id, category, PointOperation.Increment);
                    return point;
                });
                return dbResult.IsSuccess
                    ? Results.Ok(new AllowancePointDTO(dbResult.Response!))
                    : Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("IncrementPoints")
            .WithOpenApi();

            app.MapPut("/accounts/update/{id}/decrement/{category}", async (int id, string category, AccountService accountService) =>
            {
                var dbResult = await TryDatabaseInteraction<AllowancePoint>(async () =>
                {
                    var point = await accountService.SinglePointAdjustAsync(id, category, PointOperation.Decrement);
                    return point;
                });
                return dbResult.IsSuccess
                    ? Results.Ok(new AllowancePointDTO(dbResult.Response!))
                    : Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("DecrementPoints")
            .WithOpenApi();

            app.MapPut("/accounts/update/{id}/setprice/{category}", async (int id, string category, int amount, AccountService accountService) =>
            {
                var dbResult = await TryDatabaseInteraction<AllowancePoint>(async () =>
                {
                    var point = await accountService.UpdateAllowancePriceAsync(id, category, amount);
                    return point;
                });
                return dbResult.IsSuccess
                    ? Results.Ok(new AllowancePointDTO(dbResult.Response!))
                    : Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
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
                    ? Results.Ok(new AccountDTO(dbResult.Response!))
                    : Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("PayAllowance")
            .WithOpenApi();

            app.MapPut("/accounts/update/{id}/transaction/deposit", async (AccountContext context, int id, int amount, string? description, AccountService accountService) =>
            {
                if (amount <= 0)
                {
                    return Results.Problem(
                        detail: $"Deposit amount must be more than zero, not {amount}",
                        statusCode: StatusCodes.Status400BadRequest
                    );
                }
                var dbResult = await TryDatabaseInteraction<Account>(async () =>
                {
                    var account = await accountService.ApplyTransactionAsync(id, amount, TransactionType.Deposit, description);
                    return account;
                });
                return dbResult.IsSuccess
                    ? Results.Ok(new AccountDTO(dbResult.Response!))
                    : Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("DepositIntoAccount")
            .WithOpenApi();

            app.MapPut("/accounts/update/{id}/transaction/withdrawal", async (AccountContext context, int id, int amount, string? description, AccountService accountService) =>
            {
                if (amount <= 0)
                {
                    return Results.Problem(
                        detail: $"Withdrawal amount must be more than zero, not {amount}",
                        statusCode: StatusCodes.Status400BadRequest
                    );
                }
                var dbResult = await TryDatabaseInteraction<Account>(async () =>
                {
                    var account = await accountService.ApplyTransactionAsync(id, amount, TransactionType.Withdraw, description);
                    return account;
                });
                return dbResult.IsSuccess
                    ? Results.Ok(new AccountDTO(dbResult.Response!))
                    : Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
            })
            .WithName("WithdrawFromAccount")
            .WithOpenApi();
        }

        public static void SetAccountDeleteEndpoints(this WebApplication app)
        {
            app.MapDelete("/accounts/delete/{id}", async (AccountContext context, int id, AccountService accountService) =>
            {
                var dbResult = await TryDatabaseInteraction<string>(async () =>
                {
                    var name = await accountService.DeleteAccountAsync(id);
                    return name;
                });
                return dbResult.IsSuccess
                    ? Results.Ok($"Account with name {dbResult.Response!} successfully deleted.")
                    : Results.Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
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
