using Xunit;

namespace API.IntegratedTests.Base;

public abstract class ControllerTestsBase : IClassFixture<TestWebApplicationFactory>
{
    protected ControllerTestsBase(TestWebApplicationFactory factory)
    {
        WebApplicationFactory = factory;
        HttpClient = WebApplicationFactory.CreateClient();
    }

    // ReSharper disable once MemberCanBePrivate.Global
    protected TestWebApplicationFactory WebApplicationFactory { get; }
    
    protected HttpClient HttpClient { get; }
}