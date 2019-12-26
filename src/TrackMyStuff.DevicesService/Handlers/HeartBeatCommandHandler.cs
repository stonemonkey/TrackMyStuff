using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TrackMyStuff.Common.Commands;
using TrackMyStuff.Common.Events;
using TrackMyStuff.Common.ServiceBus;
using TrackMyStuff.DevicesService.DataAccess;

namespace TrackMyStuff.DevicesService.Handlers
{
    public class HeartBeatCommandHandler : ICommandHandler<HeartBeatCommand>
    {
        private readonly DevContext _ctx;
        private readonly ILogger _logger;
        private readonly IServiceBus _serviceBus;
        
        public HeartBeatCommandHandler(DevContext ctx, ILogger<HeartBeatCommandHandler> logger, IServiceBus serviceBus)
        {
            _ctx = ctx;
            _logger = logger;
            _serviceBus = serviceBus;
        }

        public async Task HandleAsync(HeartBeatCommand command)
        {
            var heartbeat = new HeartBeat { DeviceId = command.DeviceId, CreatedAt = DateTime.UtcNow };
            await _ctx.AddAsync(heartbeat);
            await _ctx.SaveChangesAsync();
            
            var updateEvent = new DeviceStatusUpdatedEvent { DeviceId = command.DeviceId, LastSeenAt = heartbeat.CreatedAt };
            await _serviceBus.PublishEventAsync(updateEvent);

            _logger.LogInformation("Device {0} heartbeat registred at {1}.", heartbeat.DeviceId, heartbeat.CreatedAt);
        }
    }
}

