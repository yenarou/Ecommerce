using ECommerce.Application.DTOs.Responses;
using ECommerce.Application.Mappers;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Application.UseCases.Admin.Products;

public class UpdateProductUseCase(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository)
{
    public async Task<ProductResponse> Execute(
        Guid productId,
        string name,
        string description,
        decimal price,
        string currency,
        int stock,
        Guid categoryId)
    {
        //buscar el producto
        var product = await productRepository.GetById(productId);

        if (product is null)
            throw new ProductNotFoundException(productId.ToString());

        //buscar la categoría
        var category = await categoryRepository.GetById(categoryId);

        if (category is null)
            throw new ArgumentException(
                "Category not found.",
                nameof(categoryId));

        //crear los Value Objects
        var money = Money.Create(price, currency);
        var quantity = Quantity.Create(stock);

        //actualizar usando los métodos del Domain
        product.UpdateName(name);
        product.UpdateDescription(description);
        product.UpdatePrice(money);
        product.UpdateStock(quantity);
        product.UpdateCategory(category);

        //guardar los cambios
        await productRepository.Save(product);

        //regresar el producto actualizado como DTO
        return ProductMapper.ToResponse(product);
    }
}