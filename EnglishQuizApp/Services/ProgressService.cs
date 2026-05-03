using EnglishQuizApp.Data;

public class ProgressService
{
    private readonly AppDbContext _context;

    public ProgressService(AppDbContext context)
    {
        _context = context;
    }

    public UserProgress GetOrCreate(string userId)
    {
        var progress = _context.UserProgresses
            .FirstOrDefault(p => p.UserId == userId);

        if (progress == null)
        {
            progress = new UserProgress
            {
                UserId = userId,
                TotalXp = 0,
                Level = 0
            };

            _context.UserProgresses.Add(progress);
        }

        return progress;
    }

    public void AddXp(UserProgress progress, int xp)
    {
        progress.TotalXp += xp;
        progress.Level = progress.TotalXp / 100;

        _context.SaveChanges();
    }

    public object GetProgress(string userId)
    {
        var progress = _context.UserProgresses
            .FirstOrDefault(p => p.UserId == userId);

        return new
        {
            totalXp = progress?.TotalXp ?? 0,
            level = progress?.Level ?? 0
        };
    }
}