using ECommerce.Domain.Repositories;
using ECommerce.Application.DTOs.Requests.Admin;
using ECommerce.Domain.Models;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.UseCases.Admin.Categories;

public class DeleteCategoryUseCase(
    ICategoryRepository categoryRepository)
{
    public async Task Execute(Guid categoryId)
    {
        var category = await categoryRepository.GetById(categoryId);

        if (category is null)
            throw new ArgumentException(
                $"Category with id '{categoryId}' was not found.");

        await categoryRepository.Delete(categoryId);
    }
}