# Deployment Guide

## Prerequisites

- Kubernetes cluster (1.28+)
- kubectl configured
- Helm 3.x
- Docker registry access
- PostgreSQL 16
- Redis 7
- Apache Kafka

## Local Development

### Using Docker Compose

```bash
# Start all infrastructure services
docker-compose up -d

# Verify services are running
docker-compose ps

# View logs
docker-compose logs -f payment-service
```

### Running Individual Services

```bash
# Payment Service
cd src/Services/PaymentService/API
dotnet run

# Merchant Portal
cd src/Frontend/merchant-portal
npm install
npm run dev
```

## Staging Deployment

### Build Images

```bash
# Build all services
docker-compose build

# Or build individual service
docker build -f infrastructure/docker/Dockerfile.service \
  --build-arg SERVICE_NAME=PaymentService \
  -t asegyapay/payment-service:latest .
```

### Deploy to Kubernetes

```bash
# Create namespace
kubectl apply -f infrastructure/kubernetes/base.yaml

# Create secrets
kubectl create secret generic payment-service-secrets \
  --namespace asegyapay \
  --from-literal=db-connection-string="Host=postgres;Database=asegyapay_payments;..." \
  --from-literal=redis-connection-string="redis:6379,******"

# Deploy services
kubectl apply -f infrastructure/kubernetes/payment-service.yaml

# Verify deployment
kubectl get pods -n asegyapay
kubectl get svc -n asegyapay
```

## Production Deployment

### Pre-deployment Checklist

- [ ] All tests passing (unit, integration, e2e)
- [ ] Security scan completed (no critical vulnerabilities)
- [ ] Database migrations tested
- [ ] Load testing completed (target: 100k TPS)
- [ ] Rollback plan documented
- [ ] Monitoring alerts configured
- [ ] PCI DSS compliance verified
- [ ] API documentation updated

### Blue-Green Deployment

1. Deploy new version to green environment
2. Run smoke tests against green
3. Switch traffic from blue to green
4. Monitor for 15 minutes
5. If issues: rollback to blue
6. If stable: decommission blue

### Environment Variables

| Variable | Description | Required |
|----------|-------------|----------|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | Yes |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection | Yes |
| `ConnectionStrings__Redis` | Redis connection | Yes |
| `Kafka__BootstrapServers` | Kafka broker addresses | Yes |
| `Auth__Authority` | OAuth2 authority URL | Yes |
| `Auth__Audience` | JWT audience | Yes |

## Monitoring

- **Prometheus**: http://localhost:9090 (metrics)
- **Grafana**: http://localhost:3100 (dashboards)
- **Jaeger**: http://localhost:16686 (distributed tracing)
- **Kibana**: http://localhost:5601 (logs)

## Scaling

```bash
# Manual scaling
kubectl scale deployment payment-service --replicas=10 -n asegyapay

# HPA will auto-scale based on CPU/memory thresholds
kubectl get hpa -n asegyapay
```
