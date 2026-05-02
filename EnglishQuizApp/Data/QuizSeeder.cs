using System.Text.Json;
using EnglishQuizApp.Data;
using Microsoft.EntityFrameworkCore;
using EnglishQuizApp.Models;

public class QuizSeeder
{
    private readonly AppDbContext _context;

    public QuizSeeder(AppDbContext context)
    {
        _context = context;
    }

    public void SeedFromJson()
    {
        if (_context.Questions.Any())
            return;

        var path = Path.Combine(AppContext.BaseDirectory, "SeedData/questions.json");
        var json = File.ReadAllText(path);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var questions = JsonSerializer.Deserialize<List<QuestionSeed>>(json, options);
        Console.WriteLine($"Loaded: {questions?.Count}");
        Console.WriteLine($"First text: {questions?.FirstOrDefault()?.Text}");

        if (questions == null || !questions.Any())
            return;

        foreach (var q in questions)
        {
            var correctCount = q.Answers.Count(a => a.IsCorrect);

            if (correctCount != 1)
            {
                Console.WriteLine($"❌ Skipped: {q.Text}");
                continue;
            }

            _context.Questions.Add(new Question
            {
                Text = q.Text,
                Category = q.Category,
                Difficulty = q.Difficulty,

                Answers = q.Answers.Select(a => new Answer
                {
                    Text = a.Text,
                    IsCorrect = a.IsCorrect
                }).ToList()
            });
        }

        _context.SaveChanges();
    }
}