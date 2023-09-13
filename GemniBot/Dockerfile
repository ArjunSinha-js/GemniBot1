FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["GemniBot/GemniBot.csproj", "GemniBot/"]
RUN dotnet restore "GemniBot/GemniBot.csproj"
COPY . .
WORKDIR "/src/GemniBot"
RUN dotnet build "GemniBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GemniBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GemniBot.dll"]
