using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.UseCases.Admin.Products;

public class GetAdminProductsUseCase(
    IProductRepository productRepository)
{
    public async Task<ICollection<ProductResponse>> Execute()
    {
        var products = await productRepository.GetAll();

        return ProductMapper.ToResponse(products);
    }
}