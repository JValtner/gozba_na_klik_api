using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;


namespace Gozba_na_klik.Services
{
    public class FileService : IFileService
    {
        private readonly string _basePath = Path.Combine("assets");
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
        private static readonly string[] AllowedMimeTypes = ["image/jpeg", "image/png", "image/webp"];

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
                throw new IOException("Došlo je do greške prilikom brisanja fajla.");
            }

            return false;
        }

        // -----------------------------
        // Core Save Logic
        // -----------------------------
        private async Task<string> SaveFileAsync(IFormFile file, string subFolder)
        {
            ValidateFile(file);

            var folderPath = Path.Combine(_basePath, subFolder);
            Directory.CreateDirectory(folderPath);

            var fileName = GenerateSafeFileName(file.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using var image = await Image.LoadAsync(file.OpenReadStream());
            await image.SaveAsync(filePath); // Re-encodes and strips metadata

            return "/" + filePath.Replace("\\", "/");
        }


        // -----------------------------
        // Modular Safety Checks
        // -----------------------------
        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Fajl je prazan.");

            if (file.Length > MaxFileSize)
                throw new ArgumentException("Fajl je prevelik. Maksimalna veličina je 5MB.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new ArgumentException("Nepodržan format fajla.");

            if (!AllowedMimeTypes.Contains(file.ContentType))
                throw new ArgumentException("Nepodržan MIME tip fajla.");

            if (HasDoubleExtension(file.FileName))
                throw new ArgumentException("Fajl ima sumnjivu ekstenziju.");
        }

        private string GenerateSafeFileName(string originalName)
        {
            var extension = Path.GetExtension(originalName);
            var baseName = Path.GetFileNameWithoutExtension(originalName);

            // Remove unsafe characters
            baseName = Regex.Replace(baseName, @"[^a-zA-Z0-9_-]", "");

            return $"{Guid.NewGuid()}_{baseName}{extension}";
        }
        private bool HasDoubleExtension(string fileName)
        {
            var parts = fileName.Split('.');
            return parts.Length > 2 && !AllowedExtensions.Contains("." + parts.Last().ToLowerInvariant());
        }
    }
}