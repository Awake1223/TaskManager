FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Отдельно копируем csproj, чтобы restore кэшировался Docker'ом
COPY TaskManager.Domain/TaskManager.Domain.csproj TaskManager.Domain/
COPY TaskManager.Application/TaskManager.Application.csproj TaskManager.Application/
COPY TaskManager.Infrastructure/TaskManager.Infrastructure.csproj TaskManager.Infrastructure/
COPY TaskManager.API/TaskManager.API.csproj TaskManager.API/

RUN dotnet restore TaskManager.API/TaskManager.API.csproj

# Копируем остальной код и публикуем
COPY . .
WORKDIR /src/TaskManager.API
RUN dotnet publish TaskManager.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskManager.API.dll"]