using BudgetService.Contracts;
using BusinesLogic;
using Domain.Models;

namespace BudgetService.Endpoints
{
    public static class CategoryEndpoints
    {
        public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/categories")
                .RequireAuthorization()
                .WithTags("Categories");

            group.MapPost("/", async (CreateCategoryRequest request, CategoryService service) =>
            {
                try
                {
                    var category = await service.CreateAsync(
                        request.FamilyId,
                        request.Name,
                        (TransactionType)request.Type,
                        request.ParentCategoryId);

                    var response = new CategoryResponse(
                        category.Id,
                        category.FamilyId,
                        category.Name,
                        (int)category.Type,
                        category.ParentCategoryId);

                    return Results.Created($"/api/categories/{category.Id}", response);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithSummary("Создание категории")
            .WithDescription("Создает новую категорию доходов или расходов.")
            .Produces<CategoryResponse>(StatusCodes.Status201Created)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

            group.MapGet("/family/{familyId:guid}", async (Guid familyId, CategoryService service) =>
            {
                var categories = await service.GetAvailableForFamilyAsync(familyId);
                var response = categories.Select(c => new CategoryResponse(c.Id, c.FamilyId, c.Name, (int)c.Type, c.ParentCategoryId));
                return Results.Ok(response);
            })
            .WithSummary("Получение категорий семьи")
            .WithDescription("Возвращает список категорий, доступных для указанной семьи.")
            .Produces<IEnumerable<CategoryResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

            group.MapPut("/{id:guid}", async (Guid id, UpdateCategoryRequest request, CategoryService service) =>
            {
                try
                {
                    var category = await service.UpdateAsync(id, request.Name, (TransactionType)request.Type);
                    if (category == null) return Results.NotFound();

                    var response = new CategoryResponse(category.Id, category.FamilyId, category.Name, (int)category.Type, category.ParentCategoryId);
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithSummary("Обновление категории")
            .WithDescription("Обновляет название и тип категории по её ID.")
            .Produces<CategoryResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);

            group.MapDelete("/{id:guid}", async (Guid id, CategoryService service) =>
            {
                try
                {
                    var success = await service.DeleteAsync(id);
                    return success ? Results.NoContent() : Results.NotFound();
                }
                catch (Exception ex)
                {
                    // Сюда попадут наши проверки на транзакции и подкатегории
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithSummary("Удаление категории")
            .WithDescription("Удаляет категорию по ID. Если есть транзакции или подкатегории, возвращается ошибка.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}
