using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poochatting.DbContext.Entities;
using Poochatting.Entities;
using Poochatting.Models;
using Poochatting.Models.Queries;
using Poochatting.Services;

namespace Poochatting.Controllers
{
    [Route("api/message")]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }
        // TO DO: change this to something like api/message?channel={id} and query selections

        [HttpGet("channel/{channelId}")]
        public async Task<ActionResult<IEnumerable<MessageModel>>> GetMessagesAsync([FromRoute] int channelId, [FromQuery] MessageQueryParams paginationParameters)
        {
            var messages = await _messageService.GetAll(channelId, paginationParameters);

            return Ok(messages);
        }
        
        // Propably don't need this

        //[HttpGet("{id}")]
        //public ActionResult<Message> Get([FromRoute] int id)
        //{
        //    var message = _messageService.GetById(id);

        //    return Ok(message);
        //}
        [HttpPost]
        public ActionResult<Message> PostMessage([FromBody] CreateMessageDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = _messageService.PostMessageAsync(dto);

            return Created("", null);
        }

        [HttpPut]
        public ActionResult<Message> PutMessage([FromBody] EditMessageDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _messageService.PutMessage(dto, User);

            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteMessage([FromRoute] int id) 
        { 
            _messageService.Delete(id, User);

            return Ok();

        }
    }
}
