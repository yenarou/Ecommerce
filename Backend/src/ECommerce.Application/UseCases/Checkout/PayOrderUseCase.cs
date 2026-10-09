using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.Requests;
using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.UseCases.Checkout;

public class PayOrderUseCase(
    ICurrentUser currentUser,
    IOrderRepository orderRepository,
    IPaymentGateway paymentGateway,
    ConfirmOrderPaymentUseCase confirmOrderPayment)
{
    public async Task<PayOrderResponse> Execute(PayOrderRequest request)
    {
        var user = await currentUser.GetUserAsync();

        var order = await orderRepository.GetById(request.OrderId);

        if (order is null || order.User.Id != user.Id)
            throw new InvalidOperationException("Order not found.");

        if (!order.Status.Equals(OrderStatus.Pending))
            throw new InvalidOperationException("Order is not pending payment.");

        var amount = order.GetTotal().Amount;

        var result = await paymentGateway.CreatePaymentAsync(new PaymentRequest(
            order.Id,
            amount,
            request.PaymentOption,
            request.Payer,
            request.Card));

        // Con tarjeta, MP responde "processed" al instante si se aprobó.
        var paid = result.Status == "processed";

        if (paid)
            await confirmOrderPayment.Execute(order.Id);

        return new PayOrderResponse(
            order.Id,
            result.Status,
            result.StatusDetail,
            result.TicketUrl,
            result.Reference,
            paid);
    }
}