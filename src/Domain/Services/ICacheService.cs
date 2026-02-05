using Common;

namespace Domain.Services;

public interface ICacheService
{
    Task<Option<string>> Get(string key);
    
    Task Set(string key, string value, TimeSpan expiration);
}