using Microsoft.EntityFrameworkCore;
using EnglishQuizApp.Models;

namespace EnglishQuizApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Tables
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<QuizResult> QuizResults { get; set; }
    public DbSet<UserProgress> UserProgresses { get; set; }
    public DbSet<QuizSession> QuizSessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relation Question -> Answers (1 -> N)
        modelBuilder.Entity<Question>()
            .HasMany(q => q.Answers)
            .WithOne(a => a.Question)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // optimisation
        modelBuilder.Entity<Question>()
            .Property(q => q.Text)
            .IsRequired()
            .HasMaxLength(500);

        modelBuilder.Entity<Answer>()
            .Property(a => a.Text)
            .IsRequired()
            .HasMaxLength(300);
    }   
}