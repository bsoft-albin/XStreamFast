using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XStreamFast.Frameworks.CommonProps;
using XStreamFast.Services.Interfaces;

namespace XStreamFast.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [ApiVersion(XStreamFastApiRoutes.Versions.Latest)]
    [Route(XStreamFastApiRoutes.Templates.ApiVersionTemplate)]
    public class PostgresController(IPostgresServices postgresServices) : ControllerBase
    {
        private readonly IPostgresServices _postgres = postgresServices;

        [HttpGet]
        public async Task<ActionResult> GetIdentification([FromQuery] int id)
        {
            return Ok(await _postgres.IdentifyUser(id));
        }

    }
}
