using Microsoft.AspNetCore.Mvc;

namespace XStreamFast.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WelcomeController : ControllerBase
    {
        [HttpGet]
        [ActionName("WelcomeUser")]
        public String WelcomeUser() {

            return "Welcome to XStreamFast WebServices!!!";
        }

        [HttpGet]
        [ActionName("WelcomeUserWithName")]
        public String WelcomeUser([FromQuery] String getName)
        {

            return $"Welcome {getName}!!!";
        }
    }
}
