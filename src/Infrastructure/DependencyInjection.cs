using Domain.Services;
using Infrastructure.Persistence.Redis;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // services.AddDbContext<ApplicationDbContext>(options =>
        //     options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        //services.AddScoped<IUserRepository, UserRepository>();

        //services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<ICacheService, RedisCacheService>();
        
        return services;
    }
}