using TrackMyStuff.Common.Commands;
using TrackMyStuff.Common.Events;

namespace TrackMyStuff.Common.ServiceBus
{
    public interface IBusBuilder
    {
        IBusBuilder SubscribeToCommand<T>() where T: ICommand;
        IBusBuilder SubscribeToEvent<T>() where T: IEvent;
    }
}
