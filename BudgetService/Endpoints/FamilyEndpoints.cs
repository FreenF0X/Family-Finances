using BudgetService.Contracts;
using BusinesLogic;

namespace BudgetService.Endpoints
{
    public static class FamilyEndpoints
    {
        public static void MapFamilyEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/families")
                .RequireAuthorization()
                .WithTags("Families");

            // CREATE
            group.MapPost("/", async (CreateFamilyRequest request, FamilyService service) =>
            {
                var family = await service.CreateAsync(request.Name);
                var response = new FamilyResponse(family.Id, family.Name, family.CreatedAt);
                return Results.Created($"/api/families/{family.Id}", response);
            })
            .WithSummary("Создание семьи")
            .WithDescription("Создает новую семейную группу и возвращает её данные.")
            .Produces<FamilyResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized);

            // READ ALL
            group.MapGet("/", async (FamilyService service) =>
            {
                var families = await service.GetAllAsync();
                var response = families.Select(f => new FamilyResponse(f.Id, f.Name, f.CreatedAt));
                return Results.Ok(response);
            })
            .WithSummary("Получение списка семей")
            .WithDescription("Возвращает список всех семей, доступных текущему пользователю.")
            .Produces<IEnumerable<FamilyResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

            // READ BY ID
            group.MapGet("/{id:guid}", async (Guid id, FamilyService service) =>
            {
                var family = await service.GetByIdAsync(id);
                if (family == null) return Results.NotFound();

                var response = new FamilyResponse(family.Id, family.Name, family.CreatedAt);
                return Results.Ok(response);
            })
            .WithSummary("Получение семьи по ID")
            .WithDescription("Возвращает данные конкретной семьи по её уникальному идентификатору.")
            .Produces<FamilyResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);

            // UPDATE
            group.MapPut("/{id:guid}", async (Guid id, UpdateFamilyRequest request, FamilyService service) =>
            {
                var family = await service.UpdateAsync(id, request.Name);
                if (family == null) return Results.NotFound();

                var response = new FamilyResponse(family.Id, family.Name, family.CreatedAt);
                return Results.Ok(response);
            })
            .WithSummary("Обновление семьи")
            .WithDescription("Обновляет название семьи по её ID.")
            .Produces<FamilyResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);

            // DELETE
            group.MapDelete("/{id:guid}", async (Guid id, FamilyService service) =>
            {
                var success = await service.DeleteAsync(id);
                return success ? Results.NoContent() : Results.NotFound();
            })
            .WithSummary("Удаление семьи")
            .WithDescription("Удаляет семью по её ID.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}
