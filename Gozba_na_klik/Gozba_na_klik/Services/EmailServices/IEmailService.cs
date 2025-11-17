using Gozba_na_klik.Models;
using Microsoft.AspNetCore.Identity;

namespace Gozba_na_klik.Services.EmailServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string fileName);
        Task<bool> ConfirmEmailAsync(int userId, string token);
    }
}
