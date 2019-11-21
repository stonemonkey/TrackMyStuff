using System.Threading.Tasks;

namespace TrackMyStuff.Common.Commands
{
    public interface ICommandHandler<in T> where T: ICommand
    {
        Task HandleAsync(T evt);
    }
}