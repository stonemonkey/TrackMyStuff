using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TrackMyStuff.ApiGateway.DataAccess;

namespace TrackMyStuff.ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceStatusController : ControllerBase
    {
        private readonly ApiContext _ctx;
        private readonly ILogger<DeviceStatusController> _logger;

        public DeviceStatusController(ApiContext ctx, ILogger<DeviceStatusController> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<DeviceStatus>> Get(string id)
        {
            var result = await _ctx.DeviceStatus.FindAsync(id);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }
    }
}
