using ECommerce.Domain.Models;

namespace ECommerce.Domain.Tests;

public class CustomizationTests
{
    [Fact]
    public void Create_PreservesGiftWrapSelection()
    {
        var customization = Customization.Create("Personalizar colores", wrap: true);

        Assert.True(customization.IsWrap);
    }
}
