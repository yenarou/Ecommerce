using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Mappers;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.UseCases.Checkout;

public class GetUserOrderHistoryUseCase(ICurrentUser currentUser, IOrderRepository orderRepository, IProductRepository productRepository)
{
    public async Task<ICollection<OrderResponse>> Execute()
    {
        var user = await currentUser.GetUserAsync();

        var orders = await orderRepository.GetByUserId(user.Id);

        foreach (var order in orders)
        {
            foreach (var item in order.Items)
            {
                if (item.ProductId  == Guid.Empty)
                    throw new InvalidOperationException(
                        $"Order item {item.Id} has no stored product ID.");
            }
        }

        var result = OrderMapper.ToResponse(orders);
        
        return result;
    }
}