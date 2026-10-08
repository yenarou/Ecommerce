using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Mappers;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.UseCases.Checkout;

public class GetUserOrderHistoryUseCase(ICurrentUser currentUser, IOrderRepository orderRepository)
{
    public async Task<ICollection<OrderResponse>> Execute()
    {
        var user = await currentUser.GetUserAsync();

        var orders = await orderRepository.GetByUserId(user.Id);
        
        var result = OrderMapper.ToResponse(orders);
        
        return result;
    }
}