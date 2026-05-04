using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace EnglishQuizApp.UI.Services;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ProtectedLocalStorage _storage;

    public AuthHeaderHandler(ProtectedLocalStorage storage)
    {
        _storage = storage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var tokenResult = await _storage.GetAsync<string>("token");

        var token = tokenResult.Success ? tokenResult.Value : null;

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        Console.WriteLine($"=== AUTH HANDLER ===");
        Console.WriteLine($"TOKEN = {token}");

        return await base.SendAsync(request, cancellationToken);
    }
}