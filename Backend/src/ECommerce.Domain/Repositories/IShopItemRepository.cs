using ECommerce.Domain.Models;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Repositories;

public interface IShopItemRepository
{
    Task<ShopItem?> GetById(Guid shopItemId);
    Task<ICollection<ShopItem>> GetAll();
    Task<ICollection<ShopItem>> GetPage(int page, int size);
    Task<ICollection<ShopItem>> Filter(
        CatalogFilter filter,
        int page,
        int size);
    Task Save(ShopItem product);
    Task Delete(ShopItem productId);
}