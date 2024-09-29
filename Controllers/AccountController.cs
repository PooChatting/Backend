using Microsoft.AspNetCore.Mvc;
using Poochatting.Models;
using Poochatting.Services;

namespace Poochatting.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _service;
        public AccountController(IAccountService service) 
        {
            _service = service;
        }
        [HttpPost]
        [Route("register")]
        public ActionResult RegisterUser([FromBody] RegisterUserDto dto)
        {
            _service.RegisterUser(dto);
            return Ok();
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginDto dto)
        {
            var authData = _service.GenerateJwt(dto);
            return Ok(authData);
        }

    }
}
