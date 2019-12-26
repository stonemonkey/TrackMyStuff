using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TrackMyStuff.ApiGateway.DataAccess;
using TrackMyStuff.Common.Events;

namespace TrackMyStuff.ApiGateway
{
    public class DeviceStatusUpdatedEventHandler : IEventHandler<DeviceStatusUpdatedEvent>
    {
        private readonly ApiContext _ctx;
        private readonly ILogger _logger;

        public DeviceStatusUpdatedEventHandler(ApiContext ctx, ILogger<DeviceStatusUpdatedEventHandler> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public async Task HandleAsync(DeviceStatusUpdatedEvent command)
        {
            var status = await _ctx.DeviceStatus.FindAsync(command.DeviceId);
            if (status == null)
            {
                status = new DeviceStatus { DeviceId = command.DeviceId };
                await _ctx.DeviceStatus.AddAsync(status);
            }
            status.LastSeenAt = command.LastSeenAt;
            await _ctx.SaveChangesAsync();

            _logger.LogInformation("Device {0} status updated at {1}.", status.DeviceId, status.LastSeenAt);
        }
    }
}

