using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.UseCases.Admin.Orders;

public record AdminOrderItemResponse(
    Guid Id,
    string Name,
    int Quantity
);

public record AdminOrderResponse(
    Guid Id,
    string CustomerName,
    string CustomerEmail,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string Status,
    decimal Total,
    string Currency,
    IReadOnlyList<AdminOrderItemResponse> Items
){
    public static AdminOrderResponse From(Order order) => new(
        order.Id,
        order.User.Name,
        order.User.Email.Value,
        order.CreatedAt,
        order.UpdatedAt,
        order.Status.ToString(),
        order.Total.Amount,
        order.Total.Currency,
        order.Items
            .Select(i => new AdminOrderItemResponse(i.Id, i.ProductName, i.Quantity.Value))
            .ToList()
    );
}

public class GetAdminOrdersUseCase(IOrderRepository orderRepository)
{
    public async Task<ICollection<AdminOrderResponse>> Execute()
    {
        var orders = await orderRepository.GetAll();
        return orders.Select(AdminOrderResponse.From).ToList();
    }
}

public class GetAdminOrderUseCase(IOrderRepository orderRepository)
{
    public async Task<AdminOrderResponse?> Execute(Guid orderId)
    {
        var order = await orderRepository.GetById(orderId);
        return order is null ? null : AdminOrderResponse.From(order);
    }
}