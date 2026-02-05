using API.DTO;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ICacheService _cacheService;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ICacheService cacheService, ITokenGenerator tokenGenerator, ILogger<AuthController> logger)
    {
        _cacheService = cacheService;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }

    [Authorize]
    [HttpPost("token")]
    [ProducesResponseType(typeof(CreateTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateTokenResponse>> CreateAccessToken([FromBody] CreateAccessTokenRequest request)
    {
        var claims = HttpContext.User.Claims;
        
        if (claims.FirstOrDefault(claim => claim.Type == "clientId")?.Value != "client-app")
        {
            return Unauthorized();
        }
        
        var token = await Task.Run(() => _tokenGenerator.Generate(request.UserId, request.Email));

        var response = new CreateTokenResponse(token);

        _logger.LogInformation("Created token for email: {email}", request.Email);

        return CreatedAtAction(nameof(CreateAccessToken), response);
    }
    
    [HttpPost("registration-token")]
    [ProducesResponseType(typeof(CreateTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateTokenResponse>> CreateRegistrationToken([FromBody] CreateRegistrationTokenRequest request)
    {
        var token = await Task.Run(() => _tokenGenerator.GenerateRegistrationToken(request.ClientId, request.ClientName));

        var response = new CreateTokenResponse(token);

        _logger.LogInformation("Created token for client: {clientId}", request.ClientId);

        return CreatedAtAction(nameof(CreateRegistrationToken), response);
    }
}