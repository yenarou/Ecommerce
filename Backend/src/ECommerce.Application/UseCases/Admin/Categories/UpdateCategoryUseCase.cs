using ECommerce.Domain.Repositories;

namespace ECommerce.Application.UseCases.Admin.Categories;

public class UpdateCategoryUseCase(
    ICategoryRepository categoryRepository)
{
    public async Task<ECommerce.Domain.Models.Category> Execute(
        Guid categoryId,
        string name,
        string description)
    {
        var category = await categoryRepository.GetById(categoryId);

        if (category is null)
            throw new ArgumentException(
                $"Category with id '{categoryId}' was not found.");

        category.Update(
            name,
            description);

        await categoryRepository.Save(category);
        return category;
    }
}