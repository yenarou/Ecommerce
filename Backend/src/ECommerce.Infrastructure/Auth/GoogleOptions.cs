namespace ECommerce.Infrastructure.Auth;

public class GoogleOptions
{
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public string ClassroomRedirectUri { get; init; } = string.Empty;
    public string WebRedirectUri { get; init; } = string.Empty;
    public string MobileRedirectUri { get; init; } = string.Empty;
}