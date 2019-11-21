using MediatR;

namespace TrackMyStuff.ApiGateway.Queries
{
    public class GetDeviceStatusQuery : IRequest<DeviceStatus>
    {
        public string DeviceId { get; set; }
    }
}

