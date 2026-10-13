using ECommerce.Domain.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Storage;

public class LocalImageStorage(
    IWebHostEnvironment environment,
    IConfiguration configuration) : IImageStorage
{
    private const string ImagesFolder = "images/products";

    public async Task<string> SaveAsync(
        Stream stream,
        string fileName,
        string contentType)
    {
        var uploadsPath = Path.Combine(
            environment.WebRootPath,
            ImagesFolder);

        Directory.CreateDirectory(uploadsPath);

        var extension = Path.GetExtension(fileName);

        var uniqueFileName =
            $"{Guid.NewGuid()}{extension}";

        var filePath = Path.Combine(
            uploadsPath,
            uniqueFileName);

        await using var fileStream = new FileStream(
            filePath,
            FileMode.Create);

        await stream.CopyToAsync(fileStream);

        var baseUrl = configuration["ApiBaseUrl"]
            ?? throw new InvalidOperationException(
                "ApiBaseUrl is not configured.");

        return $"{baseUrl.TrimEnd('/')}/{ImagesFolder}/{uniqueFileName}";
    }

    public Task DeleteAsync(string fileName)
    {
        var uri = new Uri(fileName);
        var relativePath = uri.AbsolutePath.TrimStart('/');

        var filePath = Path.Combine(
            environment.WebRootPath,
            relativePath);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}