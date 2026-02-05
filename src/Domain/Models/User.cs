using Common;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Models;

public sealed class User
{
    private User(UserId id, string name, Email email, string passwordHash, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }
    
    public UserId Id { get; }
    
    public string Name { get; }
    
    public Email Email { get; }
    
    public string PasswordHash { get; }

    public DateTime CreatedAt { get; }
    
    public DateTime? UpdatedAt { get; }
    
    public static Result<User, UserValidationException> Create(UserId id, string name, Email email,
        string passwordHash, DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new UserValidationException("Username cannot be empty");
        }
        
        var trimmedName = name.Trim();

        if (trimmedName.Length < 3)
        {
            return new UserValidationException("Username must have at least 3 characters");
        }
        
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return new UserValidationException("Password hash cannot be empty");
        }
        
        return new User(id, name, email, passwordHash.Trim(), createdAt);
    }
}