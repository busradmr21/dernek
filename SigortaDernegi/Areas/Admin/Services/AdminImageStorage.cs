namespace SigortaDernegi.Areas.Admin.Services;

public class AdminImageStorage
{
    private const long MaximumImageSize = 5 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private readonly IWebHostEnvironment _environment;

    public AdminImageStorage(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public string? Validate(IFormFile? imageFile, bool required)
    {
        if (imageFile is not { Length: > 0 })
        {
            return required ? "Bir görsel seçmelisiniz." : null;
        }

        if (imageFile.Length > MaximumImageSize)
        {
            return "Görsel dosyası en fazla 5 MB olabilir.";
        }

        var extension = Path.GetExtension(imageFile.FileName);
        return AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase)
            ? null
            : "Yalnızca JPG, JPEG, PNG veya WEBP görseller yüklenebilir.";
    }

    public async Task<string> SaveAsync(IFormFile imageFile, string moduleFolder)
    {
        var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", moduleFolder);
        Directory.CreateDirectory(uploadFolder);

        var filePath = Path.Combine(uploadFolder, fileName);
        await using var fileStream = new FileStream(
            filePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);

        await imageFile.CopyToAsync(fileStream);
        return $"/uploads/{moduleFolder}/{fileName}";
    }

    public void DeleteIfManaged(string? imageUrl, string moduleFolder)
    {
        var expectedPrefix = $"/uploads/{moduleFolder}/";
        if (string.IsNullOrWhiteSpace(imageUrl) ||
            !imageUrl.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var filePath = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            moduleFolder,
            Path.GetFileName(imageUrl));

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
