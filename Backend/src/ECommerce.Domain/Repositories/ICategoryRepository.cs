using ECommerce.Domain.Models;

namespace ECommerce.Domain.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetById(Guid categoryId);
    Task<List<Category>?> GetAll();
    Task Save(Category category);
    Task Delete(Guid categoryId);
}