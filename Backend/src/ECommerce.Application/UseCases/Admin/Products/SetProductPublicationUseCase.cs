using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.UseCases.Admin.Products;

public class SetProductPublicationUseCase(
    IProductRepository productRepository)
{
    public async Task<ProductResponse> Execute(
        Guid productId,
        bool published)
    {
        //buscar el producto
        var product = await productRepository.GetById(productId);

        if (product is null)
            throw new ProductNotFoundException(productId.ToString());

        //publicar o despublicar
        if (published)
        {
            product.Publish();
        }
        else
        {
            product.Unpublish();
        }

        //guardar los cambios
        await productRepository.Save(product);

        //regresar el producto actualizado
        return ProductMapper.ToResponse(product);
    }
}