using EnglishQuizApp.Data;
using EnglishQuizApp.DTOs;
using EnglishQuizApp.Models;
using Microsoft.EntityFrameworkCore;

public class QuizService
{
    private readonly AppDbContext _context;
    private readonly Random _random = new();

    public QuizService(AppDbContext context)
    {
        _context = context;
    }

    public List<QuestionDto> GenerateQuestions(int count)
    {
        var allWrongAnswers = _context.Answers
            .AsNoTracking()
            .Where(a => !a.IsCorrect)
            .Include(a => a.Question)
            .ToList();

        var questions = _context.Questions
            .AsNoTracking()
            .Include(q => q.Answers)
            .OrderBy(x => Guid.NewGuid())
            .Take(count)
            .ToList();

        var used = new HashSet<string>();

        return questions.Select(q =>
            BuildQuestion(q, allWrongAnswers, used)
        ).ToList();
    }

    public int CalculateScore(SubmitQuizRequestDto request)
    {
        var questionIds = request.Answers.Select(a => a.QuestionId).ToList();

        var questions = _context.Questions
            .Include(q => q.Answers)
            .Where(q => questionIds.Contains(q.Id))
            .ToList();

        int correct = 0;

        foreach (var answer in request.Answers)
        {
            var question = questions.FirstOrDefault(q => q.Id == answer.QuestionId);

            var selected = question?.Answers
                .FirstOrDefault(a => a.Id == answer.AnswerId);

            if (selected?.IsCorrect == true)
                correct++;
        }

        return correct;
    }
    
    private QuestionDto BuildQuestion(
        Question q,
        List<Answer> wrongPool,
        HashSet<string> used)
    {
        var correct = q.Answers.First(a => a.IsCorrect);

        var wrong = wrongPool
            .Where(a =>
                a.QuestionId != q.Id &&
                !used.Contains(a.Text) &&
                a.Text != correct.Text)
            .OrderBy(_ => _random.Next())
            .Take(2)
            .ToList();

        foreach (var w in wrong)
            used.Add(w.Text);

        var answers = new List<AnswerDto>
        {
            new AnswerDto { Id = correct.Id, Text = correct.Text }
        };

        answers.AddRange(wrong.Select(a => new AnswerDto
        {
            Id = a.Id,
            Text = a.Text
        }));

        return new QuestionDto
        {
            Id = q.Id,
            Text = q.Text,
            Answers = answers.OrderBy(_ => _random.Next()).ToList()
        };
    }
}