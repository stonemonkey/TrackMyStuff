using System;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrackMyStuff.Common.Commands;
using TrackMyStuff.ServiceBus;

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
            services.AddControllers();
            services.AddMediatR(GetType().Assembly);
            services.AddAzureServiceBus(Configuration);
            services.AddScoped<ICommandHandler<HeartBeatCommand>, HeartBeatCommandHandler>();
            var connectionString = Configuration["ConnectionStrings:ApiDbConnection"];
            services.AddDbContext<ApiContext>(options =>
                options.UseMySql(connectionString, builder =>
                {
                    builder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
                }));
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
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseServiceBus(serviceBus =>
            {
                serviceBus.Connect();
                serviceBus.Subscribe<HeartBeat, HeartBeatCommandHandler>();
            });
        }
    }
}
