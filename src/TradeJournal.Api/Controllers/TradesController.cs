using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeJournal.Api.Contracts;
using TradeJournal.Api.Data;
using TradeJournal.Api.Models;

namespace TradeJournal.Api.Controllers;

[ApiController]
[Route("api/trades")]
public sealed class TradesController(TradeJournalDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TradeResponse>>> GetAll(
        [FromQuery] Guid? accountId,
        [FromQuery] string? symbol,
        [FromQuery] TradeStatus? status,
        CancellationToken cancellationToken)
    {
        var query = db.Trades.AsNoTracking().Include(x => x.Tags).AsQueryable();
        if (accountId.HasValue) query = query.Where(x => x.AccountId == accountId);
        if (!string.IsNullOrWhiteSpace(symbol)) query = query.Where(x => x.Symbol == symbol.Trim().ToUpper());
        if (status.HasValue) query = query.Where(x => x.Status == status);

        var trades = await query.OrderByDescending(x => x.OpenedAt).Take(200).ToListAsync(cancellationToken);
        return Ok(trades.Select(ToResponse));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TradeResponse>> Get(Guid id, CancellationToken cancellationToken)
    {
        var trade = await db.Trades.AsNoTracking().Include(x => x.Tags)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return trade is null ? NotFound() : Ok(ToResponse(trade));
    }

    [HttpPost]
    public async Task<ActionResult<TradeResponse>> Create(CreateTradeRequest request, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0 || request.EntryPrice <= 0 || request.OpeningFee < 0)
            return ValidationProblem("Quantity and entry price must be positive; fee cannot be negative.");
        if (!await db.Accounts.AnyAsync(x => x.Id == request.AccountId, cancellationToken))
            return ValidationProblem("Account does not exist.");

        var tagNames = (request.Tags ?? [])
            .Select(x => x.Trim().ToLowerInvariant())
            .Where(x => x.Length > 0)
            .Distinct()
            .Take(10)
            .ToArray();
        if (tagNames.Any(x => x.Length > 50))
            return ValidationProblem("A tag cannot exceed 50 characters.");

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var existingTags = await db.Tags.Where(x => tagNames.Contains(x.Name)).ToListAsync(cancellationToken);
        var missingTags = tagNames.Except(existingTags.Select(x => x.Name))
            .Select(x => new Tag { Name = x })
            .ToList();

        var trade = new Trade
        {
            AccountId = request.AccountId,
            Symbol = request.Symbol.Trim().ToUpperInvariant(),
            Side = request.Side,
            Quantity = request.Quantity,
            EntryPrice = request.EntryPrice,
            OpenedAt = request.OpenedAt,
            Notes = request.Notes?.Trim(),
            Tags = [.. existingTags, .. missingTags]
        };
        trade.SetOpeningFee(request.OpeningFee);
        db.Trades.Add(trade);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Created($"/api/trades/{trade.Id}", ToResponse(trade));
    }

    [HttpPatch("{id:guid}/close")]
    public async Task<ActionResult<TradeResponse>> Close(Guid id, CloseTradeRequest request, CancellationToken cancellationToken)
    {
        var trade = await db.Trades.Include(x => x.Tags).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (trade is null) return NotFound();
        try
        {
            trade.Close(request.ExitPrice, request.ClosingFee, request.ClosedAt);
        }
        catch (ArgumentException exception)
        {
            return ValidationProblem(exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new ProblemDetails { Title = exception.Message });
        }
        await db.SaveChangesAsync(cancellationToken);
        return Ok(ToResponse(trade));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await db.Trades.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);
        return deleted == 0 ? NotFound() : NoContent();
    }

    private static TradeResponse ToResponse(Trade trade) => new(
        trade.Id, trade.AccountId, trade.Symbol, trade.Side, trade.Quantity, trade.EntryPrice,
        trade.ExitPrice, trade.Fees, trade.RealizedPnl, trade.Status, trade.OpenedAt, trade.ClosedAt,
        trade.Notes, trade.Tags.Select(x => x.Name).Order().ToArray());
}
