using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Services;

namespace ECommerce.Application.UseCases.Admin.Products;

public class UpdateProductImageUseCase(
    IProductRepository productRepository,
    IImageStorage imageStorage)
{
    public async Task<ProductResponse> Execute(Guid productId, string imageUrl)
    {
        var product = await productRepository.GetById(productId)
            ?? throw new ArgumentException("Product not found.");

        var oldImages = product.Images.ToList();
        foreach (var image in oldImages)
            product.RemoveImage(image);

        product.AddImage(imageUrl, product.Name);
        await productRepository.Save(product);

        // borramos hasta despues de modificar bien
        foreach (var image in oldImages)
        {
            try { await imageStorage.DeleteAsync(image.Url); }
            catch { /* ignoramos */ }
        }

        return ProductMapper.ToResponse(product);
    }
}