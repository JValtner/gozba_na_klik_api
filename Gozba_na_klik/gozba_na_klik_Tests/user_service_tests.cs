using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.Models;
using Gozba_na_klik.Services;
using Gozba_na_klik.Services.EmailServices;
using Gozba_na_klik.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace gozba_na_klik_Tests
{
    public class user_service_tests
    {
        private readonly Mock<IUsersRepository> _user_repo_mock = new();
        private readonly Mock<IFileService> _file_service_mock = new();
        private readonly Mock<IMapper> _mapper_mock = new();
        private readonly Mock<ILogger<UserService>> _logger_mock = new();
        private readonly Mock<UserManager<User>> _user_manager_mock;
        private readonly Mock<RoleManager<IdentityRole<int>>> _role_manager_mock;
        private readonly Mock<IEmailService> _email_service_mock = new();
        private readonly IConfiguration _config;

        private readonly UserService _sut;

        public user_service_tests()
        {
            var user_store = new Mock<IUserStore<User>>();
            _user_manager_mock = new Mock<UserManager<User>>(user_store.Object, null, null, null, null, null, null, null, null);

            var role_store = new Mock<IRoleStore<IdentityRole<int>>>();
            _role_manager_mock = new Mock<RoleManager<IdentityRole<int>>>(role_store.Object, null, null, null, null);

            _user_repo_mock.Setup(r => r.GetByIdAsync(1))
               .ReturnsAsync(new User { Id = 1, UserName = "test", Email = "test@example.com" });

            _user_repo_mock.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                           .ReturnsAsync((User u) => u);


            var in_memory_settings = new Dictionary<string, string>
            {
                {"ApiUrl", "http://localhost:5000"},
                {"FrontendUrl", "http://localhost:3000"},
                {"Jwt:Key", "0123456789abcdef0123456789abcdef"},
                {"Jwt:Issuer", "testIssuer"},
                {"Jwt:Audience", "testAudience"}
            };
            _config = new ConfigurationBuilder().AddInMemoryCollection(in_memory_settings).Build();

            _sut = new UserService(
                _user_repo_mock.Object,
                _file_service_mock.Object,
                _mapper_mock.Object,
                _logger_mock.Object,
                _user_manager_mock.Object,
                _role_manager_mock.Object,
                _config,
                _email_service_mock.Object
            );
        }

        [Fact]
        public async Task register_async_sends_confirmation_email()
        {
            var reg_dto = new RegistrationDto { Username = "testuser", Email = "test@example.com", Password = "Test@123" };
            var user = new User { Id = 1, UserName = "testuser", Email = "test@example.com" };

            _mapper_mock.Setup(m => m.Map<User>(reg_dto)).Returns(user);
            _user_manager_mock.Setup(m => m.CreateAsync(user, reg_dto.Password)).ReturnsAsync(IdentityResult.Success);
            _user_manager_mock.Setup(m => m.AddToRoleAsync(user, "User")).ReturnsAsync(IdentityResult.Success);
            _user_manager_mock.Setup(m => m.GenerateEmailConfirmationTokenAsync(user)).ReturnsAsync("rawToken");

            _email_service_mock.Setup(e => e.SendEmailAsync(user.Email, It.IsAny<string>(), It.IsAny<string>()))
                               .Returns(Task.CompletedTask);

            var result = await _sut.RegisterAsync(reg_dto);

            _email_service_mock.Verify(e => e.SendEmailAsync(user.Email, "Confirm your account", It.Is<string>(body => body.Contains("confirm-email"))), Times.Once);
        }

        [Fact]
        public async Task login_with_valid_credentials_returns_jwt()
        {
            var user = new User { Id = 1, UserName = "testuser", Email = "test@example.com" };

            _user_manager_mock.Setup(m => m.FindByNameAsync("testuser")).ReturnsAsync(user);
            _user_manager_mock.Setup(m => m.CheckPasswordAsync(user, "Test@123")).ReturnsAsync(true);
            _user_manager_mock.Setup(m => m.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
            _user_manager_mock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            var token = await _sut.Login(new LoginRequest { Username = "testuser", Password = "Test@123" });

            Assert.False(string.IsNullOrEmpty(token));
        }

        [Fact]
        public async Task confirm_email_async_valid_token_returns_true()
        {
            var user = new User { Id = 1, Email = "test@example.com" };

            _user_manager_mock.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(user);
            _user_manager_mock.Setup(m => m.ConfirmEmailAsync(user, It.IsAny<string>()))
                              .ReturnsAsync(IdentityResult.Success);

            var smtp_settings = Options.Create(new SmtpSettings());
            var service = new SmtpEmailService(smtp_settings, _user_manager_mock.Object);

            var result = await service.ConfirmEmailAsync(1, "validToken");

            Assert.True(result);
        }

        [Fact]
        public async Task reset_password_async_valid_token_resets_password()
        {
            var user = new User { Id = 1, UserName = "testuser" };

            _user_manager_mock.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(user);
            _user_manager_mock.Setup(m => m.ResetPasswordAsync(user, It.IsAny<string>(), "NewPass@123"))
                              .ReturnsAsync(IdentityResult.Success);

            var dto = new ResetPasswordDto { UserId = 1, Token = "encodedToken", NewPassword = "NewPass@123" };

            var result = await _sut.ResetPasswordAsync(dto);

            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task save_user_image_async_valid_file_saves_and_returns_path()
        {
            var file_mock = new Mock<IFormFile>();
            var content = new MemoryStream(new byte[100]);
            file_mock.Setup(f => f.OpenReadStream()).Returns(content);
            file_mock.Setup(f => f.FileName).Returns("test.png");
            file_mock.Setup(f => f.Length).Returns(100);
            file_mock.Setup(f => f.ContentType).Returns("image/png");

            _file_service_mock.Setup(f => f.SaveUserImageAsync(file_mock.Object, "profileImg"))
                              .ReturnsAsync("/assets/profileImg/test.png");

            var path = await _sut.UpdateUserAsync(1, new UpdateUserDto { Username = "test", Email = "test@example.com" }, file_mock.Object);

            Assert.NotNull(path.UserImage);
            Assert.Contains("profileImg", path.UserImage);
        }
    }
}