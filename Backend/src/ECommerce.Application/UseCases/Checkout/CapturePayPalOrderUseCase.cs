using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.UseCases.Checkout;

public class CapturePayPalOrderUseCase(
    ICurrentUser currentUser,
    IOrderRepository orderRepository,
    IPayPalGateway payPal,
    ConfirmOrderPaymentUseCase confirmOrderPayment)
{
    public async Task<PayOrderResponse> Execute(Guid orderId, string payPalOrderId)
    {
        var user = await currentUser.GetUserAsync();
        var order = await orderRepository.GetById(orderId);

        if (order is null || order.User.Id != user.Id)
            throw new InvalidOperationException("Order not found.");

        if (!order.Status.Equals(OrderStatus.Pending))
            throw new InvalidOperationException("Order is not pending payment.");

        var capture = await payPal.CaptureOrder(payPalOrderId);

        // comprobacion de dicha orden
        var total = order.GetTotal();
        var matches =
            capture.ReferenceId == order.Id.ToString() &&
            capture.Amount == Math.Round(total.Amount, 2) &&
            string.Equals(capture.Currency, total.Currency, StringComparison.OrdinalIgnoreCase);

        var paid = capture.Status == "COMPLETED" && matches;

        if (paid)
            await confirmOrderPayment.Execute(order.Id);

        var status = paid ? "processed" : capture.Status == "PENDING" ? "pending" : "failed";
        var detail = matches ? capture.Status : "amount_mismatch";

        return new PayOrderResponse(order.Id, status, detail, null, capture.CaptureId, paid);
    }
}