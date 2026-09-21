using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeJournal.Api.Data;
using TradeJournal.Api.Models;

namespace TradeJournal.Api.Controllers;

[ApiController]
[Route("api/statistics")]
public sealed class StatisticsController(TradeJournalDbContext db) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> Summary([FromQuery] Guid accountId, CancellationToken cancellationToken)
    {
        var closed = db.Trades.AsNoTracking()
            .Where(x => x.AccountId == accountId && x.Status == TradeStatus.Closed);
        var result = await closed.GroupBy(_ => 1).Select(group => new
        {
            Trades = group.Count(),
            WinningTrades = group.Count(x => x.RealizedPnl > 0),
            LosingTrades = group.Count(x => x.RealizedPnl < 0),
            TotalPnl = group.Sum(x => x.RealizedPnl) ?? 0,
            AveragePnl = group.Average(x => x.RealizedPnl) ?? 0,
            TotalFees = group.Sum(x => x.Fees)
        }).SingleOrDefaultAsync(cancellationToken);
        return Ok(result ?? new { Trades = 0, WinningTrades = 0, LosingTrades = 0, TotalPnl = 0m, AveragePnl = 0m, TotalFees = 0m });
    }
}
