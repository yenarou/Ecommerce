using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces;

public interface ITokenService
{
    string Generate(User user);
}