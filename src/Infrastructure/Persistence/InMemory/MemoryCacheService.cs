using Common;
using Domain.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Persistence.InMemory;

public sealed class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<Option<string>> Get(string key)
    {
        var isSuccess = _cache.TryGetValue(key, out string? result);

        if (!isSuccess || string.IsNullOrWhiteSpace(result))
        {
            return Task.FromResult(Option<string>.None());
        }
        
        return Task.FromResult(new Option<string>(result));
    }

    public Task<Option<string>> Get(ReadOnlySpan<char> key)
    {
        return Get(key.ToString());
    }

    public Task Set(string key, string value, TimeSpan expiration)
    {
        _ = _cache.Set(key, value, expiration);

        return Task.CompletedTask;
    }

    public Task Set(string value, TimeSpan expiration)
    {
        _ = _cache.Set(value, value, expiration);

        return Task.CompletedTask;
    }

    public Task Remove(string key)
    {
        _cache.Remove(key);
        
        return Task.CompletedTask;
    }
}