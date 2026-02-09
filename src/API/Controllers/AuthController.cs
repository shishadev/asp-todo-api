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
    [ProducesResponseType(typeof(CreateTokenResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateTokenResponse>> CreateAccessToken([FromBody] CreateAccessTokenRequest request)
    {
        var claims = HttpContext.User.Claims;
        
        if (claims.FirstOrDefault(claim => claim.Type == "clientId")?.Value != "client-app")
        {
            _logger.LogDebug("Request with not valid claim type");
            
            return Unauthorized();
        }
        
        var authorizationHeader = HttpContext.Request.Headers.Authorization.ToString();
    
        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            _logger.LogDebug("Cannot get authorization header");
            
            return Unauthorized();
        }
        
        ReadOnlySpan<char> bearerPrefix = "Bearer ";
        
        var registrationTokenSpan = authorizationHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase)
            ? authorizationHeader.AsSpan(bearerPrefix.Length)
            : authorizationHeader.AsSpan();

        var registrationTokenResult = await _cacheService.Get(registrationTokenSpan.Trim());

        if (registrationTokenResult.IsSuccess)
        {
            await _cacheService.Remove(registrationTokenResult.Value);
        }
        else
        {
            _logger.LogDebug("Token not registered");
            
            return Unauthorized();
        }
        
        var accessToken = await Task.Run(() => _tokenGenerator.Generate(request.UserId, request.Email));

        var response = new CreateTokenResponse(accessToken);

        _logger.LogInformation("Created token for email: {email}", request.Email);

        return CreatedAtAction(nameof(CreateAccessToken), response);
    }
    
    [HttpPost("registration-token")]
    [ProducesResponseType(typeof(CreateTokenResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateTokenResponse>> CreateRegistrationToken([FromBody] CreateRegistrationTokenRequest request)
    {
        var token = await Task.Run(() => _tokenGenerator.GenerateRegistrationToken(request.ClientId, request.ClientName));
        
        await _cacheService.Set(token, TimeSpan.FromMinutes(1));

        var response = new CreateTokenResponse(token);

        _logger.LogInformation("Created token for client: {clientId}", request.ClientId);

        return CreatedAtAction(nameof(CreateRegistrationToken), response);
    }
}