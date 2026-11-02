using BudgetService.Contracts;
using BusinesLogic;
using Domain.Models;

namespace BudgetService.Endpoints
{
    public static class CategoryEndpoints
    {
        public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/categories");

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
            });

            group.MapGet("/family/{familyId:guid}", async (Guid familyId, CategoryService service) =>
            {
                var categories = await service.GetAvailableForFamilyAsync(familyId);
                var response = categories.Select(c => new CategoryResponse(c.Id, c.FamilyId, c.Name, (int)c.Type, c.ParentCategoryId));
                return Results.Ok(response);
            });

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
            });

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
            });
        }
    }
}
