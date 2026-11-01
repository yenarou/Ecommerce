using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ECommerce.Application.DTOs; 
using ECommerce.Application.Interfaces;

namespace ECommerce.Infrastructure.Payments;

public class PayPalGateway(HttpClient http, IOptions<PayPalOptions> options) : IPayPalGateway
{
    private readonly PayPalOptions _options = options.Value;

    private async Task<string> GetAccessToken()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/v1/oauth2/token");
        var basic = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);
        request.Content = new FormUrlEncodedContent(
            new Dictionary<string, string> { ["grant_type"] = "client_credentials" });

        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("access_token").GetString()!;
    }

    //crear orden con id
    public async Task<string> CreateOrder(Guid orderId, decimal amount, string currency)
    {
        var token = await GetAccessToken();

        var body = new
        {
            intent = "CAPTURE",
            purchase_units = new[]
            {
                new
                {
                    reference_id = orderId.ToString(),
                    amount = new
                    {
                        currency_code = currency,
                        value = amount.ToString("0.00", CultureInfo.InvariantCulture)
                    }
                }
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BaseUrl}/v2/checkout/orders");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("PayPal-Request-Id", $"create-{orderId}");
        request.Content = JsonContent.Create(body);

        using var response = await http.SendAsync(request);
        var text = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"PayPal create order failed: {text}");

        using var json = JsonDocument.Parse(text);
        return json.RootElement.GetProperty("id").GetString()!;
    }

    //pago y cliento aprovado
    public async Task<PayPalCaptureResult> CaptureOrder(string payPalOrderId)
    {
        var token = await GetAccessToken();

        using var request = new HttpRequestMessage(
            HttpMethod.Post, $"{_options.BaseUrl}/v2/checkout/orders/{payPalOrderId}/capture");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("PayPal-Request-Id", $"capture-{payPalOrderId}");
        request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

        using var response = await http.SendAsync(request);
        var text = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"PayPal capture failed: {text}");

        using var json = JsonDocument.Parse(text);
        var root = json.RootElement;
        var unit = root.GetProperty("purchase_units")[0];
        var capture = unit.GetProperty("payments").GetProperty("captures")[0];
        var amount = capture.GetProperty("amount");

        return new PayPalCaptureResult(
            root.GetProperty("id").GetString()!,
            capture.GetProperty("status").GetString()!,
            capture.GetProperty("id").GetString(),
            decimal.Parse(amount.GetProperty("value").GetString()!, CultureInfo.InvariantCulture),
            amount.GetProperty("currency_code").GetString()!,
            unit.TryGetProperty("reference_id", out var reference) ? reference.GetString() : null);
    }
}