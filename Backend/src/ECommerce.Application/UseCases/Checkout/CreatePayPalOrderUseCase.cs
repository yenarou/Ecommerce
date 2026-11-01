using ECommerce.Application.Interfaces;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.UseCases.Checkout;

public class CreatePayPalOrderUseCase(
    ICurrentUser currentUser,
    IOrderRepository orderRepository,
    IPayPalGateway payPal)
{
    public async Task<string> Execute(Guid orderId)
    {
        var user = await currentUser.GetUserAsync();
        var order = await orderRepository.GetById(orderId);

        if (order is null || order.User.Id != user.Id)
            throw new InvalidOperationException("Order not found.");

        if (!order.Status.Equals(OrderStatus.Pending))
            throw new InvalidOperationException("Order is not pending payment.");

        // monto de orden guardada
        var total = order.GetTotal();
        return await payPal.CreateOrder(order.Id, Math.Round(total.Amount, 2), total.Currency);
    }
}