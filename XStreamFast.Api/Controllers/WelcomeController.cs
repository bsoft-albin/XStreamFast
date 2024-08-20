using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XStreamFast.Frameworks.CommonMeths;
using XStreamFast.Frameworks.CommonProps;
using XStreamFast.Models.Responses;

namespace XStreamFast.Api.Controllers
{
    /// <summary>
    /// A Welcome Controller for XStreamFast Web Services.
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [ApiVersion(XStreamFastApiRoutes.Versions.Latest)]
    [ApiVersion(XStreamFastApiRoutes.Versions.DEFAULT)]
    [Route(XStreamFastApiRoutes.Templates.ApiVersionTemplate)]
    public class WelcomeController(IWebHostEnvironment _hostEnvironment) : BaseController
    {
        private readonly IWebHostEnvironment hostEnvironment = _hostEnvironment;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("/")]
        public ContentResult HomePage()
        {

            return HtmlResponseFormatter(HelperMeths.ReadHtmlFileAsString(hostEnvironment.WebRootPath, "index.html"));
        }

        /// <summary>
        /// Endpoint to Welcome User.
        /// </summary>
        /// <returns>Welcome Message</returns>
        [HttpGet]
        [ActionName("WelcomeUser")]
        [MapToApiVersion(XStreamFastApiRoutes.Versions.DEFAULT)]
        public String WelcomeUser() {

            return "Welcome to XStreamFast WebServices!!!";
        }

        /// <summary>
        /// Endpoint to Greet User.
        /// </summary>
        /// <returns>Greeting Message from XStreamFast Server</returns>
        [HttpGet]
        [ActionName("GreetUser")]
        [MapToApiVersion(XStreamFastApiRoutes.Versions.DEFAULT)]
        public String GreetUser([FromQuery] String userName)
        {
            DateTime getTime = DateTime.Now;
            int hour = getTime.Hour;
            String greet = "";

            if (hour >= 6 && hour < 12)
            {
                greet = "Morning";
            }
            else if (hour >= 12 && hour < 17)
            {
                greet = "Afternoon";
            }
            else if (hour >= 17 && hour < 21)
            {
                greet = "Evening";
            }
            else
            {
                greet = "Night";
            }

            return $"Good {greet + " " + userName} from XStreamFast WebServices!!!";
        }

        /// <summary>
        /// Endpoint to Welcome User With their Name.
        /// </summary>
        /// <param name="getName"></param>
        /// <returns>Welcome Message with thier Name</returns>
        [HttpGet]
        [ActionName("WelcomeUserWithName")]
        [MapToApiVersion(XStreamFastApiRoutes.Versions.Latest)]
        public String WelcomeUser([FromQuery] String getName)
        {
         
            return $"Welcome {getName} to Our XStreamFast WebServices!!!";
        }

        [HttpGet("text")]
        public IActionResult GetTextResponse()
        {
            string textContent = "Hello, World!";
            return TextResponseFormatter(textContent);
        }
        [HttpGet("html")]
        public IActionResult GetHtmlResponse()
        {
            string htmlContent = "<html><body><h1>Hello, World!</h1></body></html>";
            return HtmlResponseFormatter(htmlContent);
        }
        [HttpGet("xml")]
        public IActionResult GetXmlResponse()
        {
            var data = new Person { Name = "John", Age = 30 };
            return XmlResponseFormatter(data);
        }

        [HttpGet("csv")]
        public IActionResult GetCsvResponse()
        {
            var data = new List<object>
            {
                new { Name = "John", Age = 30 },
                new { Name = "Jane", Age = 25 }
            };
            return new CsvResult(data);
        }
    }
}
