using System.Text.Json;
using EnglishQuizApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EnglishQuizApp.Models;
using EnglishQuizApp.Helpers;

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
        if (!_configuration.GetValue<bool>("SeedDatabase"))
            return;

        var path = Path.Combine(AppContext.BaseDirectory, "SeedData/questions.json");
        if (!File.Exists(path))
            return;

        var json = File.ReadAllText(path);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var seedQuestions = JsonSerializer.Deserialize<List<QuestionSeed>>(json, options);

        if (seedQuestions == null || !seedQuestions.Any())
            return;

        var dbQuestions = _context.Questions
            .Include(q => q.Answers)
            .ToList();

        var dbMap = dbQuestions.ToDictionary(q => q.ContentHash);

        foreach (var q in seedQuestions)
        {
            var correctCount = q.Answers.Count(a => a.IsCorrect);
            if (correctCount != 1)
                continue;

            var hash = QuestionHashHelper.GenerateHash(q);

            // UPDATE
            if (dbMap.TryGetValue(hash, out var existing))
            {
                continue;
            }

            // INSERT
            _context.Questions.Add(new Question
            
            {
                ContentHash = hash,
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