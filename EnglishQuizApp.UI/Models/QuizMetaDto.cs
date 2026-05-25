namespace EnglishQuizApp.UI.Models;

public class QuizMetaDto
{
    public List<string> Categories { get; set; } = new();
    public List<int> Difficulties { get; set; } = new();
}