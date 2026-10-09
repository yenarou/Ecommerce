namespace ECommerce.Application.DTOs.Responses;

public record PayOrderResponse(
    Guid OrderId,
    string Status,
    string? StatusDetail,
    string? TicketUrl,
    string? Reference,
    bool Paid);