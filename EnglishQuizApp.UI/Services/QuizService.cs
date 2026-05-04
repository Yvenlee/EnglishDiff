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

    public async Task<QuizResponseDto> GetQuiz()
    {
        return await _http.GetFromJsonAsync<QuizResponseDto>(
            "api/quiz/random?count=5"
        ) ?? new QuizResponseDto();
    }

    public async Task<ScoreResultDto> Submit(string sessionId, List<SubmitAnswerDto> answers)
    {
        var request = new SubmitQuizRequestDto
        {
            SessionId = sessionId,
            Answers = answers
        };

        var response = await _http.PostAsJsonAsync("api/quiz/submit", request);

        return await response.Content.ReadFromJsonAsync<ScoreResultDto>()
            ?? new ScoreResultDto();
    }

    public async Task<ProgressDto> GetProgress()
    {
        return await _http.GetFromJsonAsync<ProgressDto>("api/quiz/progress")
            ?? new ProgressDto();
    }
}