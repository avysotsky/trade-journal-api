using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeJournal.Api.Contracts;
using TradeJournal.Api.Data;
using TradeJournal.Api.Models;

namespace TradeJournal.Api.Controllers;

[ApiController]
[Route("api/accounts")]
public sealed class AccountsController(TradeJournalDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<AccountResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var accounts = await db.Accounts.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new AccountResponse(x.Id, x.Name, x.BaseCurrency, x.CreatedAt))
            .ToListAsync(cancellationToken);
        return Ok(accounts);
    }

    [HttpPost]
    public async Task<ActionResult<AccountResponse>> Create(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        var currency = request.BaseCurrency.Trim().ToUpperInvariant();
        if (name.Length is < 2 or > 100 || currency.Length is < 3 or > 10)
            return ValidationProblem("Name must be 2-100 characters and currency 3-10 characters.");
        if (await db.Accounts.AnyAsync(x => x.Name == name, cancellationToken))
            return Conflict(new ProblemDetails { Title = "Account name already exists." });

        var account = new Account { Name = name, BaseCurrency = currency };
        db.Accounts.Add(account);
        await db.SaveChangesAsync(cancellationToken);
        var response = new AccountResponse(account.Id, account.Name, account.BaseCurrency, account.CreatedAt);
        return Created($"/api/accounts/{account.Id}", response);
    }
}
