using System.Net.Http.Json;
using EnglishQuizApp.UI.Models;

namespace EnglishQuizApp.UI.Services;

public class QuizService
{
    private readonly HttpClient _http;

    public QuizService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("Api");
    }

    // 🔥 QUIZ AVEC SESSION
  public async Task<QuizResponseDto> GetQuiz(string userId)
    {
        return await _http.GetFromJsonAsync<QuizResponseDto>(
            $"api/quiz/random?count=5&userId={userId}"
        ) ?? new QuizResponseDto();
    }

    // SUBMIT AVEC SESSION
    public async Task<ScoreResultDto> Submit(string sessionId, List<SubmitAnswerDto> answers, string? userId = null)
    {
        var request = new SubmitQuizRequestDto
        {
            SessionId = sessionId,
            Answers = answers,
            UserId = userId
        };
        Console.WriteLine(_http.BaseAddress);
        Console.WriteLine($"Sending request for session: {request.SessionId}");
        var response = await _http.PostAsJsonAsync("api/quiz/submit", request);

        var result = await response.Content.ReadFromJsonAsync<ScoreResultDto>();

        return result ?? new ScoreResultDto();
        
    }

    // 🔥 PROGRESSION
    public async Task<ProgressDto> GetProgress()
    {
        try
        {
            return await _http.GetFromJsonAsync<ProgressDto>("api/quiz/progress")
                ?? new ProgressDto();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}