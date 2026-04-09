FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /app

COPY "Netnol.Identity.Service.csproj" .
RUN dotnet restore

COPY . .

EXPOSE 8080

ENV ASPNETCORE_URLS="http://*:8080"
ENV DOTNET_ENVIRONMENT="Production"
ENV DATABASE_URL=""
ENV CACHE_URL=""

ENTRYPOINT ["dotnet", "run", "-c", "Release", "--non-interactive"]