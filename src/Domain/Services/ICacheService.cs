using Common;

namespace Domain.Services;

public interface ICacheService
{
    Task<Option<string>> Get(string key);
    
    Task<Option<string>> Get(ReadOnlySpan<char> key);
    
    Task Set(string key, string value, TimeSpan expiration);
    
    Task Set(string value, TimeSpan expiration);
    
    Task Remove(string key);
}