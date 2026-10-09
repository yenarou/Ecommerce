namespace ECommerce.Domain.Entities;

public class Image
{
    private Image()
    {
    }

    private Image(string url, string alt)
    {
        Id = Guid.NewGuid();
        Url = url;
        Alt = alt;
    }

    public Guid Id { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public string Alt { get; private set; } = string.Empty;

    public static Image Create(string url, string alt)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Image URL cannot be null or empty.", nameof(url));

        if (url.Length > 2048)
            throw new ArgumentException("Image URL cannot exceed 2048 characters.", nameof(url));

        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            throw new ArgumentException("Image URL must be a valid absolute URL.", nameof(url));

        if (string.IsNullOrWhiteSpace(alt))
            throw new ArgumentException("Image alt text cannot be null or empty.", nameof(alt));

        if (alt.Length > 500)
            throw new ArgumentException("Image alt text cannot exceed 500 characters.", nameof(alt));

        return new Image(url, alt);
    }

    public void UpdateUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Image URL cannot be null or empty.", nameof(url));

        if (url.Length > 2048)
            throw new ArgumentException("Image URL cannot exceed 2048 characters.", nameof(url));

        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            throw new ArgumentException("Image URL must be a valid absolute URL.", nameof(url));

        Url = url;
    }

    public void UpdateAlt(string alt)
    {
        if (string.IsNullOrWhiteSpace(alt))
            throw new ArgumentException("Image alt text cannot be null or empty.", nameof(alt));

        if (alt.Length > 500)
            throw new ArgumentException("Image alt text cannot exceed 500 characters.", nameof(alt));

        Alt = alt;
    }
}