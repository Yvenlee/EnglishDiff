public class SubmitQuizRequestDto
{
    public required string SessionId { get; set; }
    public required List<SubmitAnswerDto> Answers { get; set; }
}