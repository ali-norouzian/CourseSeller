using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;

namespace CourseSeller.Core.Convertors;

public interface IImageUtils
{
    Task ImageResize(string sourcePath, string destinationPath, int width, int height);
    Task<bool> ImageIsValid(IFormFile file);
}

public class ImageUtils : IImageUtils
{
    public async Task ImageResize(string sourcePath, string destinationPath, int width, int height)
    {
        using (var image = await Image.LoadAsync(sourcePath))
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(width, height),
                Mode = ResizeMode.Max // or other modes like Stretch, Pad, etc.
            }));

            await image.SaveAsync(destinationPath);
        }
    }

    public async Task<bool> ImageIsValid(IFormFile file)
    {
        try
        {
            // If he can't: its not image!
            var img = await Image.LoadAsync(file.OpenReadStream());

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return false;
        }
    }
}

