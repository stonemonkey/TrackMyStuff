using System;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Serilog;
using TrackMyStuff.Common.Events;
using TrackMyStuff.RabbitMq;

namespace TrackMyStuff.ApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["ConnectionStrings:ApiConnection"];
            Log.Information("ConnectionStrings:ApiConnection={ApiConnection}", connectionString);

            var rabbitMqConnectionString = Configuration.GetRabbitMqConnectionString();
            Log.Information("RabbitMqConnectionString={RabbitMqConnection}", rabbitMqConnectionString);
            
            services.AddControllers();
            services.AddRabbitMq(Configuration);
            services.AddCustomDbContext<ApiContext>(connectionString);
            services.AddScoped<IEventHandler<DeviceStatusUpdatedEvent>, DeviceStatusUpdatedEventHandler>();

            // health checks
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddMySql(
                    connectionString,
                    name: "ApiGateway-check",
                    tags: new string[] { "db", "mysql" })
                .AddRabbitMQ(
                    rabbitMqConnectionString,
                    name: "ApiGateway-rabbitmqbus-check",
                    tags: new string[] { "rabbitmqbus" });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseAuthorization();
            app.UseSerilogRequestLogging();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseRabbitMq(builder => builder
                .SubscribeToEvent<DeviceStatusUpdatedEvent>());

            // health check endpoints
            app.UseHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
            app.UseHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true, // runs all registred checks
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        }
    }
    
    public static class CustomExtensions
    {
        public static IServiceCollection AddCustomDbContext<TContext>(
            this IServiceCollection services, string connectionString)
            where TContext : DbContext
        {
            services.AddDbContext<ApiContext>(options =>
                options.UseMySql(connectionString, builder =>
                {
                    builder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
                }));
            return services;
        }
    }
}
