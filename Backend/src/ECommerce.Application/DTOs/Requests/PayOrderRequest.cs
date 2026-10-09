namespace ECommerce.Application.DTOs.Requests;

public record PayOrderRequest(
    Guid OrderId,
    string PaymentOption,
    PayerDto Payer,
    CardDto? Card);