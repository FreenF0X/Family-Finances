using BudgetService.Contracts;
using BusinesLogic;

namespace BudgetService.Endpoints
{
    public static class AccountEndpoints
    {
        public static void MapAccountEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/accounts");

            group.MapPost("/", async (CreateAccountRequest request, AccountService service) =>
            {
                try
                {
                    var account = await service.CreateAsync(request.FamilyId, request.Name, request.Currency);
                    var response = new AccountResponse(account.Id, account.FamilyId, account.Name, account.Currency, account.Balance, account.IsClosed);
                    return Results.Created($"/api/accounts/{account.Id}", response);
                }
                catch (Exception ex) { return Results.BadRequest(ex.Message); }
            });

            group.MapGet("/family/{familyId:guid}", async (Guid familyId, AccountService service) =>
            {
                var accounts = await service.GetByFamilyIdAsync(familyId);
                var response = accounts.Select(a => new AccountResponse(a.Id, a.FamilyId, a.Name, a.Currency, a.Balance, a.IsClosed));
                return Results.Ok(response);
            });

            group.MapPut("/{id:guid}", async (Guid id, UpdateAccountRequest request, AccountService service) =>
            {
                var account = await service.UpdateAsync(id, request.Name, request.Currency);
                if (account == null) return Results.NotFound();

                var response = new AccountResponse(account.Id, account.FamilyId, account.Name, account.Currency, account.Balance, account.IsClosed);
                return Results.Ok(response);
            });

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
            });

            group.MapDelete("/{id:guid}", async (Guid id, AccountService service) =>
            {
                try
                {
                    var success = await service.DeleteAsync(id);
                    return success ? Results.NoContent() : Results.NotFound();
                }
                catch (Exception ex)
                {
                    // Сюда попадет ошибка "Нельзя удалить счет, так как есть транзакции"
                    return Results.BadRequest(ex.Message);
                }
            });
        }
    }
}
