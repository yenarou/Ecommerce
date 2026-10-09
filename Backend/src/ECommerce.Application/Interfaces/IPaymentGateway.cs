using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces;

public interface IPaymentGateway
{
    Task<PaymentResult> CreatePaymentAsync(PaymentRequest request);
    Task<PaymentResult> GetPaymentAsync(string gatewayOrderId);
}