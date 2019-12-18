using System;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using RabbitMQ.Client.Exceptions;
using RawRabbit;
using RawRabbit.Configuration;
using RawRabbit.Instantiation;
using TrackMyStuff.Common.Commands;
using TrackMyStuff.Common.Events;
using TrackMyStuff.Common.ServiceBus;

namespace TrackMyStuff.RabbitMq
{
    public static class Extensions
    {
        public static IServiceCollection AddRabbitMq(
            this IServiceCollection services, IConfiguration configuration)
        {
            var options = new RawRabbitConfiguration();
            configuration.GetSection("RabbitMq").Bind(options);

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

        public static void WithCommandHandler<T>(
            this IBusClient busClient, ICommandHandler<T> handler)
            where T: ICommand
            => busClient.SubscribeAsync<T>(msg => 
                handler.HandleAsync(msg),
                ctx => ctx.UseSubscribeConfiguration(cfg => 
                    cfg.FromDeclaredQueue(q => q.WithName(GetQueueName<T>()))));

        public static void WithEventHandler<T>(
            this IBusClient busClient, IEventHandler<T> handler)
            where T: IEvent
            => busClient.SubscribeAsync<T>(msg => 
                handler.HandleAsync(msg),
                ctx => ctx.UseSubscribeConfiguration(cfg => 
                    cfg.FromDeclaredQueue(q => q.WithName(GetQueueName<T>()))));

        private static string GetQueueName<T>() 
            => $"{Assembly.GetEntryAssembly().GetName()}/{typeof(T).Name}";
    }
}
