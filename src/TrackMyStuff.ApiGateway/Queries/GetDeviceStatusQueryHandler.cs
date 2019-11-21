using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace TrackMyStuff.ApiGateway.Queries
{
    public class GetDeviceStatusQueryHandler : IRequestHandler<GetDeviceStatusQuery, DeviceStatus>
    {
        private readonly ApiContext _ctx;

        public GetDeviceStatusQueryHandler(ApiContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<DeviceStatus> Handle(GetDeviceStatusQuery query, CancellationToken cancellationToken)
        {
            return await _ctx.DeviceStatus.FindAsync(query.DeviceId);
        }
    }
}

