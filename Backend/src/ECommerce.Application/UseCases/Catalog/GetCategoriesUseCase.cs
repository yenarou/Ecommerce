using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.UseCases.Catalog;

public class GetCategoriesUseCase(ICategoryRepository categoryRepository)
{
    public async Task<ICollection<CategoryResponse>> Execute()
    {
        var categories = await categoryRepository.GetAll();

        return CategoryMapper.ToResponse(categories);
    }
}