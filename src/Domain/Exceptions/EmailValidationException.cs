namespace Domain.Exceptions;

public sealed class EmailValidationException : DomainException
{
    public EmailValidationException(string email, string message)
        : base(message)
    {
    }
    
    public static EmailValidationException InvalidFormat(string email)
        => new EmailValidationException(email, $"Invalid email format: {email}");
    
    public static EmailValidationException AlreadyExists(string email)
        => new EmailValidationException(email, $"Email already exists: {email}");
}