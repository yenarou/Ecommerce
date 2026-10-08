using ECommerce.Application.UseCases.Catalog;
using ECommerce.Domain.Models;
using ECommerce.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace ECommerce.Application.Tests.UseCases.Catalog;

public class GetProductDetailsUseCaseTests
{
    [Fact]
    public async Task Execute_ShouldReturnNull_WhenProductDoesNotExist()
    {
        var productRepository = new Mock<IProductRepository>();
        var productId = Guid.NewGuid();
        productRepository
            .Setup(repository => repository.GetById(productId))
            .ReturnsAsync((Product?)null);

        var useCase = new GetProductDetailsUseCase(productRepository.Object);

        var result = await useCase.Execute(productId);

        result.Should().BeNull();
    }
}
