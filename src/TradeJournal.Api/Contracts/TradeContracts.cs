using TradeJournal.Api.Models;

namespace TradeJournal.Api.Contracts;

public sealed record CreateTradeRequest(
    Guid AccountId,
    string Symbol,
    TradeSide Side,
    decimal Quantity,
    decimal EntryPrice,
    decimal OpeningFee,
    DateTimeOffset OpenedAt,
    string? Notes,
    IReadOnlyCollection<string>? Tags);

public sealed record CloseTradeRequest(decimal ExitPrice, decimal ClosingFee, DateTimeOffset ClosedAt);

public sealed record TradeResponse(
    Guid Id,
    Guid AccountId,
    string Symbol,
    TradeSide Side,
    decimal Quantity,
    decimal EntryPrice,
    decimal? ExitPrice,
    decimal Fees,
    decimal? RealizedPnl,
    TradeStatus Status,
    DateTimeOffset OpenedAt,
    DateTimeOffset? ClosedAt,
    string? Notes,
    IReadOnlyCollection<string> Tags);
