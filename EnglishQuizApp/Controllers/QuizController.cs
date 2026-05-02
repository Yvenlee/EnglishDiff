using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnglishQuizApp.Data;
using EnglishQuizApp.DTOs;

namespace EnglishQuizApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizController : ControllerBase
{
    private readonly AppDbContext _context;

    public QuizController(AppDbContext context)
    {
        _context = context;
    }


[HttpGet("random")]
public IActionResult GetRandom(int count, string userId)
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

    // 🔥 Pool des mauvaises réponses
    var allWrongAnswers = _context.Answers
        .AsNoTracking()
        .Where(a => !a.IsCorrect)
        .Include(a => a.Question)
        .ToList();

    // 🔥 Questions aléatoires
    var rawQuestions = _context.Questions
        .AsNoTracking()
        .Include(q => q.Answers)
        .OrderBy(x => Guid.NewGuid())
        .Take(count)
        .ToList();

    var random = new Random();
    var usedInQuiz = new HashSet<string>();

    var questions = rawQuestions
        .Select(q =>
        {
            var correctAnswer = q.Answers.First(a => a.IsCorrect);

            // 🎯 mauvaises réponses (catégorie + anti doublon)
            var wrongPool = allWrongAnswers
                .Where(a =>
                    a.Question != null &&
                    a.QuestionId != q.Id &&
                    a.Question.Category == q.Category &&
                    !usedInQuiz.Contains(a.Text))
                .OrderBy(_ => random.Next())
                .Take(2)
                .ToList();

            // fallback si pas assez
            if (wrongPool.Count < 2)
            {
                wrongPool = allWrongAnswers
                    .Where(a =>
                        a.QuestionId != q.Id &&
                        !usedInQuiz.Contains(a.Text))
                    .OrderBy(_ => random.Next())
                    .Take(2)
                    .ToList();
            }

            foreach (var a in wrongPool)
                usedInQuiz.Add(a.Text);

            var answers = new List<AnswerDto>(3)
            {
                new AnswerDto
                {
                    Id = correctAnswer.Id,
                    Text = correctAnswer.Text
                }
            };

            answers.AddRange(wrongPool.Select(a => new AnswerDto
            {
                Id = a.Id,
                Text = a.Text
            }));

            return new QuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                Answers = answers
                    .OrderBy(_ => random.Next())
                    .ToList()
            };
        })
        .ToList();

    return Ok(new
    {
        sessionId = session.SessionId,
        questions
    });
}

[HttpPost("submit")]
public async Task<IActionResult> SubmitQuizAsync(SubmitQuizRequestDto request)
{
    if (request == null || request.Answers == null || !request.Answers.Any())
    {
        Console.WriteLine("❌ Invalid request");
        return BadRequest("Invalid request.");
    }

    var session = await _context.QuizSessions
        .AsNoTracking()
        .SingleOrDefaultAsync(s => EF.Functions.Like(s.SessionId, request.SessionId));

    if (session == null)
    {
        Console.WriteLine("❌ Session NOT FOUND");
        return BadRequest("Invalid session.");
    }

    string userId = session.UserId;

    if (session.IsCompleted)
    {
        Console.WriteLine("❌ Quiz already submitted");
        return BadRequest("Quiz already submitted.");
    }

    int correct = 0;

    foreach (var answer in request.Answers)
    {
        var question = _context.Questions
            .Include(q => q.Answers)
            .FirstOrDefault(q => q.Id == answer.QuestionId);

        if (question == null)
            continue;

        var selectedAnswer = question.Answers
            .FirstOrDefault(a => a.Id == answer.AnswerId);

        if (selectedAnswer != null && selectedAnswer.IsCorrect)
            correct++;
    }

    int total = request.Answers.Count;
    int scorePercent = total == 0 ? 0 : (int)Math.Round((double)correct / total * 100);

    int earnedXp = correct * 10;

    var progress = _context.UserProgresses.FirstOrDefault(p => p.UserId == userId);

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

    progress.TotalXp += earnedXp;
    progress.Level = progress.TotalXp / 100;

    // 🔥 SESSION LOCK
    session.IsCompleted = true;
    _context.QuizSessions.Update(session);

    // 🔥 RESULT SAVE
    var result = new QuizResult
    {
        ScorePercent = scorePercent,
        CorrectAnswers = correct,
        TotalQuestions = total
    };

    _context.QuizResults.Add(result);

    _context.SaveChanges();

    return Ok(new
    {
        scorePercent,
        correctAnswers = correct,
        totalQuestions = total,
        earnedXp,
        totalXp = progress.TotalXp,
        level = progress.Level
    });
}

[HttpGet("progress")]
public IActionResult GetProgress()
{
    var progress = _context.UserProgresses.FirstOrDefault();

    if (progress == null)
        return Ok(new { totalXp = 0, level = 0 });

    return Ok(new
    {
        totalXp = progress?.TotalXp ?? 0,
        level = progress?.Level ?? 0
    });
}
}