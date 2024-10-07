FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5264

ENV ASPNETCORE_URLS=http://+:5264

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/AuthServer/AuthServer.Api/AuthServer.Api.csproj", "src/AuthServer/AuthServer.Api/"]
RUN dotnet restore "src/AuthServer/AuthServer.Api/AuthServer.Api.csproj"
COPY . .
WORKDIR "/src/src/AuthServer/AuthServer.Api"
RUN dotnet build "AuthServer.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "AuthServer.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthServer.Api.dll"]
