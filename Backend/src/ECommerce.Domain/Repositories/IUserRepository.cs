using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetById(Guid userId);
    Task<User?> GetByEmail(string email);
    Task<User?> GetByGoogleId(string googleId);
    Task Save(User user);
    Task Delete(Guid userId);
}