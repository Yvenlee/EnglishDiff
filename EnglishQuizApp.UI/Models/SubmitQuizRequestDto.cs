namespace EnglishQuizApp.UI.Models;

public class SubmitQuizRequestDto
{
    public string SessionId { get; set; }
    public string? UserId { get; set; }
    public List<SubmitAnswerDto> Answers { get; set; }
}