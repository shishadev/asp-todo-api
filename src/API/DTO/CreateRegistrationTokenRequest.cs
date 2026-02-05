namespace API.DTO;

public sealed class CreateRegistrationTokenRequest
{
    public required string ClientId { get; init; }
    
    public required string ClientName { get; init; }
}