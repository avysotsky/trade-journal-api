FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY TradeJournal.slnx ./
COPY src/TradeJournal.Api/TradeJournal.Api.csproj src/TradeJournal.Api/
RUN dotnet restore src/TradeJournal.Api/TradeJournal.Api.csproj
COPY src/TradeJournal.Api/ src/TradeJournal.Api/
RUN dotnet publish src/TradeJournal.Api/TradeJournal.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TradeJournal.Api.dll"]
