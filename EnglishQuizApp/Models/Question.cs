namespace EnglishQuizApp.Models;

public class Question
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    // Navigation vers les réponses
    public List<Answer> Answers { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}