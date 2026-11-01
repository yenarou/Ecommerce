namespace ECommerce.Application.DTOs;

public record PayPalCaptureResult(
    string PayPalOrderId,
    string Status,
    string? CaptureId,
    decimal Amount,
    string Currency,
    string? ReferenceId);