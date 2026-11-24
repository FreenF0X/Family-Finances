using BudgetService.Protos;
using System.Security.Claims;

namespace BudgetService.Endpoints
{
    public static class AnalyticsEndpoints
    {
        public static void MapAnalyticsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/analytics")
                .RequireAuthorization()
                .WithTags("Analytics");

            group.MapGet("/monthly-report", async (
                int year,
                int month,
                Guid familyId,
                Analytics.AnalyticsClient analyticsClient,
                HttpContext httpContext) =>
            {
                try
                {
                    var request = new MonthlyReportRequest
                    {
                        FamilyId = familyId.ToString(),
                        Year = year,
                        Month = month
                    };

                    var response = await analyticsClient.GetMonthlyReportAsync(request);
                    return Results.Ok(response);
                }
                catch (Grpc.Core.RpcException ex)
                {
                    return Results.Problem($"gRPC Error: {ex.Status.Detail}");
                }
            })
            .WithSummary("Получение месячного отчета")
            .WithDescription("Запрашивает агрегированные данные через gRPC у AnalyticsService.")
            .Produces<MonthlyReportResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}