using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EnglishQuizApp.Models;

namespace EnglishQuizApp.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Tables métier
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<QuizResult> QuizResults { get; set; }
    public DbSet<UserProgress> UserProgresses { get; set; }
    public DbSet<QuizSession> QuizSessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ⚠️ OBLIGATOIRE pour Identity
        base.OnModelCreating(modelBuilder);

        // Question -> Answers
        modelBuilder.Entity<Question>()
            .HasMany(q => q.Answers)
            .WithOne(a => a.Question)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // constraints
        modelBuilder.Entity<Question>()
            .Property(q => q.Text)
            .IsRequired()
            .HasMaxLength(500);

        modelBuilder.Entity<Answer>()
            .Property(a => a.Text)
            .IsRequired()
            .HasMaxLength(300);
    }

    internal void SaveChanges()
    {
        throw new NotImplementedException();
    }
}