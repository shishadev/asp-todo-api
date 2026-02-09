using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using API.DTO;
using API.IntegratedTests.Base;
using Xunit;

namespace API.IntegratedTests.Controllers;

[Collection("ControllerTests")] // to use one WebApplicationFactory for such tests
public class AuthControllerIntegratedTests(TestWebApplicationFactory factory) : ControllerTestsBase(factory)
{
    private const string ValidClientId = "client-app";
    private static readonly Uri RegistrationTokenUrl = new("/api/auth/registration-token", UriKind.Relative);
    private static readonly Uri AccessTokenUrl = new("/api/auth/token", UriKind.Relative);

    [Fact]
    public async Task CreateRegistrationToken_Always_ReturnExpectedResponse()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var request = CreateRegistrationTokenRequest("clientId", "clientName");
        using var requestContent = CreatePostContent(request);
        
        var response = await HttpClient.PostAsync(RegistrationTokenUrl, requestContent, cancellationToken);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var responseDto = await response.Content.ReadFromJsonAsync<CreateTokenResponse>(cancellationToken);
        Assert.NotNull(responseDto);
        Assert.False(string.IsNullOrWhiteSpace(responseDto.Token));
    }

    [Fact]
    public async Task CreateAccessToken_Always_ReturnExpectedResponse()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var registrationToken = await SendCreateRegistrationTokenRequest(cancellationToken);
        var request = CreateAccessTokenRequest("userId", "email");
        using var requestMessage = CreateRequestMessage(AccessTokenUrl, request, registrationToken);
        
        var response = await HttpClient.SendAsync(requestMessage, cancellationToken);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var responseDto = await response.Content.ReadFromJsonAsync<CreateTokenResponse>(cancellationToken);
        Assert.NotNull(responseDto);
        Assert.False(string.IsNullOrWhiteSpace(responseDto.Token));
        Assert.NotEqual(registrationToken, responseDto.Token);
    }

    [Fact]
    public async Task CreateAccessToken_WhenSendRequestWithSameTokenTwice_ReturnUnauthorizedResponse()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var registrationToken = await SendCreateRegistrationTokenRequest(cancellationToken);
        var request = CreateAccessTokenRequest("userId", "email");
        using var firstRequestMessage = CreateRequestMessage(AccessTokenUrl, request, registrationToken);
        var firstResponse = await HttpClient.SendAsync(firstRequestMessage, cancellationToken);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        using var secondRequestMessage = CreateRequestMessage(AccessTokenUrl, request, registrationToken);
        
        var secondResponse = await HttpClient.SendAsync(secondRequestMessage, cancellationToken);
        
        Assert.Equal(HttpStatusCode.Unauthorized, secondResponse.StatusCode);
    }
    
    private static CreateRegistrationTokenRequest CreateRegistrationTokenRequest(string clientId, string clientName)
    {
        return new CreateRegistrationTokenRequest
        {
            ClientId = clientId,
            ClientName = clientName
        };
    }
    
    private static CreateAccessTokenRequest CreateAccessTokenRequest(string userId, string email)
    {
        return new CreateAccessTokenRequest
        {
            UserId = userId,
            Email = email
        };
    }

    private async Task<string> SendCreateRegistrationTokenRequest(CancellationToken cancellationToken)
    {
        var request = CreateRegistrationTokenRequest(ValidClientId, "clientName");
        using var requestContent = CreatePostContent(request);
        
        var response = await HttpClient.PostAsync(RegistrationTokenUrl, requestContent, cancellationToken);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var responseDto = await response.Content.ReadFromJsonAsync<CreateTokenResponse>(cancellationToken);
        Assert.NotNull(responseDto);

        return responseDto.Token;
    }
    
    private static StringContent CreatePostContent(object content)
    {
        var json = JsonSerializer.Serialize(content);
        
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static HttpRequestMessage CreateRequestMessage(Uri requestUri, object content, string jwtToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = CreatePostContent(content),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", jwtToken) }
        };
        
        return request;
    }
}