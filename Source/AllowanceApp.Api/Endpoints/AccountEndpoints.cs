using AllowanceApp.Api.Results;
using AllowanceApp.Core.Models;
using AllowanceApp.Core.Services;
using AllowanceApp.Data.Exceptions;
using AllowanceApp.Shared.DTO;
using AllowanceApp.Shared.Utilities;
using static Microsoft.AspNetCore.Http.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace AllowanceApp.Api.Endpoints
{
    public static class AccountEndpoints
    {
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

        private static async Task<IResult> HandleRequest<T>(Func<Task<T>> func, Func<T, IResult> onSuccess)
        {
            var dbResult = await TryDatabaseInteraction(func);
            return dbResult.IsSuccess
                ? onSuccess(dbResult.Response!)
                : Problem(detail: dbResult.Message, statusCode: dbResult.StatusCode);
        }

        public static void SetAccountCreateEndpoints(this WebApplication app) =>
            app.MapPost("/accounts/create", async (CreateAccountRequest request, AccountService accountService) =>
                await HandleRequest(
                    () => accountService.AddAccountAsync(request.Name),
                    account => Ok(new AccountDTO(account))
                )
            )
            .WithName("CreateAccount")
            .WithOpenApi()
            .Produces<AccountDTO>()
            .ProducesProblem(500);

        public static void SetAccountReadEndpoints(this WebApplication app)
        {
            var AccountRead = app.MapGroup("/accounts/read");

            AccountRead.MapGet("/all", async (AccountService accountService) =>
                await HandleRequest(
                    () => accountService.GetAllAccountsAsync(),
                    accounts => Ok(DTOUtility.AccountListToDTOs(accounts))
                )
            )
            .WithName("ReadAllAccounts")
            .WithOpenApi()
            .Produces<List<AccountDTO>>()
            .ProducesProblem(500);

            AccountRead.MapGet("/{id}", async (int id, AccountService accountService) =>
                await HandleRequest(
                    () => accountService.GetAccountAsync(id),
                    account => Ok(new AccountDTO(account))
                )
            )
            .WithName("ReadAccount")
            .WithOpenApi();

            AccountRead.MapGet("/{id}/points/{category}", async (int id, string category, AccountService accountService) =>
                await HandleRequest(
                    () => accountService.GetAllowancePointAsync(id, category),
                    point => Ok(new AllowancePointDTO(point))
                )
            )
            .WithName("ReadAllowancePoint")
            .WithOpenApi();

            AccountRead.MapGet("/{id}/transactions", async (int id, AccountService accountService) =>
                await HandleRequest(
                    () => accountService.GetAccountAsync(id),
                    account => Ok(DTOUtility.TransactionListToDTOs(account.Transactions))
                )
            )
            .WithName("ReadAccountTransactions")
            .WithOpenApi();
        }

        public static void SetAccountUpdateEndpoints(this WebApplication app)
        {
            var AccountUpdate = app.MapGroup("/accounts/update");

            AccountUpdate.MapPut("/{id}/points/{category}/increment", async (int id, string category, AccountService accountService) =>
                await HandleRequest(
                    () => accountService.SinglePointAdjustAsync(id, category, PointOperation.Increment),
                    point => Ok(new AllowancePointDTO(point))
                )
            )
            .WithName("UpdatePointIncrement")
            .WithOpenApi();

            AccountUpdate.MapPut("/{id}/points/{category}/decrement", async (int id, string category, AccountService accountService) =>
                await HandleRequest(
                    () => accountService.SinglePointAdjustAsync(id, category, PointOperation.Decrement),
                    point => Ok(new AllowancePointDTO(point))
                )
            )
            .WithName("UpdatePointDecrement")
            .WithOpenApi();

            AccountUpdate.MapPut("/{id}/points/{category}/setpoints", async (int id, string category, PointUpdateRequest request, AccountService accountService) =>
                await HandleRequest(
                    () => accountService.SetPointAsync(id, category, request.Points),
                    point => Ok(new AllowancePointDTO(point))
                )
            )
            .WithName("UpdatePoint")
            .WithOpenApi();

            AccountUpdate.MapPut("/{id}/points/{category}/setprice", async (int id, string category, TransactionRequest request, AccountService accountService) =>
                await HandleRequest(
                    () => accountService.UpdateAllowancePriceAsync(id, category, request.Amount),
                    point => Ok(new AllowancePointDTO(point))
                )
            )
            .WithName("UpdateAllowancePrice")
            .WithOpenApi();

            AccountUpdate.MapPut("/{id}/transaction/payout", async (int id, AccountService accountService) =>
                await HandleRequest(
                    () => accountService.PayAllowanceAsync(id),
                    account => Ok(new AccountDTO(account))
                )
            )
            .WithName("UpdateRequestAllowancePayout")
            .WithOpenApi();

            AccountUpdate.MapPut("/{id}/transaction/deposit", async (int id, TransactionRequest request, AccountService accountService) =>
            {
                if (request.Amount <= 0)
                {
                    return Problem(
                        detail: $"Deposit amount must be more than zero, not {request.Amount}",
                        statusCode: StatusCodes.Status400BadRequest
                    );
                }
                return await HandleRequest(
                    () => accountService.RequestTransactionAsync(id, request.Amount, TransactionType.Deposit, request.Description),
                    account => Ok(new AccountDTO(account))
                );
            })
            .WithName("UpdateRequestDeposit")
            .WithOpenApi();

            AccountUpdate.MapPut("/{id}/transaction/withdrawal", async (int id, TransactionRequest request, AccountService accountService) =>
            {
                if (request.Amount <= 0)
                {
                    return Problem(
                        detail: $"Withdrawal amount must be more than zero, not {request.Amount}",
                        statusCode: StatusCodes.Status400BadRequest
                    );
                }
                return await HandleRequest(
                    () => accountService.RequestTransactionAsync(id, request.Amount, TransactionType.Withdraw, request.Description),
                    account => Ok(new AccountDTO(account))
                );
            })
            .WithName("UpdateRequestWithdrawal")
            .WithOpenApi();

            AccountUpdate.MapPut("/{id}/transaction/approve", async (int id, TransactionStatusUpdateRequest request, AccountService accountService) =>
                await HandleRequest(
                    () => accountService.ApproveTransactionAsync(id, request.TransactionID),
                    account => Ok(new AccountDTO(account))
                )
            )
            .WithName("UpdateApproveTransaction")
            .WithOpenApi();

            AccountUpdate.MapPut("/{id}/transaction/decline", async (int id, TransactionStatusUpdateRequest request, AccountService accountService) =>
                await HandleRequest(
                    () => accountService.DeclineTransactionAsync(id, request.TransactionID),
                    account => Ok(new AccountDTO(account))
                )
            )
            .WithName("UpdateDeclineTransaction")
            .WithOpenApi();
        }

        public static void SetAccountDeleteEndpoints(this WebApplication app)
        {
            app.MapDelete("/accounts/delete/{id}", async (int id, AccountService accountService) =>
                await HandleRequest(
                    () => accountService.DeleteAccountAsync(id),
                    name => Ok($"Account with name {name} successfully deleted.")
                )
            )
            .WithName("DeleteAccount")
            .WithOpenApi();
        }
    }
}
