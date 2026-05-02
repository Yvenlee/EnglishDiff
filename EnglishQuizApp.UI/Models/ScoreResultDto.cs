namespace EnglishQuizApp.UI.Models;
public class ScoreResultDto
{
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public int ScorePercent { get; set; }
    public int EarnedXp { get; set; }
    public int TotalXp { get; set; }
    public int Level { get; set; }
}