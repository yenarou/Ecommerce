using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Mappers;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.UseCases.Checkout;

public class GetActiveCartUseCase(ICurrentUser currentUser, ICartRepository cartRepository)
{
    public async Task<CartResponse> Execute()
    {
        var user = await currentUser.GetUserAsync();
        
        var cart = await cartRepository.GetActiveByUserId(user.Id);
        
        if(cart is null)
            cart = Cart.Create(user);

        var response = CartMapper.CartToResponse(cart);
        
        return response;
    }
}