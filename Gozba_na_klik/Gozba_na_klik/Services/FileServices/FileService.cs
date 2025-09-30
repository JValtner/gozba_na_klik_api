using Microsoft.AspNetCore.Http;

namespace Gozba_na_klik.Services.FileServices
{
    public class FileService : IFileService
    {
        private readonly string _basePath = Path.Combine("assets");

        private async Task<string> SaveFileAsync(IFormFile file, string subFolder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file upload");

            var folderPath = Path.Combine(_basePath, subFolder);
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/" + filePath.Replace("\\", "/");
        }

        public Task<string> SaveUserImageAsync(IFormFile file, string subFolder = "profileImg")
            => SaveFileAsync(file, subFolder);

        public Task<string> SaveMealImageAsync(IFormFile file, string subFolder = "mealImg")
            => SaveFileAsync(file, subFolder);

        public bool DeleteFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return false;

            try
            {
                var normalized = relativePath.TrimStart('/')
                                              .Replace("/", Path.DirectorySeparatorChar.ToString());
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), normalized);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
            }
            catch
            {
                // log error later
            }

            return false;
        }
    }
}
