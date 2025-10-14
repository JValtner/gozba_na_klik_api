namespace Gozba_na_klik.Services
{
    public interface IFileService
    {
        Task<string> SaveUserImageAsync(IFormFile file, string subFolder = "profileImg");
        Task<string> SaveMealImageAsync(IFormFile file, string subFolder = "mealImg");
        bool DeleteFile(string? relativePath);
    }
}
