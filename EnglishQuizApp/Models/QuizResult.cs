public class QuizResult
{
    public int Id { get; set; }

    public int ScorePercent { get; set; }

    public int CorrectAnswers { get; set; }

    public int TotalQuestions { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}