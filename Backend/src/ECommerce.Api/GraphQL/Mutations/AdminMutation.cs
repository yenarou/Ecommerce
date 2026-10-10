using ECommerce.Domain.Services;
using HotChocolate;

namespace ECommerce.Api.GraphQL.Mutations;

public class AdminMutation
{
    public async Task<string> UploadProductImage(
        IFile file,
        IImageStorage imageStorage)
    {
        await using var stream = file.OpenReadStream();

        var imageUrl = await imageStorage.SaveAsync(
            stream,
            file.Name,
            file.ContentType);

        return imageUrl;
    }
}