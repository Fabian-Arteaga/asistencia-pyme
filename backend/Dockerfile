FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["src/AsistenciaPyme.WebApi/AsistenciaPyme.WebApi.csproj", "src/AsistenciaPyme.WebApi/"]
COPY ["src/AsistenciaPyme.Application/AsistenciaPyme.Application.csproj", "src/AsistenciaPyme.Application/"]
COPY ["src/AsistenciaPyme.Infrastructure/AsistenciaPyme.Infrastructure.csproj", "src/AsistenciaPyme.Infrastructure/"]
COPY ["src/AsistenciaPyme.Domain/AsistenciaPyme.Domain.csproj", "src/AsistenciaPyme.Domain/"]

RUN dotnet restore "src/AsistenciaPyme.WebApi/AsistenciaPyme.WebApi.csproj"

COPY . .

WORKDIR "/src/src/AsistenciaPyme.WebApi"

RUN dotnet publish "AsistenciaPyme.WebApi.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:10000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AsistenciaPyme.WebApi.dll"]