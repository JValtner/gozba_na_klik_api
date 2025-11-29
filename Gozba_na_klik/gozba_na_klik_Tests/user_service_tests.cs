using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Models;
using Gozba_na_klik.Services;
using Gozba_na_klik.Services.EmailServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace gozba_na_klik_Tests
{
    public class user_service_tests
    {
        private readonly Mock<IUsersRepository> _user_repo = new();
        private readonly Mock<IFileService> _file_service = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<ILogger<UserService>> _logger = new();
        private readonly Mock<UserManager<User>> _user_manager;
        private readonly Mock<RoleManager<IdentityRole<int>>> _role_manager;
        private readonly Mock<IEmailService> _email_service = new();

        private readonly IConfiguration _config;
        private readonly UserService _sut;

        public user_service_tests()
        {
            // --- UserManager Mock Setup ---
            var userStore = new Mock<IUserStore<User>>();
            _user_manager = new Mock<UserManager<User>>(
                userStore.Object, null, null, null, null, null, null, null, null
            );

            // --- RoleManager Mock Setup ---
            var roleStore = new Mock<IRoleStore<IdentityRole<int>>>();
            _role_manager = new Mock<RoleManager<IdentityRole<int>>>(
                roleStore.Object, null, null, null, null
            );

            // --- Repo default behavior ---
            _user_repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(
                new User { Id = 1, UserName = "test", Email = "test@example.com" }
            );

            _user_repo.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            // --- Config ---
            var cfgDict = new Dictionary<string, string>
            {
                {"ApiUrl", "http://localhost:5000"},
                {"FrontendUrl", "http://localhost:3000"},
                {"Jwt:Key", "0123456789abcdef0123456789abcdef"},
                {"Jwt:Issuer", "testIssuer"},
                {"Jwt:Audience", "testAudience"}
            };

            _config = new ConfigurationBuilder().AddInMemoryCollection(cfgDict).Build();

            // --- SUT ---
            _sut = new UserService(
                _user_repo.Object,
                _file_service.Object,
                _mapper.Object,
                _logger.Object,
                _user_manager.Object,
                _role_manager.Object,
                _config,
                _email_service.Object
            );
        }

        // ------------------------------------------------------------
        // REGISTER
        // ------------------------------------------------------------
        [Fact]
        public async Task register_async_sends_confirmation_email()
        {
            var dto = new RegistrationDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Test@123"
            };

            var user = new User { Id = 1, UserName = "testuser", Email = "test@example.com" };

            _mapper.Setup(m => m.Map<User>(dto)).Returns(user);
            _user_manager.Setup(m => m.CreateAsync(user, dto.Password)).ReturnsAsync(IdentityResult.Success);
            _user_manager.Setup(m => m.AddToRoleAsync(user, "User")).ReturnsAsync(IdentityResult.Success);
            _user_manager.Setup(m => m.GenerateEmailConfirmationTokenAsync(user)).ReturnsAsync("rawToken");

            _email_service.Setup(e =>
                e.SendEmailAsync(
                    user.Email,
                    It.IsAny<string>(),
                    It.IsAny<string>()
                )
            ).Returns(Task.CompletedTask);

            var result = await _sut.RegisterAsync(dto);

            _email_service.Verify(e =>
                e.SendEmailAsync(
                    user.Email,
                    "Confirm your account",
                    It.Is<string>(body =>
                        body.Contains("confirm-email") &&
                        body.Contains("userId=1") &&
                        body.Contains("rawToken")
                    )
                ),
                Times.Once
            );
        }

        // ------------------------------------------------------------
        // LOGIN
        // ------------------------------------------------------------
        [Fact]
        public async Task login_async_valid_credentials_returns_jwt()
        {
            var user = new User { Id = 1, UserName = "testuser", Email = "test@example.com" };

            _user_manager.Setup(m => m.FindByNameAsync("testuser")).ReturnsAsync(user);
            _user_manager.Setup(m => m.CheckPasswordAsync(user, "Test@123")).ReturnsAsync(true);
            _user_manager.Setup(m => m.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
            _user_manager.Setup(m => m.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "User" });

            var token = await _sut.Login(new LoginRequest
            {
                Username = "testuser",
                Password = "Test@123"
            });

            Assert.False(string.IsNullOrEmpty(token));
        }

        // ------------------------------------------------------------
        // CONFIRM EMAIL
        // ------------------------------------------------------------
        [Fact]
        public async Task confirm_email_async_valid_token_returns_true()
        {
            var user = new User { Id = 1, Email = "test@example.com" };

            _user_manager.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(user);
            _user_manager.Setup(m => m.ConfirmEmailAsync(user, "validToken"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _sut.ConfirmEmailAsync(1, "validToken");

            Assert.True(result);
        }

        // ------------------------------------------------------------
        // RESET PASSWORD
        // ------------------------------------------------------------
        [Fact]
        public async Task reset_password_async_valid_token_resets_password()
        {
            var user = new User { Id = 1, UserName = "testuser" };

            _user_manager.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(user);
            _user_manager.Setup(m => m.ResetPasswordAsync(user, "decoded", "NewPass@123"))
                .ReturnsAsync(IdentityResult.Success);

            var dto = new ResetPasswordDto
            {
                UserId = 1,
                Token = "decoded",
                NewPassword = "NewPass@123"
            };

            var result = await _sut.ResetPasswordAsync(dto);

            Assert.True(result.Succeeded);
        }

        // ------------------------------------------------------------
        // UPDATE USER + IMAGE UPLOAD
        // ------------------------------------------------------------
        [Fact]
        public async Task update_user_with_valid_file_saves_and_returns_path()
        {
            var formFile = new Mock<IFormFile>();
            var stream = new MemoryStream(new byte[100]);

            formFile.Setup(f => f.OpenReadStream()).Returns(stream);
            formFile.Setup(f => f.FileName).Returns("test.png");
            formFile.Setup(f => f.Length).Returns(100);
            formFile.Setup(f => f.ContentType).Returns("image/png");

            _file_service.Setup(f =>
                f.SaveUserImageAsync(formFile.Object, "profileImg")
            ).ReturnsAsync("/assets/profileImg/test.png");

            _mapper.Setup(m =>
                m.Map(It.IsAny<UpdateUserDto>(), It.IsAny<User>())
            ).Verifiable();

            var result = await _sut.UpdateUserAsync(
                1,
                new UpdateUserDto { Username = "test", Email = "test@example.com" },
                formFile.Object
            );

            Assert.NotNull(result.UserImage);
            Assert.Contains("profileImg", result.UserImage);
        }
    }
}
