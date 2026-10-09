namespace ECommerce.Infrastructure.Payments;

public class MercadoPagoOptions
{
    public string AccessToken { get; set; } = string.Empty;
    public string NotificationUrl { get; set; } = string.Empty;
}