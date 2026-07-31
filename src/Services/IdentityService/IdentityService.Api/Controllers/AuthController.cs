using IdentityService.Api.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Application.Models.LoginRequestModel requestModel)
        {
            var response = await _identityService.Login(requestModel);
            if (response == null)
            {
                return Unauthorized();
            }
            return Ok(response);
        }
    }
}
