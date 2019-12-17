using System.Threading.Tasks;
using TrackMyStuff.Common.Commands;
using TrackMyStuff.Common.Events;

namespace TrackMyStuff.Common.ServiceBus
{
    public interface IServiceBus
    {
        Task PublishCommandAsync(ICommand cmd);
        Task PublishEventAsync(IEvent evt);
    }
}