using Common;
using Domain.Services;

namespace Infrastructure.Persistence.Redis;

public sealed class RedisCacheService : ICacheService
{
    public Task<Option<string>> Get(string key)
    {
        throw new NotImplementedException();
    }

    public Task Set(string key, string value, TimeSpan expiration)
    {
        throw new NotImplementedException();
    }
}