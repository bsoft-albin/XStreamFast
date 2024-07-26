using Microsoft.AspNetCore.Mvc;
using XStreamFast.Frameworks.CommonProps;

namespace XStreamFast.Api.Controllers
{
    [ApiController]
    [ApiVersion(XStreamFastApiRoutes.Versions.Latest)]
    [ApiVersion(XStreamFastApiRoutes.Versions.DEFAULT)]
    [Route(XStreamFastApiRoutes.Templates.ApiVersionTemplate)]
    public class WelcomeController : ControllerBase
    {
        [HttpGet]
        [ActionName("WelcomeUser")]
        [MapToApiVersion(XStreamFastApiRoutes.Versions.DEFAULT)]
        public String WelcomeUser() {

            return "Welcome to XStreamFast WebServices!!!";
        }

        [HttpGet]
        [ActionName("WelcomeUserWithName")]
        [MapToApiVersion(XStreamFastApiRoutes.Versions.Latest)]
        public String WelcomeUser([FromQuery] String getName)
        {

            return $"Welcome {getName}!!!";
        }
    }
}
