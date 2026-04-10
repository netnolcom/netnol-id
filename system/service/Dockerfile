FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /app

COPY "Netnol.Identity.Service.csproj" .
RUN dotnet restore

COPY . .

EXPOSE 8080

ENV HOST="http://*:8080"
ENV ENVIRONMENT="Production"
ENV DATABASE_URI=""
ENV CACHE_URI=""

ENTRYPOINT ["dotnet", "run", "-c", "Release", "--non-interactive"]