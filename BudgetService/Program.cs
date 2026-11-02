using BudgetService.Endpoints;
using BusinesLogic;
using DataAccess;
using Microsoft.EntityFrameworkCore;


namespace BudgetService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Регистрация DbContext
            builder.Services.AddDbContext<BudgetDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Регистрация сервисов
            builder.Services.AddScoped<FamilyService>();
            builder.Services.AddScoped<AccountService>();
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<TransactionService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapGet("/", () => "Hello World!");
            app.MapFamilyEndpoints(); 
            app.MapAccountEndpoints();
            app.MapCategoryEndpoints();
            app.MapTransactionEndpoints();

            app.Run();
        }
    }
}
