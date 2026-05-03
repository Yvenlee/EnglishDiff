using EnglishQuizApp.Data;

public class SessionService
{
    private readonly AppDbContext _context;

    public SessionService(AppDbContext context)
    {
        _context = context;
    }

    public QuizSession CreateSession(string userId)
    {
        var session = new QuizSession
        {
            SessionId = Guid.NewGuid().ToString(),
            UserId = userId,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.QuizSessions.Add(session);
        _context.SaveChanges();

        return session;
    }

    public QuizSession? GetSession(string sessionId)
    {
        return _context.QuizSessions
            .FirstOrDefault(s => s.SessionId == sessionId);
    }

    public bool IsCompleted(QuizSession session)
    {
        return session.IsCompleted;
    }

    public void CompleteSession(QuizSession session)
    {
        session.IsCompleted = true;
        _context.QuizSessions.Update(session);
        _context.SaveChanges();
    }
}