using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit;
using TrackMyStuff.Common.Commands;
using TrackMyStuff.Common.Events;
using TrackMyStuff.Common.ServiceBus;

namespace TrackMyStuff.RabbitMq
{
    public class RabbitMqBusBuilder : IBusBuilder
    {
        private IBusClient _busClient;
        private IServiceProvider _serviceProvider;

        public RabbitMqBusBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _busClient = _serviceProvider.GetRequiredService<IBusClient>();
        }

        public IBusBuilder SubscribeToCommand<T>() where T: ICommand
        {
            _busClient.SubscribeAsync<T>(
                async msg => { 
                    // we needed a scope because registred scoped instances are no accesible in the middlewhere  
                    // https://stackoverflow.com/questions/48590579/cannot-resolve-scoped-service-from-root-provider-net-core-2
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var handler = (ICommandHandler<T>) scope.ServiceProvider.GetRequiredService<ICommandHandler<T>>();
                        await handler.HandleAsync(msg);
                    }},
                ctx => ctx.UseSubscribeConfiguration(cfg => 
                    cfg.FromDeclaredQueue(q => q.WithName(GetQueueName<T>()))));

            return this;
        }

        public IBusBuilder SubscribeToEvent<T>() where T: IEvent
        {
            _busClient.SubscribeAsync<T>(
                async msg => {
                    // we needed a scope because registred scoped instances are no accesible in the middlewhere  
                    // https://stackoverflow.com/questions/48590579/cannot-resolve-scoped-service-from-root-provider-net-core-2
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var handler = (IEventHandler<T>) scope.ServiceProvider.GetRequiredService<IEventHandler<T>>();
                        await handler.HandleAsync(msg);
                    }},
                ctx => ctx.UseSubscribeConfiguration(cfg => 
                    cfg.FromDeclaredQueue(q => q.WithName(GetQueueName<T>()))));

            return this;
        }

        private static string GetQueueName<T>() 
            => $"{Assembly.GetEntryAssembly().GetName()}/{typeof(T).Name}";
    }
}
