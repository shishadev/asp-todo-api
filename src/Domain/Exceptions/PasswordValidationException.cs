namespace Domain.Exceptions;

public class PasswordValidationException : DomainException
{
    public PasswordValidationException(string password, string message)
        : base(message)
    {
    }
}