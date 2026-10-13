using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetById(Guid categoryId);
    Task<Category?> GetBySlug(string slug);
    Task<ICollection<Category>> GetAll();
    Task Save(Category category);
    Task Delete(Guid categoryId);
}