using Domain.Services;
using Infrastructure.Persistence.InMemory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace API.IntegratedTests.Base;

// ReSharper disable once ClassNeverInstantiated.Global
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Отключите аутентификацию если нужно
            // services.AddAuthentication("Test")
            //     .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
            //         "Test", options => { });

            services.Replace(ServiceDescriptor.Scoped<ICacheService, MemoryCacheService>());
        });
    }
}