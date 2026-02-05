namespace Domain.Services;

public interface ITokenGenerator
{
    string Generate(string userId, string email);

    string GenerateRegistrationToken(string clientId, string clientName);
}