using System.Text.Json;
using EnglishQuizApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EnglishQuizApp.Models;
using EnglishQuizApp.Helpers;
using EFCore.BulkExtensions;

public class QuizSeeder
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public QuizSeeder(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task SeedFromJsonAsync()
    {
        if (!_configuration.GetValue<bool>("SeedDatabase"))
            return;

        var path = Path.Combine(AppContext.BaseDirectory, "SeedData/questions.json");

        if (!File.Exists(path))
            return;

        var json = await File.ReadAllTextAsync(path);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var seedQuestions = JsonSerializer.Deserialize<List<QuestionSeed>>(json, options);

        if (seedQuestions == null || !seedQuestions.Any())
            return;

        // On récupère uniquement les hashes existants
        var existingHashes = (await _context.Questions
            .Select(q => q.ContentHash)
            .ToListAsync())
            .ToHashSet();

        var questionsToInsert = new List<Question>();

        foreach (var q in seedQuestions)
        {
            var correctCount = q.Answers.Count(a => a.IsCorrect);

            if (correctCount != 1)
                continue;

            var hash = QuestionHashHelper.GenerateHash(q);

            // Ignore si déjà présent
            if (existingHashes.Contains(hash))
                continue;

            questionsToInsert.Add(new Question
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

        if (!questionsToInsert.Any())
            return;

        // Bulk insert Questions
        await _context.BulkInsertAsync(questionsToInsert, new BulkConfig
        {
            IncludeGraph = true
        });
    }
}