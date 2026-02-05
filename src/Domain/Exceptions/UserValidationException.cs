namespace Domain.Exceptions;

public class UserValidationException : DomainException
{
    public UserValidationException(string message) : base(message)
    {
    }
}