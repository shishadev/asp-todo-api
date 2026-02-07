using System.Text.RegularExpressions;
using Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public sealed partial record Email
{
    private Email(string value)
    {
        Value = value;
    }
    
    public string Value { get; }

    public static Result<Email, EmailValidationException> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return new EmailValidationException(email, "Email cannot be empty");
        }

        var trimmedEmail = email.Trim();
        
        if (!EmailFormatRegex().IsMatch(trimmedEmail))
        {
            return EmailValidationException.InvalidFormat(email);
        }

        return new Email(trimmedEmail);
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailFormatRegex();
}