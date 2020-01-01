using System;
using System.Net.Sockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using RabbitMQ.Client.Exceptions;
using RawRabbit;
using RawRabbit.Configuration;
using RawRabbit.Instantiation;
using TrackMyStuff.Common.ServiceBus;

namespace TrackMyStuff.RabbitMq
{
    public static class Extensions
    {
        public static string GetRabbitMqConnectionString(this IConfiguration configuration)
        {
            var section = configuration.GetSection("RabbitMq");
            var username = section.GetValue("Username", "guest");
            var password = section.GetValue("Password", "guest");
            var host = section.GetValue("HostName:0", "rabbitMq");
            var port = section.GetValue("Port", "5672");
            var vhost = section.GetValue("VirtualHost", "");
            if (vhost == "/")
            {
                vhost = "";
            }
            return $"amqp://{username}:{password}@{host}:{port}/{vhost}";
        }

        public static IServiceCollection AddRabbitMq(
            this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("RabbitMq");
            if (!section.Exists())
            {
                throw new InvalidOperationException("Missing 'RabbitMq' section in config!");
            }
            var options = new RawRabbitConfiguration();
            section.Bind(options);

            IBusClient client = null;
            Policy
                .Handle<BrokerUnreachableException>().Or<SocketException>()
                .WaitAndRetry(3, _ => TimeSpan.FromSeconds(10))
                .Execute(() =>
                {
                    client = RawRabbitFactory.CreateSingleton(
                        new RawRabbitOptions { ClientConfiguration = options });
                });
            services.AddSingleton<IBusClient>(_ => client);
            services.AddSingleton<IServiceBus, RabbitMqBus>();
            services.AddSingleton<IBusBuilder, RabbitMqBusBuilder>();
            return services;
        }

        public static void UseRabbitMq(this IApplicationBuilder app, Action<IBusBuilder> build)
        {
            var builder = app.ApplicationServices.GetRequiredService<IBusBuilder>();
            build(builder);
        }
    }
}
