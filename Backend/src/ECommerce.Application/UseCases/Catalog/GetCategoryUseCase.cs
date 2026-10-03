using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.UseCases.Catalog;

public class GetCategoryUseCase(ICategoryRepository categoryRepository)
{
    public async Task<CategoryResponse?> Execute(Guid categoryId)
    {
        var product = await categoryRepository.GetById(categoryId);

        return product is not null
            ? CategoryMapper.ToResponse(product)
            #warning implement domain exception
            : throw new ProductNotFoundException(categoryId.ToString());
    }
}