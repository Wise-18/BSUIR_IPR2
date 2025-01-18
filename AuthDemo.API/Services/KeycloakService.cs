using AuthDemo.API.Models;

namespace AuthDemo.API.Services
{
    public interface IKeycloakService
    {
        Task<string> RegisterUserAsync(UserRegistrationModel model);
        Task UpdateUserAsync(string userId, UserModel model);
        Task<UserModel> GetUserAsync(string userId);
    }

    public class KeycloakService : IKeycloakService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KeycloakService> _logger;
        private readonly string _adminToken;

        public KeycloakService(HttpClient httpClient, IConfiguration configuration, ILogger<KeycloakService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            // В реальном приложении нужно реализовать получение и обновление админского токена
            _adminToken = GetAdminToken().Result;
        }

        public async Task<string> RegisterUserAsync(UserRegistrationModel model)
        {
            try
            {
                var keycloakUser = new
                {
                    username = model.Username,
                    email = model.Email,
                    firstName = model.FirstName,
                    lastName = model.LastName,
                    enabled = true,
                    credentials = new[]
                    {
                    new
                    {
                        type = "password",
                        value = model.Password,
                        temporary = false
                    }
                }
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"{_configuration["Keycloak:auth-server-url"]}admin/realms/{_configuration["Keycloak:realm"]}/users",
                    keycloakUser);

                response.EnsureSuccessStatusCode();

                // Получаем ID созданного пользователя
                var locationHeader = response.Headers.Location;
                return Path.GetFileName(locationHeader?.ToString() ?? string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {Username}", model.Username);
                throw;
            }
        }

        public async Task UpdateUserAsync(string userId, UserModel model)
        {
            try
            {
                var keycloakUser = new
                {
                    firstName = model.FirstName,
                    lastName = model.LastName,
                    email = model.Email,
                    attributes = new Dictionary<string, string[]>
                {
                    { "avatarUrl", new[] { model.AvatarUrl ?? string.Empty } }
                }
                };

                var response = await _httpClient.PutAsJsonAsync(
                    $"{_configuration["Keycloak:auth-server-url"]}admin/realms/{_configuration["Keycloak:realm"]}/users/{userId}",
                    keycloakUser);

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", userId);
                throw;
            }
        }

        public async Task<UserModel> GetUserAsync(string userId)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_configuration["Keycloak:auth-server-url"]}admin/realms/{_configuration["Keycloak:realm"]}/users/{userId}");

                response.EnsureSuccessStatusCode();

                var user = await response.Content.ReadFromJsonAsync<KeycloakUser>();
                return MapToUserModel(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId}", userId);
                throw;
            }
        }

        private async Task<string> GetAdminToken()
        {
            // Реализация получения админского токена
            // В реальном приложении нужно добавить кэширование и обновление токена
            return "admin-token";
        }

        private static UserModel MapToUserModel(KeycloakUser user)
        {
            return new UserModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AvatarUrl = user.Attributes?.GetValueOrDefault("avatarUrl")?.FirstOrDefault()
            };
        }
    }
}
