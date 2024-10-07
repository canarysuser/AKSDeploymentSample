using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ProductGrpcService.Services;
using ProductsProtoLibrary.Helpers;
using Prometheus;
using ProtoBuf.Grpc.Server;
using Serilog;

namespace ProductGrpcService
{
    public class Program
    {
        static string connectionString = @"Server=4.227.57.229;Database=Northwind;User Id=sa;Password=AVery_str0ngPwd;trustservercertificate=true"; 

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(80, o => o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);
            });

            //connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            connectionString = Environment.GetEnvironmentVariable("SqlServerConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = @"Server=4.227.57.229;Database=Northwind;User Id=sa;Password=AVery_str0ngPwd;trustservercertificate=true";
            }
                //builder.WebHost.ConfigureKestrel(options =>
                //{
                //    options.ListenAnyIP(443, config => config.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2);
                //});
                ConfigureServices(builder.Services);


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            ConfigureApp(app);

            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
        static void ConfigureServices(IServiceCollection services)
        {
            ConfigureApplicationServices(services);
            ConfigureGrpcServices(services);
            ConfigureHealthServicesAndMetrics(services);
        }
        static void ConfigureHealthServicesAndMetrics(IServiceCollection services)
        {

        }
        static void ConfigureApplicationServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            services.AddSerilog();

            

            services.AddDbContext<ProductDbContext>(options =>
            {
                options.UseSqlServer(connectionString: connectionString);
            });
            services.AddScoped<IProductRepositoryAsync, ProductRepository>();
            services.AddCors(setupAction =>
            {
                setupAction.AddPolicy(
                    name: "AllowAll",
                    policy: new CorsPolicyBuilder()
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyMethod()
                    .WithExposedHeaders("Grpc-Status",
                        "Grpc-Message",
                        "Grpc-Encoding",
                        "Grpc-Accept-Encoding")
                    .Build()
                );
            });
        }
        static void ConfigureGrpcServices(IServiceCollection services)
        {
            // Add services to the container.
            services.AddGrpc()
                .AddJsonTranscoding();
            services.AddGrpcReflection();

        }
        static void ConfigureSecurityServices(IServiceCollection services)
        {
        }
        static void ConfigureApp(WebApplication app)
        {
            
            app.UseRouting();
            app.UseCors(policyName: "AllowAll");
            AddMetricsConfigurations(app);
            AddSecurityConfiguration(app);
            AddGrpcConfigurations(app);

        }
        static void AddMetricsConfigurations(WebApplication app)
        {
            app.UseMetricServer();
            app.UseHttpMetrics(options =>
            {
                options.AddCustomLabel("host", context => context.Request.Host.Host);
            });
        }
        static void AddGrpcConfigurations(WebApplication app)
        {

            app.UseGrpcWeb();
            app.MapGrpcService<ProductDbServiceImpl>()
                .RequireCors(policyName: "AllowAll");
            app.MapGrpcService<ProductJsonServiceImpl>()
                .EnableGrpcWeb()
                .RequireCors(policyName: "AllowAll");
            app.MapGrpcReflectionService();
            app.MapCodeFirstGrpcReflectionService();
        }
        static void AddSecurityConfiguration(WebApplication app)
        {

        }
    }
}