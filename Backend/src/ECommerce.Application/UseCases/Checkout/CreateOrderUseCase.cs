using ECommerce.Application.DTOs.Requests;
using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.UseCases.Checkout;

public class CreateOrderUseCase(ICurrentUser currentUser, IOrderRepository orderRepository, IProductRepository productRepository, ICartRepository cartRepository)
{
    public async Task<CreateOrderResponse> Execute(CreateOrderRequest request)
    {
        var address = request.Address;
        
        if(address is null)
            throw new ArgumentNullException(nameof(address));
        
        var user = await currentUser.GetUserAsync();
        
        var cart = await cartRepository.GetActiveByUserId(user.Id);
        
        if (cart is null)
            throw new InvalidOperationException("Cart not found.");

        if(cart.Items.Count == 0)
            #warning implement domain exception
            throw new InvalidOperationException("Cart is empty.");
        

        foreach (var item in cart.Items)
        {
            if(item.Quantity.Value > item.Product.Stock.Value)
                #warning implement domain exception
                throw new InvalidOperationException("Product is out of stock.");
        }
        
        var order = Order.Create(user, address);
        
        order.AddOrderItems(cart);
        
        await orderRepository.Save(order);
        
        return new CreateOrderResponse(
            order.Id,
            order.Total.Amount
        );
    }
}