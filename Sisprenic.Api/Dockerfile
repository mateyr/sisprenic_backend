# Build context must be the solution root (sisprenic_backend/):
#   docker build -f Sisprenic.Api/Dockerfile .

# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Solution-level MSBuild files (TargetFramework + central package versions)
# must be present before restore, or every csproj fails to resolve.
COPY ["global.json", "Directory.Build.props", "Directory.Packages.props", "./"]
COPY ["Sisprenic.Api/Sisprenic.Api.csproj", "Sisprenic.Api/"]
COPY ["Sisprenic.Domain/Sisprenic.Domain.csproj", "Sisprenic.Domain/"]
COPY ["Sisprenic.Reports/Sisprenic.Reports.csproj", "Sisprenic.Reports/"]
RUN dotnet restore "Sisprenic.Api/Sisprenic.Api.csproj" -r linux-x64

COPY . .
RUN dotnet publish "Sisprenic.Api/Sisprenic.Api.csproj" \
    -c $BUILD_CONFIGURATION -r linux-x64 --self-contained false --no-restore \
    -o /app/publish /p:UseAppHost=false

# ---------- Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble AS final
WORKDIR /app

# Sisprenic.Reports launches Chromium in-process for PDF rendering.
# Install it with the Playwright driver bundled in the publish output, so the
# browser version always matches the Microsoft.Playwright NuGet version.
ENV PLAYWRIGHT_BROWSERS_PATH=/ms-playwright
COPY --from=build /app/publish .
RUN ./.playwright/node/linux-x64/node ./.playwright/package/cli.js install --with-deps chromium \
    && rm -rf /var/lib/apt/lists/* \
    && chmod -R a+rx /ms-playwright

USER $APP_UID
EXPOSE 8080

# Railway injects PORT; default to 8080 locally.
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} exec dotnet Sisprenic.Api.dll"]
