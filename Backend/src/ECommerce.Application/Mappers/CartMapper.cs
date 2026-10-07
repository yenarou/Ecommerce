using ECommerce.Application.DTOs.Requests;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.Mappers;

public class CartMapper(IProductRepository productRepository)
{
    public async Task<Cart> ResponseToCart(
        User user,
        CartRequest cartRequest)
    {
        var cart = Cart.Create(user);

        foreach (var itemRequest in cartRequest.Items)
        {
            var product = await productRepository.GetById(
                itemRequest.ProductId
            );

            if (product is null)
                throw new KeyNotFoundException(
                    $"Product {itemRequest.ProductId} not found."
                );

            var customization = Customization.Create(
                itemRequest.Customization.PersonalizationDescription,
                itemRequest.Customization.Wrap
            );
            
            var quantity = Quantity.Create(itemRequest.Quantity);

            cart.AddCartItem(
                product,
                quantity,
                customization
            );
        }

        return cart;
    }
}