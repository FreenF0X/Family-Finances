using AnalyticService.Services;

namespace AnalyticService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Добавляем gRPC
            builder.Services.AddGrpc();

            // Явно настраиваем Kestrel для gRPC без TLS
            builder.WebHost.ConfigureKestrel(options =>
            {
                // Слушаем порт на всех сетевых интерфейсах
                options.ListenAnyIP(5276, listenOptions =>
                {
                    // gRPC требует HTTP/2. Без TLS используем только Http2 (не Http1AndHttp2)
                    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
                });
            });

            var app = builder.Build();

            // Мапим наш gRPC-сервис
            app.MapGrpcService<AnalyticGrpcService>();
            
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}
