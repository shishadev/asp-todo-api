using Common;
using Domain.Models;
using Domain.ValueObjects;

namespace Domain.Repositories;

public interface IUserRepository
{
    Task AddUserAsync(User user, CancellationToken cancellationToken);
    
    Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken);
    
    Task<Option<User>> GetUserAsync(Email email, CancellationToken cancellationToken);
}