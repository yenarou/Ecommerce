using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Payments;

public class MercadoPagoGateway : IPaymentGateway
{
    private const string BaseUrl = "https://api.mercadopago.com/";

    private readonly HttpClient _httpClient;
    private readonly MercadoPagoOptions _options;

    public MercadoPagoGateway(HttpClient httpClient, IOptions<MercadoPagoOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<PaymentResult> CreatePaymentAsync(PaymentRequest request)
    {
        var amount = request.Amount.ToString("F2", CultureInfo.InvariantCulture);

        var payer = new Dictionary<string, object?>
        {
            ["email"] = request.Payer.Email,
            ["first_name"] = request.Payer.FirstName,
            ["last_name"] = request.Payer.LastName
        };

        if (!string.IsNullOrWhiteSpace(request.Payer.IdentificationType)
            && !string.IsNullOrWhiteSpace(request.Payer.IdentificationNumber))
        {
            payer["identification"] = new
            {
                type = request.Payer.IdentificationType,
                number = request.Payer.IdentificationNumber
            };
        }

        var body = new Dictionary<string, object?>
        {
            ["type"] = "online",
            ["processing_mode"] = "automatic",
            ["total_amount"] = amount,
            ["external_reference"] = request.OrderId.ToString(),
            ["payer"] = payer,
            ["transactions"] = new
            {
                payments = new[]
                {
                    new { amount, payment_method = BuildPaymentMethod(request) }
                }
            }
        };

        // Las fichas de OXXO y SPEI vencen; la tarjeta se cobra al momento.
        if (request.PaymentOption != "card")
            body["expiration_time"] = "P3D";

        using var message = new HttpRequestMessage(HttpMethod.Post, BaseUrl + "v1/orders")
        {
            Content = JsonContent.Create(body)
        };
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);
        message.Headers.Add("X-Idempotency-Key", Guid.NewGuid().ToString());

        return await SendAsync(message);
    }

    public async Task<PaymentResult> GetPaymentAsync(string gatewayOrderId)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, BaseUrl + $"v1/orders/{gatewayOrderId}");
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);

        return await SendAsync(message);
    }

    private async Task<PaymentResult> SendAsync(HttpRequestMessage message)
    {
        var response = await _httpClient.SendAsync(message);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                $"Mercado Pago rechazó la solicitud ({(int)response.StatusCode}): {json}");

        return ParseResult(json);
    }

    private static Dictionary<string, object?> BuildPaymentMethod(PaymentRequest request)
    {
        switch (request.PaymentOption)
        {
            case "card":
                if (request.Card is null)
                    throw new ArgumentException("Card data is required for card payments.");

                return new Dictionary<string, object?>
                {
                    ["id"] = request.Card.PaymentMethodId,
                    ["type"] = request.Card.PaymentMethodType,
                    ["token"] = request.Card.Token,
                    ["installments"] = request.Card.Installments
                };

            case "spei":
                return new Dictionary<string, object?> { ["id"] = "clabe", ["type"] = "bank_transfer" };

            case "oxxo":
                return new Dictionary<string, object?> { ["id"] = "oxxo", ["type"] = "ticket" };

            default:
                throw new ArgumentException($"Unknown payment option '{request.PaymentOption}'.");
        }
    }

    private static PaymentResult ParseResult(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        string? ticketUrl = null;
        string? reference = null;

        if (root.TryGetProperty("transactions", out var transactions)
            && transactions.TryGetProperty("payments", out var payments)
            && payments.GetArrayLength() > 0
            && payments[0].TryGetProperty("payment_method", out var paymentMethod))
        {
            ticketUrl = GetString(paymentMethod, "ticket_url");
            reference = GetString(paymentMethod, "reference");
        }

        Guid? orderId = Guid.TryParse(GetString(root, "external_reference"), out var parsed)
            ? parsed
            : null;

        return new PaymentResult(
            GetString(root, "id") ?? string.Empty,
            orderId,
            GetString(root, "status") ?? string.Empty,
            GetString(root, "status_detail"),
            ticketUrl,
            reference);
    }

    private static string? GetString(JsonElement element, string name)
    {
        return element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }
}