using Microsoft.AspNetCore.Mvc;

namespace DFWeb.BE.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitorController : ControllerBase
    {
        [HttpGet]
        [Route("Ping")]
        public ActionResult<string> Ping()
        {
            return Ok("PONG");
        }
    }
}
