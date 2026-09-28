FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Directory.Build.props Cribbage.sln ./
COPY src/Cribbage.Web/Cribbage.Web.csproj src/Cribbage.Web/
RUN dotnet restore src/Cribbage.Web/Cribbage.Web.csproj
COPY src/Cribbage.Web/ src/Cribbage.Web/
RUN dotnet publish src/Cribbage.Web/Cribbage.Web.csproj -c Release -o /app --no-restore

FROM nginx:1.27-alpine
COPY src/Cribbage.Web/nginx.conf.template /etc/nginx/templates/default.conf.template
COPY --from=build /app/wwwroot /usr/share/nginx/html
ENV API_SCHEME=http API_HOST=api API_PORT=8080 PORT=8080
EXPOSE 8080
