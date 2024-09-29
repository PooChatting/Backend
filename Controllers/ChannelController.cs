using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poochatting.Services;

namespace Poochatting.Controllers
{
    [Route("api/channel")]
    [Authorize]
    public class ChannelController : ControllerBase
    {
        private readonly IChannelService _channelService;
        public ChannelController(IChannelService channelService)
        {
            _channelService = channelService;
        }

        [HttpPut("{channelId}/user")]
        public ActionResult AddUser([FromRoute] int channelId)
        {
            _channelService.addToChannel(channelId);

            return Ok();
        }

    }
}
