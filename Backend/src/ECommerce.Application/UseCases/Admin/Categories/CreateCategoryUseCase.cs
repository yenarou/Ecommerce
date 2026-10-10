using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.UseCases.Admin.Categories;

public class CreateCategoryUseCase(
    ICategoryRepository categoryRepository)
{
    public async Task<Category> Execute(
        string slug,
        string name,
        string description)
    {
        var category = Category.Create(
            slug,
            name,
            description);

        await categoryRepository.Save(category);

        return category;
    }
}