using ECommerce.Application.DTOs.Responses;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Mappers;

public static class OrderMapper
{
    public static OrderResponse ToResponse(this Order order)
    {
        return new OrderResponse(
            order.Id,
            order.CreatedAt,
            order.UpdatedAt,
            order.Address.ToResponse(),
            order.Status,
            order.Items
                .Select(item => item.ToResponse())
                .ToList()
        );
    }
    
    public static
        ICollection<OrderResponse> ToResponse(
            ICollection<Order> orders)
    {
        return orders
            .Select(ToResponse)
            .ToList();
    }
}