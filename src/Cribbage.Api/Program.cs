using Cribbage.Api.Data;
using Cribbage.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Cribbage")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? throw new InvalidOperationException("Set ConnectionStrings__Cribbage or DATABASE_URL.");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICribbageScorer, CribbageScorer>();
builder.Services.AddDbContext<CribbageDbContext>(options => options.UseNpgsql(NormalizeConnectionString(connectionString)));
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(_ => true)));
builder.Services.AddHealthChecks().AddDbContextCheck<CribbageDbContext>();

var app = builder.Build();
app.UseSwagger(); app.UseSwaggerUI();
app.UseCors();
app.MapControllers(); app.MapHealthChecks("/health");
app.MapGet("/api/version", () => Results.Ok(new { commit = Environment.GetEnvironmentVariable("RENDER_GIT_COMMIT") ?? "local" }));
await ApplyMigrations(app.Services);
app.Run();

static async Task ApplyMigrations(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CribbageDbContext>();
    await db.Database.MigrateAsync();
}

static string NormalizeConnectionString(string value)
{
    if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || uri.Scheme is not ("postgres" or "postgresql")) return value;
    var credentials = uri.UserInfo.Split(':', 2).Select(Uri.UnescapeDataString).ToArray();
    var port = uri.IsDefaultPort ? 5432 : uri.Port;
    return $"Host={uri.Host};Port={port};Database={uri.AbsolutePath.TrimStart('/')};Username={credentials[0]};Password={(credentials.Length > 1 ? credentials[1] : "")};SSL Mode=Require;Trust Server Certificate=true";
}

public partial class Program { }
