using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Threading.Tasks;
using TrackMyStuff.ApiGateway.Queries;
using TrackMyStuff.Common.Commands;

namespace TrackMyStuff.ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceStatusController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DeviceStatusController> _logger;

        public DeviceStatusController(IMediator mediator, ILogger<DeviceStatusController> logger,
            ICommandHandler<HeartBeatCommand> handler)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<DeviceStatus>> Get(string id)
        {
            var result = await _mediator.Send(new GetDeviceStatusQuery { DeviceId = id });
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }
    }
}
