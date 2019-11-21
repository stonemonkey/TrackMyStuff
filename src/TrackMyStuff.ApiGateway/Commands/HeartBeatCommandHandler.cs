using System.Threading.Tasks;
using TrackMyStuff.Common.Commands;

public class HeartBeatCommandHandler : ICommandHandler<HeartBeatCommand>
{
    public Task HandleAsync(HeartBeatCommand command)
    {
        return Task.FromResult(true);
    }
}

