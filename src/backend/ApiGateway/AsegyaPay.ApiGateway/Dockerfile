# ── Build Stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY AsegyaPay.sln ./
COPY SharedKernel/AsegyaPay.SharedKernel/AsegyaPay.SharedKernel.csproj SharedKernel/AsegyaPay.SharedKernel/
COPY ApiGateway/AsegyaPay.ApiGateway/AsegyaPay.ApiGateway.csproj ApiGateway/AsegyaPay.ApiGateway/

RUN dotnet restore ApiGateway/AsegyaPay.ApiGateway/AsegyaPay.ApiGateway.csproj

COPY SharedKernel/ SharedKernel/
COPY ApiGateway/ ApiGateway/

RUN dotnet publish ApiGateway/AsegyaPay.ApiGateway/AsegyaPay.ApiGateway.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Runtime Stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN addgroup --system --gid 1000 appgroup && \
    adduser --system --uid 1000 --gid 1000 --no-create-home appuser

RUN mkdir -p /app/logs /tmp && chown -R appuser:appgroup /app

COPY --from=build --chown=appuser:appgroup /app/publish ./

USER appuser
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=15s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "AsegyaPay.ApiGateway.dll"]
