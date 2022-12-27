using Microsoft.AspNetCore.Components.Authorization;
using Blazored.SessionStorage;
using System.Security.Claims;

namespace ALib.BlazorWasmSimpleAuth;

public class SimpleAuthenticationStateProvider : AuthenticationStateProvider
{
    public const string SessionKey = "login";

    private readonly ISessionStorageService _storage;
    private LoginUser? _user;
    private bool _initialized = false;
    private readonly AuthenticationState _unauth = new AuthenticationState(
        new ClaimsPrincipal(
            new ClaimsIdentity()));

    public SimpleAuthenticationStateProvider(ISessionStorageService storage)
    {
        _storage = storage;
    }

    public LoginUser? User => _user;

    public async Task LoginAsync(LoginUser user)
    {
        _user = user;
        _initialized = true;
        await _storage.SetItemAsync<LoginUser>(SessionKey, _user).ConfigureAwait(false);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task LogoutAsync()
    {
        _user = null;
        _initialized = true;
        await _storage.RemoveItemAsync(SessionKey);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async ValueTask LoadFromStorageAsync()
    {
        if (!_initialized)
        {
            _initialized = true;
            _user = await _storage.GetItemAsync<LoginUser>(SessionKey);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_user == null)
        {
            return Task.FromResult(_unauth);
        }
        var claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Name, _user.UserName));
        claims.AddRange(_user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return Task.FromResult(new AuthenticationState(
            new ClaimsPrincipal(
                new ClaimsIdentity(claims, SessionKey))));
    }
}
