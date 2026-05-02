namespace EnglishQuizApp.DTOs;

public class QuestionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public string DifficultyLabel { get; set; } = string.Empty;

    public List<AnswerDto> Answers { get; set; } = new();
}