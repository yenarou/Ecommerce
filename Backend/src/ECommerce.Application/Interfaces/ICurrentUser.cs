using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.Interfaces;
public interface ICurrentUser
{
    Task<User> GetUserAsync();
}