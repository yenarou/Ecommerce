using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.UseCases.Admin.Orders;

public class UpdateOrderStatusUseCase(
    IOrderRepository orderRepository)
{
    public async Task Execute(
        Guid orderId,
        string status)
    {
        var order = await orderRepository.GetById(orderId);

        if (order is null)
            throw new ArgumentException(
                $"Order with id '{orderId}' was not found.");

        var newStatus = OrderStatus.FromString(status);

        order.UpdateStatus(newStatus);

        await orderRepository.Save(order);
    }
}