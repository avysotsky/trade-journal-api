namespace TradeJournal.Api.Models;

public sealed class Trade
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;
    public required string Symbol { get; set; }
    public TradeSide Side { get; set; }
    public decimal Quantity { get; set; }
    public decimal EntryPrice { get; set; }
    public decimal? ExitPrice { get; private set; }
    public decimal Fees { get; private set; }
    public decimal? RealizedPnl { get; private set; }
    public TradeStatus Status { get; private set; } = TradeStatus.Open;
    public DateTimeOffset OpenedAt { get; set; }
    public DateTimeOffset? ClosedAt { get; private set; }
    public string? Notes { get; set; }
    public List<Tag> Tags { get; set; } = [];

    public void Close(decimal exitPrice, decimal closingFee, DateTimeOffset closedAt)
    {
        if (Status == TradeStatus.Closed)
            throw new InvalidOperationException("Trade is already closed.");
        if (exitPrice <= 0)
            throw new ArgumentOutOfRangeException(nameof(exitPrice), "Exit price must be positive.");
        if (closingFee < 0)
            throw new ArgumentOutOfRangeException(nameof(closingFee), "Fee cannot be negative.");
        if (closedAt < OpenedAt)
            throw new ArgumentOutOfRangeException(nameof(closedAt), "Close time cannot precede open time.");

        ExitPrice = exitPrice;
        Fees += closingFee;
        ClosedAt = closedAt;
        Status = TradeStatus.Closed;
        var grossPnl = Side == TradeSide.Buy
            ? (exitPrice - EntryPrice) * Quantity
            : (EntryPrice - exitPrice) * Quantity;
        RealizedPnl = grossPnl - Fees;
    }

    public void SetOpeningFee(decimal fee)
    {
        if (fee < 0)
            throw new ArgumentOutOfRangeException(nameof(fee), "Fee cannot be negative.");
        Fees = fee;
    }
}
