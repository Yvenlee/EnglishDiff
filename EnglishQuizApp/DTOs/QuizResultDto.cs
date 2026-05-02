public class QuizResultDto
{
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public List<QuestionResultDto> Details { get; set; } = new();
}