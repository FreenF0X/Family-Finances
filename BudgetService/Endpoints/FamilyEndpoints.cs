using BudgetService.Contracts;
using BusinesLogic;

namespace BudgetService.Endpoints
{
    public static class FamilyEndpoints
    {
        public static void MapFamilyEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/families");

            // CREATE
            group.MapPost("/", async (CreateFamilyRequest request, FamilyService service) =>
            {
                var family = await service.CreateAsync(request.Name);
                var response = new FamilyResponse(family.Id, family.Name, family.CreatedAt);
                return Results.Created($"/api/families/{family.Id}", response);
            });

            // READ ALL
            group.MapGet("/", async (FamilyService service) =>
            {
                var families = await service.GetAllAsync();
                var response = families.Select(f => new FamilyResponse(f.Id, f.Name, f.CreatedAt));
                return Results.Ok(response);
            });

            // READ BY ID
            group.MapGet("/{id:guid}", async (Guid id, FamilyService service) =>
            {
                var family = await service.GetByIdAsync(id);
                if (family == null) return Results.NotFound();

                var response = new FamilyResponse(family.Id, family.Name, family.CreatedAt);
                return Results.Ok(response);
            });

            // UPDATE
            group.MapPut("/{id:guid}", async (Guid id, UpdateFamilyRequest request, FamilyService service) =>
            {
                var family = await service.UpdateAsync(id, request.Name);
                if (family == null) return Results.NotFound();

                var response = new FamilyResponse(family.Id, family.Name, family.CreatedAt);
                return Results.Ok(response);
            });

            // DELETE
            group.MapDelete("/{id:guid}", async (Guid id, FamilyService service) =>
            {
                var success = await service.DeleteAsync(id);
                return success ? Results.NoContent() : Results.NotFound();
            });
        }
    }
}
