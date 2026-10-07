using ECommerce.Application.DTOs.Requests;
using ECommerce.Application.Factories;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.UseCases.Checkout;

public class UpdateCartUseCase(ICurrentUser currentUser, ICartRepository cartRepository, CartFactory cartFactory)
{
    public async Task Execute(CartRequest request)
    {
        var user = currentUser.User;
        
        var cart = await cartRepository.GetActiveByUserId(user.Id);
        
        if(cart is null)
            cart = Cart.Create(user);
        
        var newCart = await cartFactory.Create(user, request);

        cart.UpdateItems(newCart.Items);
        
        await cartRepository.Save(cart);
    }
}