using Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public sealed record Password
{
    private Password(string password)
    {
        Value = password;
    }
    
    public string Value { get; }

    public static Result<Password, PasswordValidationException> Create(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return new PasswordValidationException(password, "Password cannot be empty");
        }

        var trimmedPassword = password.Trim();
        
        if (trimmedPassword.Length < 3)
        {
            return new PasswordValidationException(password, "Password must contain at least 3 characters");
        }

        return new Password(trimmedPassword);
    }
}