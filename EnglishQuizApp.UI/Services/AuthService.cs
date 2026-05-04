using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using EnglishQuizApp.UI.Models;
using EnglishQuizApp.UI.Services.Auth;

namespace EnglishQuizApp.UI.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly CustomAuthStateProvider _authStateProvider;

    public AuthService(IHttpClientFactory factory, AuthenticationStateProvider provider)
    {
        _http = factory.CreateClient("Auth");
        _authStateProvider = (CustomAuthStateProvider)provider;
    }

    public async Task<bool> Login(string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", new
        {
            email,
            password
        });

        if (!response.IsSuccessStatusCode)
            return false;

        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

        if (result?.Token == null)
            return false;

        await _authStateProvider.MarkUserAsAuthenticated(result.Token);

        return true;
    }

    public async Task Logout()
    {
        await _authStateProvider.Logout();
    }
}