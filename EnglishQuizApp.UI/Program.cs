using EnglishQuizApp.UI.Services;
using EnglishQuizApp.UI.Services.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// BLAZOR
// --------------------
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// --------------------
// AUTH CORE
// --------------------
builder.Services.AddScoped<ProtectedLocalStorage>();

builder.Services.AddSingleton<TokenStore>();

builder.Services.AddScoped<CustomAuthStateProvider>();

builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>());

builder.Services.AddAuthorizationCore();

// --------------------
// HTTP CLIENTS
// --------------------
builder.Services.AddHttpClient("Auth", c =>
{
    c.BaseAddress = new Uri("http://localhost:5047/");
});

builder.Services.AddHttpClient("Api", c =>
{
    c.BaseAddress = new Uri("http://localhost:5047/");
})
.AddHttpMessageHandler<AuthHeaderHandler>();

// --------------------
// SERVICES
// --------------------
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<QuizService>();
builder.Services.AddTransient<AuthHeaderHandler>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();