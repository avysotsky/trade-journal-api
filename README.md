# Trade Journal API

A private-first pet project for practicing C#/.NET backend development and PostgreSQL on a real database. It is designed so it can later be published as a portfolio project without exposing credentials or production data.

## Stack

- .NET 8 / ASP.NET Core Web API
- PostgreSQL 17
- Entity Framework Core + Npgsql migrations
- Docker Compose
- xUnit
- GitHub Actions with a real PostgreSQL service

## Features

- trading accounts;
- open and closed trades;
- buy/sell PnL calculation with fees;
- tags with a many-to-many relationship;
- filtering and aggregate statistics;
- constraints, unique and composite indexes;
- transactional trade/tag creation;
- health endpoints and OpenAPI document.

## Run

```bash
docker compose up --build
```

- API: `http://localhost:8080`
- Swagger UI: `http://localhost:8080/swagger`
- OpenAPI: `http://localhost:8080/swagger/v1/swagger.json`
- PostgreSQL: `localhost:5432`

Development defaults in `docker-compose.yml` are intentionally non-secret. For your own values, copy `.env.example` to `.env`; `.env` is ignored by Git.

## First requests

```bash
curl -X POST http://localhost:8080/api/accounts -H 'Content-Type: application/json' -d '{"name":"Demo","baseCurrency":"USDT"}'
curl http://localhost:8080/api/accounts
```

Use the returned account ID to create a trade:

```bash
curl -X POST http://localhost:8080/api/trades -H 'Content-Type: application/json' -d '{"accountId":"ACCOUNT_ID","symbol":"BTCUSDT","side":1,"quantity":0.01,"entryPrice":65000,"openingFee":0.2,"openedAt":"2026-09-21T12:00:00Z","notes":"practice trade","tags":["btc","breakout"]}'
```

## EF Core migrations

```bash
dotnet tool restore
dotnet ef migrations add MigrationName --project src/TradeJournal.Api --startup-project src/TradeJournal.Api
dotnet ef database update --project src/TradeJournal.Api --startup-project src/TradeJournal.Api
```

## Tests

```bash
dotnet test
```

See [PostgreSQL practice plan](docs/training-plan.md) and [SQL exercises](sql/exercises.sql).
