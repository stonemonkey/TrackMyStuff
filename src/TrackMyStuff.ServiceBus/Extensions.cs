using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TrackMyStuff.ServiceBus
{
    public static class Extensions
    {
        public static void AddAzureServiceBus(
            this IServiceCollection service, IConfiguration configuration)
        {
            var section = configuration.GetSection("AzureServiceBus");
            var connectionString = section["ConnectionString"];
            var queueName = section["QueueName"];
            var queueClient = new QueueClient(connectionString, queueName);

            service.AddSingleton<IQueueClient>(_ => queueClient);
            service.AddSingleton<IServiceBus, AzureServiceBus>();
        }

        public static void UseServiceBus(this IApplicationBuilder app, Action<IServiceBus> configure)
        {
            var serviceBus = app.ApplicationServices.GetRequiredService<IServiceBus>();
            configure(serviceBus);
        }
    }
}
