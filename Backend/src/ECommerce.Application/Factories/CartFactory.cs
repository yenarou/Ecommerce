using ECommerce.Application.DTOs.Requests;
using ECommerce.Application.Interfaces.Factories;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.Factories;

public class CartFactory(IProductRepository productRepository) : ICartFactory
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

            var description = item.Customization?.PersonalizationDescription;
            var wrap = item.Customization?.Wrap ?? false;
            var customization = string.IsNullOrWhiteSpace(description) && !wrap
                ? null
                : Customization.Create(description, wrap);

            cart.AddCartItem(
                product,
                quantity,
                customization
            );
        }

        return cart;
    }
}