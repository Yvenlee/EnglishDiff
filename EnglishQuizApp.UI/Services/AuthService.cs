using System.Net.Http.Json;
using EnglishQuizApp.UI.Models;

namespace EnglishQuizApp.UI.Services.Auth;

public class AuthService
{
    private readonly IHttpClientFactory _factory;
    private readonly TokenStore _tokenStore;
    private readonly CustomAuthStateProvider _authProvider;

    public AuthService(
        IHttpClientFactory factory,
        TokenStore tokenStore,
        CustomAuthStateProvider authProvider)
    {
        _factory = factory;
        _tokenStore = tokenStore;
        _authProvider = authProvider;
    }

    public async Task<bool> Login(string email, string password)
    {
        var client = _factory.CreateClient("Auth");

        var response = await client.PostAsJsonAsync("api/auth/login", new
        {
            email,
            password
        });

        if (!response.IsSuccessStatusCode)
            return false;

        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

        if (result?.Token is null)
            return false;

        // 🔥 IMPORTANT : synchro complète
        _tokenStore.Set(result.Token);
        _authProvider.MarkUserAsAuthenticated(result.Token);

        Console.WriteLine("🟢 TOKEN STORED SUCCESSFULLY");

        return true;
    }
}