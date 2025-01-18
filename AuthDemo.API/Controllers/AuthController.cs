using AuthDemo.API.Models;
using AuthDemo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthDemo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IKeycloakService _keycloakService;
        private readonly IFileService _fileService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IKeycloakService keycloakService,
            IFileService fileService,
            ILogger<AuthController> logger)
        {
            _keycloakService = keycloakService;
            _fileService = fileService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserModel>> Register([FromForm] UserRegistrationModel model)
        {
            try
            {
                // Регистрируем пользователя в Keycloak
                var userId = await _keycloakService.RegisterUserAsync(model);

                // Если есть аватар, сохраняем его
                string? avatarUrl = null;
                if (model.Avatar != null)
                {
                    avatarUrl = await _fileService.SaveAvatarAsync(model.Avatar, userId);
                }

                // Обновляем пользователя с URL аватара
                var userModel = new UserModel
                {
                    Id = userId,
                    Username = model.Username,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    AvatarUrl = avatarUrl
                };

                await _keycloakService.UpdateUserAsync(userId, userModel);

                return CreatedAtAction(nameof(GetUser), new { userId }, userModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {Username}", model.Username);
                return StatusCode(500, new { message = "Error registering user" });
            }
        }

        [HttpGet("users/{userId}")]
        [Authorize]
        public async Task<ActionResult<UserModel>> GetUser(string userId)
        {
            try
            {
                var user = await _keycloakService.GetUserAsync(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId}", userId);
                return StatusCode(500, new { message = "Error getting user" });
            }
        }
    }
}
