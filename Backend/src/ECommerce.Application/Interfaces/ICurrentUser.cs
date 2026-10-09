using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.Interfaces;
public interface ICurrentUser
{
    Task<User> GetUserAsync();
}