using Microsoft.AspNetCore.Mvc;
using XStreamFast.Frameworks.CommonProps;

namespace XStreamFast.Api.Controllers
{
    [ApiController]
    [ApiVersion(XStreamFastApiRoutes.Versions.Latest)]
    [Route(XStreamFastApiRoutes.Templates.ApiVersionTemplate)]
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
