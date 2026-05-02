using EnglishQuizApp.Models;

public class QuestionSeed
{
    public string Text { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public List<AnswerSeed> Answers { get; set; } = new();
}