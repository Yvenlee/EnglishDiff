namespace EnglishQuizApp.DTOs;

public class QuizQuestionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;

    public List<QuizAnswerDto> Answers { get; set; } = new();
}