# ── Build Stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first for caching
COPY AsegyaPay.sln ./
COPY SharedKernel/AsegyaPay.SharedKernel/AsegyaPay.SharedKernel.csproj SharedKernel/AsegyaPay.SharedKernel/
COPY Services/PaymentService/AsegyaPay.PaymentService.Domain/AsegyaPay.PaymentService.Domain.csproj Services/PaymentService/AsegyaPay.PaymentService.Domain/
COPY Services/PaymentService/AsegyaPay.PaymentService.Application/AsegyaPay.PaymentService.Application.csproj Services/PaymentService/AsegyaPay.PaymentService.Application/
COPY Services/PaymentService/AsegyaPay.PaymentService.Infrastructure/AsegyaPay.PaymentService.Infrastructure.csproj Services/PaymentService/AsegyaPay.PaymentService.Infrastructure/
COPY Services/PaymentService/AsegyaPay.PaymentService.API/AsegyaPay.PaymentService.API.csproj Services/PaymentService/AsegyaPay.PaymentService.API/

# Restore NuGet packages
RUN dotnet restore Services/PaymentService/AsegyaPay.PaymentService.API/AsegyaPay.PaymentService.API.csproj

# Copy source files
COPY SharedKernel/ SharedKernel/
COPY Services/PaymentService/ Services/PaymentService/

# Publish
RUN dotnet publish Services/PaymentService/AsegyaPay.PaymentService.API/AsegyaPay.PaymentService.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Runtime Stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Security: create non-root user
RUN addgroup --system --gid 1000 appgroup && \
    adduser --system --uid 1000 --gid 1000 --no-create-home appuser

# Create directories with proper permissions
RUN mkdir -p /app/logs /tmp && \
    chown -R appuser:appgroup /app

# Copy published app
COPY --from=build --chown=appuser:appgroup /app/publish ./

USER appuser

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "AsegyaPay.PaymentService.API.dll"]
