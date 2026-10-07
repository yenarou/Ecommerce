using ECommerce.Application.DTOs.Requests;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.Factories;

public class CartFactory(IProductRepository productRepository)
{
    public async Task<Cart> Create(
        User user,
        CartRequest request)
    {
        var cart = Cart.Create(user);

        foreach (var item in request.Items)
        {
            var product = await productRepository.GetById(
                item.ProductId
            );

            if (product is null)
            {
                throw new KeyNotFoundException(
                    $"Product {item.ProductId} not found."
                );
            }

            var quantity = Quantity.Create(item.Quantity);

            var customization = Customization.Create(
                item.Customization.PersonalizationDescription,
                item.Customization.Wrap
            );

            cart.AddCartItem(
                product,
                quantity,
                customization
            );
        }

        return cart;
    }
}