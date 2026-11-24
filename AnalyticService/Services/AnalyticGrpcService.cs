using AnalyticsService.Protos;
using Grpc.Core;

namespace AnalyticService.Services
{
    public class AnalyticGrpcService : Analytics.AnalyticsBase // Класс сгенерируется автоматически из .proto
    {
        public override Task<MonthlyReportResponse> GetMonthlyReport(MonthlyReportRequest request, ServerCallContext context)
        {
            // Пока это заглушка. Позже (на этапе 5) мы будем брать данные из ClickHouse.
            var response = new MonthlyReportResponse
            {
                TotalExpenses = 15000.50,
                TotalIncomes = 50000.00
            };

            response.Categories.Add(new CategorySummary { CategoryName = "Продукты", Amount = 10000.0 });
            response.Categories.Add(new CategorySummary { CategoryName = "Развлечения", Amount = 5000.50 });

            return Task.FromResult(response);
        }
    }
}
