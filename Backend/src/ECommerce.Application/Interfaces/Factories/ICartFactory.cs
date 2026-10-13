using ECommerce.Application.DTOs.Requests;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Factories;

public interface ICartFactory
{
    Task<Cart> Create(
        User user,
        CartRequest request);
}