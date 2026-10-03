namespace ECommerce.Domain.Exceptions;

public class ProductNotFoundException(string productId) : DomainException($"Couldn't find product with id { productId}")
{
    public string ProductId { get; } = productId;
}