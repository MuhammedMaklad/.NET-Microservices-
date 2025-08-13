using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
  [Route("/api/c/[controller]")]
  [ApiController]
  public class PlatformController : ControllerBase
  {
    public PlatformController()
    {
      // Synchronous & Asynchronous Messaging 
    }
    [HttpPost]
    public ActionResult TestInboundConnection()
    {
      Console.WriteLine($"---> Inbound Post # Command Service");
      return Ok("Inbound test of from Platforms Controller");
    }
  }
}
