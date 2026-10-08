using ECommerce.Application.DTOs.Requests;
using ECommerce.Domain.Models;

namespace ECommerce.Application.Interfaces.Factories;

public interface ICartFactory
{
    Task<Cart> Create(
        User user,
        CartRequest request);
}