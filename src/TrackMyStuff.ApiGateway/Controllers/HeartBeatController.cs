using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TrackMyStuff.Common.Commands;
using TrackMyStuff.Common.ServiceBus;

namespace TrackMyStuff.ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HeartBeatController : ControllerBase
    {
        private readonly IServiceBus _serviceBus;
        private readonly ILogger<DeviceStatusController> _logger;

        public HeartBeatController(IServiceBus serviceBus, ILogger<DeviceStatusController> logger)
        {
            _serviceBus = serviceBus;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] HeartBeatCommand command)
        {
            await _serviceBus.PublishCommandAsync(command);
            return Accepted();
        }
    }
}
