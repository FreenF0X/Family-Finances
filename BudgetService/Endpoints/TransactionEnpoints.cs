using BudgetService.Contracts;
using BusinesLogic;
using Domain.Models;

namespace BudgetService.Endpoints
{
    public static class TransactionEndpoints
    {
        public static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/transactions");

            // CREATE
            group.MapPost("/", async (CreateTransactionRequest request, TransactionService service) =>
            {
                try
                {
                    // TODO: Позже заменим этот хардкод на реальный ID из JWT-токена
                    var hardcodedUserId = Guid.Parse("0dab9965-2e3e-4f0d-a3de-491526a10410");

                    var transaction = await service.CreateAsync(
                        request.AccountId,
                        request.CategoryId,
                        request.Amount,
                        (TransactionType)request.Type,
                        request.Date,
                        request.Comment,
                        hardcodedUserId);

                    var response = new TransactionResponse(
                        transaction.Id,
                        transaction.AccountId,
                        transaction.CategoryId,
                        transaction.Amount,
                        (int)transaction.Type,
                        transaction.Date,
                        transaction.Comment,
                        transaction.CreatedAt);

                    return Results.Created($"/api/transactions/{transaction.Id}", response);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });

            // READ (по счету)
            group.MapGet("/account/{accountId:guid}", async (Guid accountId, TransactionService service) =>
            {
                var transactions = await service.GetByAccountIdAsync(accountId);

                var response = transactions.Select(t => new TransactionResponse(
                    t.Id,
                    t.AccountId,
                    t.CategoryId,
                    t.Amount,
                    (int)t.Type,
                    t.Date,
                    t.Comment,
                    t.CreatedAt));

                return Results.Ok(response);
            });
        }
    }
}
