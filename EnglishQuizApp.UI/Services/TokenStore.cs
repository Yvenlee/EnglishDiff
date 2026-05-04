namespace EnglishQuizApp.UI.Services.Auth;

public class TokenStore
{
    private string? _token;

    public string? Get()
    {
        Console.WriteLine($"🔎 TokenStore GET: {_token}");
        return _token;
    }

    public void Set(string token)
    {
        _token = token;
        Console.WriteLine($"🟢 TokenStore SET: {token}");
    }

    public void Clear()
    {
        _token = null;
        Console.WriteLine("🧹 TokenStore CLEARED");
    }

    public bool HasToken => !string.IsNullOrWhiteSpace(_token);
}