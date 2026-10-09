using ECommerce.Application.Interfaces;
using ECommerce.Application.UseCases.Checkout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ECommerce.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/v1/payments")]
public class PaymentWebhookController(
    IPaymentGateway paymentGateway,
    ConfirmOrderPaymentUseCase confirmOrderPayment,
    ILogger<PaymentWebhookController> logger) : ControllerBase
{
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] JsonElement body)
    {
        try
        {
            if (!body.TryGetProperty("data", out var data) ||
                !data.TryGetProperty("id", out var idProp))
                return Ok();

            var gatewayOrderId = idProp.GetString();
            if (string.IsNullOrEmpty(gatewayOrderId)) return Ok();

            // No confiamos en lo que dice el body
            var result = await paymentGateway.GetPaymentAsync(gatewayOrderId);

            if (result.Status == "processed" && result.OrderId.HasValue)
                await confirmOrderPayment.Execute(result.OrderId.Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error procesando webhook de pago");
        }

        return Ok();
    }
}