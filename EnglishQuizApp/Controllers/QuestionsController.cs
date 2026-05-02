using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnglishQuizApp.Data;
using EnglishQuizApp.DTOs;
using EnglishQuizApp.Models;

namespace EnglishQuizApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public QuestionsController(AppDbContext context)
    {
        _context = context;
    }

    // 🔥 GET ALL QUESTIONS
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var questions = await _context.Questions
            .Include(q => q.Answers)
            .Select(q => new QuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                Category = q.Category,
                Difficulty = (int)q.Difficulty,
                DifficultyLabel = q.Difficulty.ToString(),
                Answers = q.Answers.Select(a => new AnswerDto
                {
                    Id = a.Id,
                    Text = a.Text
                }).ToList()
            })
            .ToListAsync();

        return Ok(questions);
    }

    // 🔥 CREATE SINGLE QUESTION
    [HttpPost]
    public async Task<IActionResult> Create(CreateQuestionDto dto)
    {
        if (dto == null || dto.Answers == null || !dto.Answers.Any())
            return BadRequest("Question invalide.");

        if (dto.Answers.Count(a => a.IsCorrect) != 1)
            return BadRequest("Il doit y avoir exactement une bonne réponse.");

        if (!Enum.IsDefined(typeof(DifficultyLevel), dto.Difficulty))
            return BadRequest("Difficulty invalide.");

        var question = new Question
        {
            Text = dto.Text,
            Category = dto.Category,
            Difficulty = (int)dto.Difficulty,
            Answers = dto.Answers.Select(a => new Answer
            {
                Text = a.Text,
                IsCorrect = a.IsCorrect
            }).ToList()
        };

        _context.Questions.Add(question);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            question.Id,
            question.Text
        });
    }
}