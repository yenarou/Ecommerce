using ECommerce.Domain.Models;

namespace ECommerce.Domain.Repositories;

public interface IShopItemRepository
{
    Task<ShopItem?> GetById(Guid shopItemId);
    Task<List<ShopItem>?> GetAll();
    Task Save(ShopItem product);
    Task Delete(ShopItem productId);
}