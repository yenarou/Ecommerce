using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.UseCases.Checkout;

public class ConfirmOrderPaymentUseCase(
    IOrderRepository orderRepository,
    IProductRepository productRepository)
{
    public async Task Execute(Guid orderId)
    {
        var order = await orderRepository.GetById(orderId)
            ?? throw new InvalidOperationException("Order not found.");

        // si ya no está pendiente, no se vuelve a descontar nada
        if (!order.Status.Equals(OrderStatus.Pending))
            return;

        foreach (var item in order.Items)
        {
            var product = await productRepository.GetById(item.Product.Id)
                ?? throw new InvalidOperationException($"Product {item.Product.Id} not found.");

            product.UpdateStock(product.Stock.Decrease(item.Quantity.Value));

            await productRepository.Save(product);
        }

        order.UpdateStatus(OrderStatus.Confirmed);

        await orderRepository.Save(order);
    }
}