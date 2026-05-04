using EnglishQuizApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class QuizController : ControllerBase
{
    private readonly QuizService _quizService;
    private readonly SessionService _sessionService;
    private readonly ProgressService _progressService;
    private readonly AppDbContext _context;

    public QuizController(
        QuizService quizService,
        SessionService sessionService,
        ProgressService progressService,
        AppDbContext context)
    {
        _quizService = quizService;
        _sessionService = sessionService;
        _progressService = progressService;
        _context = context;
    }

    private string GetUserId()
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException("UserId not found in token");

        return userId;
    }

    [HttpGet("random")]
    public IActionResult GetRandom(int count)
    {
        var userId = GetUserId();

        var session = _sessionService.CreateSession(userId);
        var questions = _quizService.GenerateQuestions(count);

        return Ok(new
        {
            sessionId = session.SessionId,
            questions
        });
    }

    [HttpPost("submit")]
    public IActionResult Submit(SubmitQuizRequestDto request)
    {
        var userId = GetUserId();

        var session = _sessionService.GetSession(request.SessionId, userId);

        if (session == null || session.IsCompleted)
            return BadRequest("Invalid session.");

        int correct = _quizService.CalculateScore(request);
        int total = request.Answers?.Count ?? 0;

        int xp = correct * 10;

        var progress = _progressService.GetOrCreate(userId);
        _progressService.AddXp(progress, xp);

        _sessionService.CompleteSession(session);

        var result = new QuizResult
        {
            ScorePercent = total == 0 ? 0 : (int)Math.Round((double)correct / total * 100),
            CorrectAnswers = correct,
            TotalQuestions = total
        };

        _context.QuizResults.Add(result);
        _context.SaveChanges();

        return Ok(new
        {
            result.ScorePercent,
            correctAnswers = correct,
            totalQuestions = total,
            earnedXp = xp,
            totalXp = progress.TotalXp,
            level = progress.Level
        });
    }

    [HttpGet("progress")]
    public IActionResult Progress()
    {
        var userId = GetUserId();
        return Ok(_progressService.GetProgress(userId));
    }
}