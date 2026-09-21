-- 1. List the 20 newest trades with account names.

-- 2. Calculate realized PnL and fees by symbol.

-- 3. Calculate win rate by account. Avoid integer division.

-- 4. Use a window function to calculate cumulative realized PnL by close time.

-- 5. Find accounts that have no trades with a LEFT JOIN.

-- 6. Inspect the execution plan for open trades filtered by account and date.
-- EXPLAIN (ANALYZE, BUFFERS) ...

-- 7. Add a partial index that accelerates queries for open trades.

-- 8. In two psql sessions, reproduce READ COMMITTED behavior and a row lock.
