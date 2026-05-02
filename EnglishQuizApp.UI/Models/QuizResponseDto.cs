namespace EnglishQuizApp.UI.Models;

public class QuizResponseDto
{
    public string SessionId { get; set; } = string.Empty;
    public List<QuestionDto> Questions { get; set; } = new();
}