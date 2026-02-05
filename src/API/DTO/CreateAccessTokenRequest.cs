namespace API.DTO;

public class CreateAccessTokenRequest
{
    public required string UserId { get; init; }
    public required string Email { get; init; }
}