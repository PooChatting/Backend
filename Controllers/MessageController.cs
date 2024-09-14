using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poochatting.Entities;
using Poochatting.Models;
using Poochatting.Services;

namespace Poochatting.Controllers
{
    [Route("api/message")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<MessageDto>> GetAll()
        {
            var messages = _messageService.GetAll();
            
            return Ok(messages);
        }
        [HttpGet("{id}")]
        public ActionResult<Message> Get([FromRoute] int id)
        {
            var message = _messageService.GetById(id);

            return Ok(message);
        }
        [HttpPost]
        public ActionResult<Message> PostMessage([FromBody] CreateMessageDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = _messageService.PostMessage(dto);

            return Created("", null);
        }

        [HttpPut]
        public ActionResult<Message> PutMessage([FromBody] EditMessageDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _messageService.PutMessage(dto);

            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteMessage([FromRoute] int id) 
        { 
            _messageService.Delete(id);

            return Ok();

        }
    }
}
