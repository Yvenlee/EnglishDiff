public class CreateQuestionDto
{
    public string Text { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Difficulty { get; set; }

    public List<CreateAnswerDto> Answers { get; set; } = new();
}