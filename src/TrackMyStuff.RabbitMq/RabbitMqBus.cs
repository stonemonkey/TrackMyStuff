using System.Threading.Tasks;
using RawRabbit;
using TrackMyStuff.Common.Commands;
using TrackMyStuff.Common.Events;
using TrackMyStuff.Common.ServiceBus;

namespace TrackMyStuff.RabbitMq
{
    public class RabbitMqBus : IServiceBus
    {
        private readonly IBusClient _queueClient;

        public RabbitMqBus(IBusClient queueClient)
        {
            _queueClient = queueClient;
        }

        public Task PublishCommandAsync(ICommand cmd)
        {
            return _queueClient.PublishAsync(cmd);
        }

        public Task PublishEventAsync(IEvent evt)
        {
            return _queueClient.PublishAsync(evt);
        }
    }
}