using Microsoft.AspNetCore.Mvc;

namespace Netnol.Identity.Service.Controllers;

[ApiController]
[Route("/hello")]
public class HelloController : ControllerBase
{
    [HttpGet]
    public string Get() => "Hello, World!";
}