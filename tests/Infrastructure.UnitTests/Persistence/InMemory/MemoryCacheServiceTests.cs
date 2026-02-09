using Infrastructure.Persistence.InMemory;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.UnitTests.Persistence.InMemory;

public class MemoryCacheServiceTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("notExisting")]
    public async Task Get_WhenKeyNotExist_ReturnNotSuccessfulOption(string key)
    {
        var service = CreateService();

        var result = await service.Get(key);
        
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Get_WhenKeyExistsAndValueNotValid_ReturnNotSuccessfulOption(string? invalidValue)
    {
        const string key = "existingKey";
        using var innerCache = CreateInnerCache();
        var service = CreateService(innerCache);
        await service.Set(key, "value", TimeSpan.FromMinutes(1));
        innerCache.Set(key, invalidValue, TimeSpan.FromMinutes(1));

        var result = await service.Get(key);
        
        Assert.False(result.IsSuccess);
    }
    
    [Theory]
    [InlineData("value1")]
    [InlineData("newValue2")]
    public async Task Get_WhenKeyExistsAndValueValid_ReturnExpectedValue(string expectedValue)
    {
        const string key = "existingKey";
        var service = CreateService();
        await service.Set(key, expectedValue, TimeSpan.FromMinutes(1));

        var result = await service.Get(key);
        
        Assert.Equal(expectedValue, result.Value);
    }
    
    [Fact]
    public async Task Remove_WhenKeyExists_GetReturnsNotSuccessfulOption()
    {
        const string key = "existingKey";
        var service = CreateService();
        await service.Set(key, "val", TimeSpan.FromMinutes(1));

        await service.Remove(key);
        
        var result = await service.Get(key);
        Assert.False(result.IsSuccess);
    }
    
    [Fact]
    public async Task Remove_WhenKeyNotExist_GetReturnsNotSuccessfulOption()
    {
        const string key = "notExistingKey";
        var service = CreateService();

        await service.Remove(key);
        
        var result = await service.Get(key);
        Assert.False(result.IsSuccess);
    }
    
    private static MemoryCacheService CreateService()
    {
#pragma warning disable CA2000
        var cache = CreateInnerCache();
#pragma warning restore CA2000
        
        return CreateService(cache);
    }
    
    private static MemoryCacheService CreateService(IMemoryCache cache)
    {
        return new MemoryCacheService(cache);
    }

    private static MemoryCache CreateInnerCache()
    {
#pragma warning disable CA2000
        return new MemoryCache(new MemoryCacheOptions());
#pragma warning restore CA2000
    }
}