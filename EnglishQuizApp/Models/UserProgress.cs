public class UserProgress
{
    public int Id { get; set; }

    public required string UserId { get; set; }
    
    public int TotalXp { get; set; }

    public int Level { get; set; }
}