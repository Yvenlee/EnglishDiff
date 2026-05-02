public class QuizSession
{
    public int Id { get; set; }
    public string SessionId { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = default!;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}