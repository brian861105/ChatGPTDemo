FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5269

ENV ASPNETCORE_URLS=http://+:5269

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["backend/src/AuthServer/AuthServer.csproj", "backend/src/AuthServer/"]
RUN dotnet restore "backend/src/AuthServer/AuthServer.csproj"
COPY . .
WORKDIR "/src/backend/src/AuthServer"
RUN dotnet build "AuthServer.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "AuthServer.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthServer.dll"]
