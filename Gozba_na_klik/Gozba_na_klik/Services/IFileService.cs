
namespace Gozba_na_klik.Services
{
    public interface IFileService
    {
        bool DeleteFile(string? relativePath);
        Task<string> SaveMealImageAsync(IFormFile file, string subFolder = "mealImg");
        Task<string> SaveUserImageAsync(IFormFile file, string subFolder = "profileImg");
    }
}