namespace ECommerce.Application.DTOs;

public record PayerDto(
    string Email,
    string FirstName,
    string LastName,
    string? IdentificationType,
    string? IdentificationNumber);

public record CardDto(
    string Token,
    string PaymentMethodId,
    string PaymentMethodType,
    int Installments);

public record PaymentRequest(
    Guid OrderId,
    decimal Amount,
    string PaymentOption,
    PayerDto Payer,
    CardDto? Card);

public record PaymentResult(
    string GatewayOrderId,
    Guid? OrderId,
    string Status,
    string? StatusDetail,
    string? TicketUrl,
    string? Reference);