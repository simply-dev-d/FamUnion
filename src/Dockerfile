FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base

ENV ASPNETCORE_ENVIRONMENT=Docker

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY . ./
RUN dotnet restore "FamUnion.sln"
COPY . .
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish "FamUnion.Api/FamUnion.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FamUnion.Api.dll"]