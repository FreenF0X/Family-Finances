using BudgetService.Contracts;
using BusinesLogic;
using Domain.Models;
using System.Security.Claims;

namespace BudgetService.Endpoints
{
    public static class TransactionEndpoints
    {
        public static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/transactions")
                .RequireAuthorization()
                .WithTags("Transactions");

            // CREATE
            group.MapPost("/", async (CreateTransactionRequest request, TransactionService service, HttpContext httpContext) =>
            {
                try
                {
                    // Достаем ID пользователя из JWT-токена
                    var hardcodedUserId = Guid.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                            ?? httpContext.User.FindFirst("sub")?.Value
                                            ?? throw new Exception("User ID not found in token"));

                    var transaction = await service.CreateAsync(
                        request.AccountId,
                        request.CategoryId,
                        request.Amount,
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
            })
            .WithSummary("Создание транзакции")
            .WithDescription("Создает новую транзакцию (доход или расход) и привязывает её к счету и категории. ID пользователя берется из JWT.")
            .Produces<TransactionResponse>(StatusCodes.Status201Created)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

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
            })
            .WithSummary("Получение транзакций по счету")
            .WithDescription("Возвращает список всех транзакций, привязанных к указанному счету.")
            .Produces<IEnumerable<TransactionResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}
