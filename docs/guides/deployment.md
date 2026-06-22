# Deployment Guide

## Prerequisites

- [kubectl](https://kubernetes.io/docs/tasks/tools/) ≥ 1.28
- [Helm](https://helm.sh/) ≥ 3.14
- [Docker](https://docs.docker.com/get-docker/) ≥ 24
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org/)

---

## Local Development

### 1. Start Infrastructure

```bash
# Clone the repository
git clone https://github.com/saritamaurya-asegya/AsegyaPay.git
cd AsegyaPay

# Start all infrastructure services
docker compose -f infrastructure/docker/docker-compose.yml up -d

# Verify services are running
docker compose -f infrastructure/docker/docker-compose.yml ps
```

This starts:
- PostgreSQL (port 5432)
- Redis (port 6379)
- Kafka + Zookeeper (port 9092)
- Kafka UI (port 8090)
- Elasticsearch (port 9200)
- Prometheus (port 9090)
- Grafana (port 3333)
- Jaeger (port 16686)
- pgAdmin (port 5050)
- MinIO (port 9000)

### 2. Configure Environment

```bash
# Copy example env files
cp .env.example .env

# Update with your values
# POSTGRES_CONNECTION, JWT_SECRET, etc.
```

### 3. Run Database Migrations

```bash
cd src/backend

# Auth Service
dotnet ef database update \
  --project Services/AuthService/AsegyaPay.AuthService.Infrastructure \
  --startup-project Services/AuthService/AsegyaPay.AuthService.API

# Payment Service
dotnet ef database update \
  --project Services/PaymentService/AsegyaPay.PaymentService.Infrastructure \
  --startup-project Services/PaymentService/AsegyaPay.PaymentService.API

# Merchant Service
dotnet ef database update \
  --project Services/MerchantService/AsegyaPay.MerchantService.Infrastructure \
  --startup-project Services/MerchantService/AsegyaPay.MerchantService.API
```

### 4. Start Backend Services

```bash
cd src/backend

# Option A: Start all services via terminal multiplexer (tmux/screen)
dotnet run --project Services/AuthService/AsegyaPay.AuthService.API &
dotnet run --project Services/PaymentService/AsegyaPay.PaymentService.API &
dotnet run --project Services/MerchantService/AsegyaPay.MerchantService.API &
dotnet run --project ApiGateway/AsegyaPay.ApiGateway &

# Option B: Use VS Code with the launch.json configuration
```

### 5. Start Frontend

```bash
# Merchant Portal (http://localhost:3000)
cd src/frontend/merchant-portal
npm install
npm run dev

# Admin Portal (http://localhost:3001)
cd src/frontend/admin-portal
npm install
npm run dev
```

---

## Production Deployment (Kubernetes)

### 1. Create Secrets

```bash
# Create namespace
kubectl apply -f infrastructure/kubernetes/namespace.yaml

# Create secrets (NEVER commit real secrets to git)
kubectl create secret generic payment-service-secrets \
  --from-literal=db-connection-string="Host=postgres;Database=asegyapay_payments;Username=payment_service;******" \
  --from-literal=redis-connection-string="${REDIS_URL}" \
  -n asegyapay

kubectl create secret generic jwt-secrets \
  --from-literal=jwt-secret="${JWT_SECRET}" \
  -n asegyapay
```

### 2. Create ConfigMaps

```bash
kubectl apply -f infrastructure/kubernetes/configmaps/ -n asegyapay
```

### 3. Deploy Services

```bash
# Deploy all services
kubectl apply -f infrastructure/kubernetes/services/ -n asegyapay

# Apply ingress
kubectl apply -f infrastructure/kubernetes/ingress.yaml -n asegyapay

# Check rollout status
kubectl rollout status deployment/payment-service -n asegyapay
kubectl rollout status deployment/auth-service -n asegyapay
kubectl rollout status deployment/merchant-service -n asegyapay
```

### 4. Verify Deployment

```bash
# Check all pods are running
kubectl get pods -n asegyapay

# Check services
kubectl get services -n asegyapay

# Check ingress
kubectl get ingress -n asegyapay

# Test health endpoints
curl https://api.asegyapay.com/health
```

---

## Environment Variables

### Backend Services

| Variable | Description | Required |
|----------|-------------|----------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | ✅ |
| `Redis__ConnectionString` | Redis connection string | ✅ |
| `Kafka__BootstrapServers` | Kafka bootstrap servers | ✅ |
| `Jwt__Secret` | JWT signing secret (min 256-bit) | ✅ |
| `Jwt__Issuer` | JWT issuer URL | ✅ |
| `Jwt__Audience` | JWT audience URL | ✅ |
| `Auth__Authority` | Auth service URL | ✅ |
| `Jaeger__Host` | Jaeger agent host | ❌ |
| `ASPNETCORE_ENVIRONMENT` | Environment name | ✅ |

### Frontend

| Variable | Description | Required |
|----------|-------------|----------|
| `NEXT_PUBLIC_API_URL` | API Gateway URL | ✅ |
| `NEXTAUTH_URL` | NextAuth callback URL | ✅ |
| `NEXTAUTH_SECRET` | NextAuth secret | ✅ |

---

## Docker Images

Build production Docker images:

```bash
# Build payment service
docker build \
  -t ghcr.io/asegyapay/payment-service:latest \
  src/backend/Services/PaymentService/AsegyaPay.PaymentService.API

# Build all services
for svc in AuthService PaymentService MerchantService; do
  docker build \
    -t "ghcr.io/asegyapay/${svc,,}:latest" \
    "src/backend/Services/$svc/AsegyaPay.${svc}.API"
done
```

---

## Monitoring Setup

### Grafana Dashboards

1. Navigate to http://localhost:3333 (default: admin/asegyapay_grafana_pwd)
2. Import dashboards from `infrastructure/docker/grafana/provisioning/dashboards/`

Key dashboards:
- **Payment Overview** — TPS, latency, success rate
- **Service Health** — Pod status, resource usage
- **Fraud Dashboard** — Risk scores, blocked transactions
- **Settlement Dashboard** — Settlement status and volumes

### Jaeger Tracing

1. Navigate to http://localhost:16686
2. Select service from dropdown
3. Search traces by operation, duration, or tags

---

## Scaling

```bash
# Scale payment service to 10 replicas
kubectl scale deployment payment-service --replicas=10 -n asegyapay

# Or let HPA handle it automatically (configured in Kubernetes manifests)
# HPA scales based on CPU > 70% or Memory > 80%
```

---

## Troubleshooting

```bash
# View service logs
kubectl logs -f deployment/payment-service -n asegyapay

# Execute into a pod
kubectl exec -it deployment/payment-service -n asegyapay -- /bin/sh

# Check events
kubectl describe pod <pod-name> -n asegyapay

# Port forward for debugging
kubectl port-forward service/payment-service 8080:8080 -n asegyapay
```
