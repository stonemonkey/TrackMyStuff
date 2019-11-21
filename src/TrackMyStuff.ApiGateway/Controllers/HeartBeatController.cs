using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TrackMyStuff.Common.Commands;
using Microsoft.Azure.ServiceBus;
using System.Text;
using Newtonsoft.Json;

namespace TrackMyStuff.ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HeartBeatController : ControllerBase
    {
        private readonly IQueueClient _queueClient;
        private readonly ILogger<DeviceStatusController> _logger;

        public HeartBeatController(IQueueClient queueClient, ILogger<DeviceStatusController> logger)
        {
            _queueClient = queueClient;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] HeartBeatCommand command)
        {
            var json = JsonConvert.SerializeObject(command);
            await _queueClient.SendAsync(new Message(Encoding.UTF8.GetBytes(json)));
            return Accepted();
        }
    }
}
