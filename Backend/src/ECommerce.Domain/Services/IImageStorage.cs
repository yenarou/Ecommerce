namespace ECommerce.Domain.Services;

public interface IImageStorage
{
    Task<string> SaveAsync(
        Stream stream,
        string fileName,
        string contentType);

    Task DeleteAsync(string fileName);
}