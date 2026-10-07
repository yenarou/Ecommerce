using ECommerce.Application.DTOs.Responses;
using ECommerce.Domain.Models;

namespace ECommerce.Application.Mappers;

public static class CategoryMapper
{
    public static CategoryResponse ToResponse(Category category)
    {
        return new CategoryResponse(
            category.Id,
            category.Name,
            category.Slug,
            category.Description
        );
    }

    public static
        ICollection<CategoryResponse> ToResponse(
            ICollection<Category> categories)
    {
        return categories
            .Select(ToResponse)
            .ToList();
    }
}