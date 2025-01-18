using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace AuthDemo.UI.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;

    public CustomAuthStateProvider(IAuthService authService)
    {
        _authService = authService;
        _authService.OnAuthenticationStateChanged += AuthenticationStateChanged;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!_authService.IsAuthenticated)
        {
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }

        var token = _authService.Token;
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
        var principal = new ClaimsPrincipal(identity);

        return Task.FromResult(new AuthenticationState(principal));
    }

    private void AuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}