using BudgetService.Contracts;
using BusinesLogic;
using System.Security.Claims;

namespace BudgetService.Endpoints
{
    public static class AccountEndpoints
    {
        public static void MapAccountEndpoints(this IEndpointRouteBuilder app)
        {
            // Достаточно повесить RequireAuthorization() на группу, 
            // и все эндпоинты внутри нее станут защищенными.
            var group = app.MapGroup("/api/accounts")
                .RequireAuthorization()
                .WithTags("Accounts");

            group.MapPost("/", async (CreateAccountRequest request, AccountService service, HttpContext httpContext) =>
            {
                try
                {
                    // Вот так достается userId из токена
                    var userId = Guid.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                            ?? httpContext.User.FindFirst("sub")?.Value
                                            ?? throw new Exception("User ID not found in token"));

                    // В будущем здесь можно передавать userId в сервис, чтобы проверить,
                    // состоит ли этот пользователь в семье request.FamilyId.

                    var account = await service.CreateAsync(request.FamilyId, request.Name, request.Currency);
                    var response = new AccountResponse(account.Id, account.FamilyId, account.Name, account.Currency, account.Balance, account.IsClosed);
                    return Results.Created($"/api/accounts/{account.Id}", response);
                }
                catch (Exception ex) { return Results.BadRequest(ex.Message); }
            })
            .WithSummary("Создание счета")
            .WithDescription("Создает новый финансовый счет для указанной семьи.")
            .Produces<AccountResponse>(StatusCodes.Status201Created)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

            group.MapGet("/family/{familyId:guid}", async (Guid familyId, AccountService service) =>
            {
                var accounts = await service.GetByFamilyIdAsync(familyId);
                var response = accounts.Select(a => new AccountResponse(a.Id, a.FamilyId, a.Name, a.Currency, a.Balance, a.IsClosed));
                return Results.Ok(response);
            })
            .WithSummary("Получение счетов семьи")
            .WithDescription("Возвращает список всех счетов, принадлежащих указанной семье.")
            .Produces<IEnumerable<AccountResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

            group.MapPut("/{id:guid}", async (Guid id, UpdateAccountRequest request, AccountService service) =>
            {
                var account = await service.UpdateAsync(id, request.Name, request.Currency);
                if (account == null) return Results.NotFound();

                var response = new AccountResponse(account.Id, account.FamilyId, account.Name, account.Currency, account.Balance, account.IsClosed);
                return Results.Ok(response);
            })
            .WithSummary("Обновление счета")
            .WithDescription("Обновляет название и валюту счета по его ID.")
            .Produces<AccountResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);

            // Закрытие счета
            group.MapPost("/{id:guid}/close", async (Guid id, AccountService service) =>
            {
                try
                {
                    var account = await service.CloseAsync(id);
                    if (account == null) return Results.NotFound();

                    return Results.Ok($"Счет '{account.Name}' успешно закрыт.");
                }
                catch (Exception ex) { return Results.BadRequest(ex.Message); }
            })
            .WithSummary("Закрытие счета")
            .WithDescription("Закрывает счет. После закрытия операции по нему будут недоступны.")
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);

            group.MapDelete("/{id:guid}", async (Guid id, AccountService service) =>
            {
                try
                {
                    var success = await service.DeleteAsync(id);
                    return success ? Results.NoContent() : Results.NotFound();
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithSummary("Удаление счета")
            .WithDescription("Удаляет счет по ID. Если по счету есть транзакции, возвращается ошибка.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}