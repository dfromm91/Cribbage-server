FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Directory.Build.props Cribbage.sln ./
COPY src/Cribbage.Api/Cribbage.Api.csproj src/Cribbage.Api/
RUN dotnet restore src/Cribbage.Api/Cribbage.Api.csproj
COPY src/Cribbage.Api/ src/Cribbage.Api/
RUN dotnet publish src/Cribbage.Api/Cribbage.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER app
ENTRYPOINT ["dotnet", "Cribbage.Api.dll"]
