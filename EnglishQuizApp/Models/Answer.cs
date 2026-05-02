namespace EnglishQuizApp.Models;

public class Answer
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    // Indique si c’est la bonne réponse
    public bool IsCorrect { get; set; }

    // Clé étrangère
    public int QuestionId { get; set; }

    // Navigation vers la question
    public Question? Question { get; set; }
    public string Category { get; set; } = string.Empty;
}