using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Tests;

public class CustomizationTests
{
    [Fact]
    public void Create_PreservesGiftWrapSelection()
    {
        var customization = Customization.Create("Personalizar colores", wrap: true);

        Assert.True(customization.IsWrap);
    }

    [Fact]
    public void Create_AllowsMissingPersonalization()
    {
        var customization = Customization.Create(null);

        Assert.Null(customization.Description);
        Assert.False(customization.IsWrap);
    }

    [Fact]
    public void Create_AllowsGiftWrapWithoutPersonalization()
    {
        var customization = Customization.Create(null, wrap: true);

        Assert.Null(customization.Description);
        Assert.True(customization.IsWrap);
    }
}
