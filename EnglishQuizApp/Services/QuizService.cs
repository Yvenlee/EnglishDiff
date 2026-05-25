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

    public List<QuestionDto> GenerateQuestions(
        int count,
        string? category,
        int? difficulty)
    {
        // 1. Récupérer les questions (filtrées par catégorie/difficulté si spécifié)
        var questionsQuery = _context.Questions
            .AsNoTracking()
            .Include(q => q.Answers)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            questionsQuery = questionsQuery.Where(q => q.Category == category);
        }

        if (difficulty.HasValue)
        {
            questionsQuery = questionsQuery.Where(q => q.Difficulty == difficulty.Value);
        }

        var questions = questionsQuery
            .OrderBy(x => Guid.NewGuid())
            .Take(count)
            .ToList();

        if (!questions.Any())
        {
            return new List<QuestionDto>();
        }

        // 2. Récupérer TOUTES les mauvaises réponses (pour toutes les catégories)
        // On filtrera plus tard dans BuildQuestion pour matcher la catégorie de chaque question.
        var wrongAnswers = _context.Answers
            .AsNoTracking()
            .Where(a => !a.IsCorrect)
            .Include(a => a.Question)
            .ToList();

        // 3. Construire les questions avec leurs réponses
        var usedWrongAnswers = new HashSet<string>();
        var result = new List<QuestionDto>();

        foreach (var question in questions)
        {
            var questionDto = BuildQuestion(question, wrongAnswers, usedWrongAnswers);
            result.Add(questionDto);
        }

        return result;
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
            var selected = question?.Answers.FirstOrDefault(a => a.Id == answer.AnswerId);
            if (selected?.IsCorrect == true)
            {
                correct++;
            }
        }
        return correct;
    }

    private QuestionDto BuildQuestion(
        Question question,
        List<Answer> wrongPool,
        HashSet<string> usedWrongAnswers)
    {
        var correctAnswer = question.Answers.First(a => a.IsCorrect);

        // 🔥 Filtrer les mauvaises réponses :
        // - Pas de la même question
        // - Pas déjà utilisées
        // - Pas le même texte que la bonne réponse
        // - DE LA MÊME CATÉGORIE QUE LA QUESTION (même en mode global)
        var candidateWrongAnswers = wrongPool
            .Where(a =>
                a.QuestionId != question.Id &&
                !usedWrongAnswers.Contains(a.Text) &&
                a.Text != correctAnswer.Text &&
                a.Question.Category == question.Category // 👈 Clé : toujours la même catégorie
            )
            .ToList();

        // Prendre 2 mauvaises réponses aléatoires (ou moins si pas assez de candidates)
        var wrongAnswers = candidateWrongAnswers
            .OrderBy(_ => _random.Next())
            .Take(2)
            .ToList();

        // Ajouter les réponses utilisées au set
        foreach (var wrong in wrongAnswers)
        {
            usedWrongAnswers.Add(wrong.Text);
        }

        var answers = new List<AnswerDto>
        {
            new AnswerDto { Id = correctAnswer.Id, Text = correctAnswer.Text }
        };
        answers.AddRange(wrongAnswers.Select(a => new AnswerDto { Id = a.Id, Text = a.Text }));

        // Mélanger les réponses
        return new QuestionDto
        {
            Id = question.Id,
            Text = question.Text,
            Category = question.Category,
            Difficulty = question.Difficulty,
            Answers = answers.OrderBy(_ => _random.Next()).ToList()
        };
    }
}