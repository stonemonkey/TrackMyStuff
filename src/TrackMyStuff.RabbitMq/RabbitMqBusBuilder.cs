using System;
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
            var handler = (ICommandHandler<T>) _serviceProvider.GetRequiredService<ICommandHandler<T>>();
            _busClient.WithCommandHandler(handler);
            return this;
        }

        public IBusBuilder SubscribeToEvent<T>() where T: IEvent
        {
            var handler = (IEventHandler<T>) _serviceProvider.GetRequiredService<IEventHandler<T>>();
            _busClient.WithEventHandler(handler);
            return this;
        }
    }
}
