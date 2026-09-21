using Microsoft.EntityFrameworkCore;
using Npgsql;
using TradeJournal.Api.Data;
using TradeJournal.Api.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("ConnectionStrings:Postgres is required.");

builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.MapEnum<TradeSide>("trade_side");
dataSourceBuilder.MapEnum<TradeStatus>("trade_status");
var dataSource = dataSourceBuilder.Build();
builder.Services.AddSingleton(dataSource);
builder.Services.AddDbContext<TradeJournalDbContext>(options => options.UseNpgsql(dataSource));

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapGet("/health/database", async (TradeJournalDbContext db, CancellationToken cancellationToken) =>
    await db.Database.CanConnectAsync(cancellationToken)
        ? Results.Ok(new { status = "ok", database = "postgresql" })
        : Results.Problem("PostgreSQL is unavailable", statusCode: StatusCodes.Status503ServiceUnavailable));

if (builder.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<TradeJournalDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();

public partial class Program;
