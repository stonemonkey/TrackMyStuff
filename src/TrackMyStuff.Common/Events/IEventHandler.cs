using System.Threading.Tasks;

namespace TrackMyStuff.Common.Events
{
    public interface IEventHandler<in T> where T: IEvent
    {
        Task HandleAsync(T evt);
    }
}