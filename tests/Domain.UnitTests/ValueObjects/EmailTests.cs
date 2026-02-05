using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

public sealed class EmailTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WhenEmailValueInvalid_ReturnNotSuccessfulResult(string? email)
    {
        var result = Email.Create(email!);
        
        Assert.False(result.IsSuccess);
    }
    
    [Theory]
    [InlineData(" @mail.ru  ")]
    [InlineData("  mail.com ")]
    [InlineData("my@mail")]
    [InlineData("my mail")]
    public void Create_WhenEmailFormatInvalid_ReturnNotSuccessfulResult(string email)
    {
        var result = Email.Create(email);
        
        Assert.False(result.IsSuccess);
    }
    
    [Theory]
    [InlineData(" my@mail.ru   ")]
    [InlineData("  some_mail@mail.comm")]
    public void Create_WhenEmailValid_ReturnEmailWithTrimmedValue(string emailValue)
    {
        var expectedResult = emailValue.Trim();
        
        var result = Email.Create(emailValue);

        var actualResult = result.Match(string? (email) => email.Value, _ => null);
        Assert.Equal(expectedResult, actualResult);
    }
}