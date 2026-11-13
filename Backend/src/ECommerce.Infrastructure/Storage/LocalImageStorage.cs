using ECommerce.Domain.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using ImageMagick;

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
    { //mejora de rendimiento
        var uploadsPath = Path.Combine(environment.WebRootPath, ImagesFolder);
        Directory.CreateDirectory(uploadsPath);

        //var extension = Path.GetExtension(fileName);

        var uniqueFileName = $"{Guid.NewGuid()}.webp";

        var filePath = Path.Combine(uploadsPath, uniqueFileName);

        using var image = new MagickImage(stream);
        image.AutoOrient();
        image.Resize(new MagickGeometry(1000, 1000) { Greater = true }); //solo reduce si se pasa de lanza
        image.Quality = 78;
        image.Format = MagickFormat.WebP;
        await image.WriteAsync(filePath);


        var baseUrl = configuration["ApiBaseUrl"]
            ?? throw new InvalidOperationException("ApiBaseUrl is not configured.");

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