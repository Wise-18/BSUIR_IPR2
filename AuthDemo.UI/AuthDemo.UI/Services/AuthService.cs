using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AuthDemo.UI.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Text;

namespace AuthDemo.UI.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(LoginModel model);
    Task LogoutAsync();
    Task<bool> RegisterAsync(RegisterModel model);
    Task<UserProfile?> GetUserProfileAsync();
    bool IsAuthenticated { get; }
    string? Token { get; } // Добавляем свойство Token в интерфейс
    event Action? OnAuthenticationStateChanged; // Добавляем событие
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private string? _token;

    public bool IsAuthenticated => !string.IsNullOrEmpty(_token);
    public string? Token => _token; // Реализуем свойство
    public event Action? OnAuthenticationStateChanged; // Реализуем событие

    public AuthService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> LoginAsync(LoginModel model)
    {
        try
        {
            var parameters = new Dictionary<string, string>
            {
                {"client_id", _configuration["Keycloak:ClientId"]!},
                {"grant_type", "password"},
                {"username", model.Username},
                {"password", model.Password}
            };

            var response = await _httpClient.PostAsync(
                $"{_configuration["Keycloak:Authority"]}/protocol/openid-connect/token",
                new FormUrlEncodedContent(parameters));

            if (!response.IsSuccessStatusCode) return false;

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if (tokenResponse?.AccessToken == null) return false;

            _token = tokenResponse.AccessToken;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            OnAuthenticationStateChanged?.Invoke(); // Вызываем событие
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed");
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        _token = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
        OnAuthenticationStateChanged?.Invoke(); // Вызываем событие
    }

    public async Task<bool> RegisterAsync(RegisterModel model)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.Username), "username");
            content.Add(new StringContent(model.Password), "password");

            if (!string.IsNullOrEmpty(model.Email))
                content.Add(new StringContent(model.Email), "email");

            if (!string.IsNullOrEmpty(model.FirstName))
                content.Add(new StringContent(model.FirstName), "firstName");

            if (!string.IsNullOrEmpty(model.LastName))
                content.Add(new StringContent(model.LastName), "lastName");

            if (model.Avatar != null)
            {
                var stream = model.Avatar.OpenReadStream(maxAllowedSize: 1024 * 1024 * 5);
                var bytes = new byte[stream.Length];
                await stream.ReadAsync(bytes);
                var avatarContent = new ByteArrayContent(bytes);
                content.Add(avatarContent, "avatar", model.Avatar.Name);
            }

            var response = await _httpClient.PostAsync("api/auth/register", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed");
            return false;
        }
    }

    public async Task<UserProfile?> GetUserProfileAsync()
    {
        try
        {
            if (!IsAuthenticated) return null;

            var response = await _httpClient.GetAsync("api/auth/profile");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserProfile>();
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile");
            return null;
        }
    }
}