using Microsoft.AspNetCore.Mvc;

namespace ASWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IndexController : ControllerBase
    {
        private readonly ILogger<IndexController> _logger;

        public IndexController(ILogger<IndexController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "/")]
        public IActionResult Get()
        {
            return Ok(new { message = "App is running on port 5000"});
        }
    }
}
