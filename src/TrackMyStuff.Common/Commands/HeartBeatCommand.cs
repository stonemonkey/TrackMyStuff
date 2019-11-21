namespace TrackMyStuff.Common.Commands
{
    public class HeartBeatCommand : ICommand
    {
        public string DeviceId { get; set; }
    }
}