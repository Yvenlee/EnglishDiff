using Microsoft.EntityFrameworkCore;
using EnglishQuizApp.Data;

var builder = WebApplication.CreateBuilder(args);

// DB CONTEXT
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// CONTROLLERS
builder.Services.AddControllers();

// SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ADD SEEDER
builder.Services.AddScoped<QuizSeeder>();

var app = builder.Build();

// PIPELINE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


// EXECUTE SEED AT STARTUP
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<QuizSeeder>();
    seeder.SeedFromJson();
}

app.Run();