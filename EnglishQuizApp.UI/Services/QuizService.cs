using System.Net.Http.Headers;
using System.Net.Http.Json;
using EnglishQuizApp.UI.Models;
using EnglishQuizApp.UI.Services.Auth;

namespace EnglishQuizApp.UI.Services;

public class QuizService
{
    private readonly HttpClient _http;
    private readonly TokenStore _tokenStore;

    public QuizService(IHttpClientFactory factory, TokenStore tokenStore)
    {
        _http = factory.CreateClient("Api");
        _tokenStore = tokenStore;
    }

    // 🔥 IMPORTANT: assure toujours un header propre
    private void AttachToken()
    {
        var token = _tokenStore.Get();

        Console.WriteLine($"🎯 QuizService TOKEN: {token}");

        _http.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrWhiteSpace(token))
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<QuizResponseDto> GetQuiz()
    {
        Console.WriteLine("📌 QUIZ CALL");

        try
        {
            AttachToken();

            var result = await _http.GetFromJsonAsync<QuizResponseDto>(
                "api/quiz/random?count=5"
            );

            return result ?? new QuizResponseDto();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ GET QUIZ ERROR: {ex.Message}");
            return new QuizResponseDto();
        }
    }

    public async Task<ScoreResultDto> Submit(string sessionId, List<SubmitAnswerDto> answers)
    {
        try
        {
            AttachToken();

            var request = new SubmitQuizRequestDto
            {
                SessionId = sessionId,
                Answers = answers
            };

            var response = await _http.PostAsJsonAsync("api/quiz/submit", request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ SUBMIT HTTP ERROR: {response.StatusCode}");
                return new ScoreResultDto();
            }

            return await response.Content.ReadFromJsonAsync<ScoreResultDto>()
                   ?? new ScoreResultDto();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ SUBMIT ERROR: {ex.Message}");
            return new ScoreResultDto();
        }
    }

    public async Task<ProgressDto> GetProgress()
    {
        Console.WriteLine("📌 PROGRESS CALL");

        try
        {
            AttachToken();

            var result = await _http.GetFromJsonAsync<ProgressDto>(
                "api/quiz/progress"
            );

            return result ?? new ProgressDto();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ PROGRESS ERROR: {ex.Message}");
            return new ProgressDto();
        }
    }
}