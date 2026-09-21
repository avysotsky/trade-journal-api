# PostgreSQL practice plan

Use the API for application-level exercises and `psql` for direct SQL practice.

1. Create accounts and trades; inspect tables, primary keys, foreign keys and enum types.
2. Write `SELECT` queries with filters, sorting, pagination and aliases.
3. Practice `INNER JOIN`, `LEFT JOIN` and the many-to-many `trade_tags` table.
4. Aggregate PnL by account, symbol, month and trade side.
5. Compare execution plans before and after indexes with `EXPLAIN (ANALYZE, BUFFERS)`.
6. Add a partial index for open trades and verify that PostgreSQL uses it.
7. Practice transactions by inserting a trade and its tags atomically.
8. Reproduce a concurrent update and add optimistic concurrency handling.
9. Build a window-function report with running PnL and drawdown.
10. Create a materialized monthly-statistics view and refresh it.
11. Back up the database with `pg_dump`; restore it into a second database.
12. Add an integration test for one API endpoint against PostgreSQL.

Start with [`sql/exercises.sql`](../sql/exercises.sql). Do not put real credentials or production data in this repository.
