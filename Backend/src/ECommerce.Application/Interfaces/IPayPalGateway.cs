using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces;

public interface IPayPalGateway
{
    Task<string> CreateOrder(Guid orderId, decimal amount, string currency);
    Task<PayPalCaptureResult> CaptureOrder(string payPalOrderId);
}