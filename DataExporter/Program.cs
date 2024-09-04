using DataExporter.Services;

namespace DataExporter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            
            builder.Services.AddDbContext<ExporterDbContext>();
            builder.Services.AddScoped<IPolicyService, PolicyService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
