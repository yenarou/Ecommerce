FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
ENV DOTNET_SYSTEM_NET_DISABLEIPV6=1
COPY src/ECommerce.Api/ECommerce.Api.csproj src/ECommerce.Api/
COPY src/ECommerce.Application/ECommerce.Application.csproj src/ECommerce.Application/
COPY src/ECommerce.Domain/ECommerce.Domain.csproj src/ECommerce.Domain/
COPY src/ECommerce.Infrastructure/ECommerce.Infrastructure.csproj src/ECommerce.Infrastructure/
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore src/ECommerce.Api/ECommerce.Api.csproj
COPY . .
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish src/ECommerce.Api/ECommerce.Api.csproj -c Release -o /app/publish /p:UseAppHost=false --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ECommerce.Api.dll"]