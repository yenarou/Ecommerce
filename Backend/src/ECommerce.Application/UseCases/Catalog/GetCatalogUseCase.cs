using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;

namespace ECommerce.Application.UseCases.Catalog;

public class GetCatalogUseCase(
    IShopItemRepository shopItemRepository)
{
    public async Task<ICollection<ShopItem>> Execute(    int page,
        int size)
    {
        if (page < 1)
            throw new ArgumentOutOfRangeException(nameof(page), page, "Page must be greater than 0.");

        if (size < 1)
            throw new ArgumentOutOfRangeException(nameof(size), size, "Size must be greater than 0.");
        
        return await shopItemRepository.GetPage(page, size);
    }
}
