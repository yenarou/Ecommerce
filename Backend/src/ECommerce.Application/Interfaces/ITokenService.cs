using ECommerce.Domain.Models;

namespace ECommerce.Application.Interfaces;

public interface ITokenService
{
    string Generate(User user);
}