using System.Text.Json;
using EnglishQuizApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EnglishQuizApp.Models;

public class QuizSeeder
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public QuizSeeder(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public void SeedFromJson()
    {
        // Condition via appsettings.json
        if (!_configuration.GetValue<bool>("SeedDatabase"))
        {
            return;
        }

        // Sécurité: éviter double seed
        if (_context.Questions.Any())
        {
            return;
        }

        var path = Path.Combine(AppContext.BaseDirectory, "SeedData/questions.json");

        if (!File.Exists(path))
        {
            return;
        }

        var json = File.ReadAllText(path);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var questions = JsonSerializer.Deserialize<List<QuestionSeed>>(json, options);

        Console.WriteLine($"Loaded: {questions?.Count}");

        if (questions == null || !questions.Any())
            return;

        foreach (var q in questions)
        {
            var correctCount = q.Answers.Count(a => a.IsCorrect);

            if (correctCount != 1)
            {
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

        Console.WriteLine("Seeding completed.");
    }
}