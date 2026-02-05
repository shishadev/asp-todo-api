using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<string>> GetUser([FromBody] string id)
    {
        return null;
    }
}