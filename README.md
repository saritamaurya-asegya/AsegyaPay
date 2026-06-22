# AsegyaPay — Enterprise Payment Gateway Platform

<p align="center">
  <img src="https://img.shields.io/badge/version-1.0.0-blue.svg" alt="Version">
  <img src="https://img.shields.io/badge/.NET-9.0-purple.svg" alt=".NET 9">
  <img src="https://img.shields.io/badge/Next.js-14-black.svg" alt="Next.js 14">
  <img src="https://img.shields.io/badge/TypeScript-5.x-blue.svg" alt="TypeScript">
  <img src="https://img.shields.io/badge/license-MIT-green.svg" alt="License">
</p>

> An enterprise-grade, cloud-native payment gateway platform comparable to PayU, Razorpay, Stripe, and Adyen. Built with ASP.NET Core 9 microservices, Next.js 14, and designed for 100,000+ TPS at 99.99% uptime.

---

## 🏗️ Architecture Overview

AsegyaPay follows a **microservices architecture** with Clean Architecture (DDD + CQRS) per service:

```
┌─────────────────────────────────────────────────────────────────┐
│                        API Gateway (YARP)                        │
│              Rate Limiting · Auth · Load Balancing               │
└───────────────────────────┬─────────────────────────────────────┘
                            │
        ┌───────────────────┼───────────────────────┐
        │                   │                       │
   ┌────▼────┐        ┌─────▼─────┐          ┌─────▼──────┐
   │  Auth   │        │ Merchant  │          │  Payment   │
   │ Service │        │  Service  │          │  Service   │
   └─────────┘        └───────────┘          └────────────┘
        │                   │                       │
   ┌────▼────┐        ┌─────▼─────┐          ┌─────▼──────┐
   │Settlement│       │Subscription│          │   Fraud    │
   │ Service │        │  Service  │          │  Service   │
   └─────────┘        └───────────┘          └────────────┘
        │                   │                       │
   ┌────▼────┐        ┌─────▼─────┐          ┌─────▼──────┐
   │ Payout  │        │Notification│          │ Analytics  │
   │ Service │        │  Service  │          │  Service   │
   └─────────┘        └───────────┘          └────────────┘
        │
┌───────▼──────────────────────────────────────────────────────┐
│                     Message Bus (Kafka)                       │
└───────────────────────────────────────────────────────────────┘
        │
┌───────▼──────────────────────────────────────────────────────┐
│          Data Layer: PostgreSQL · Redis · Elasticsearch       │
└───────────────────────────────────────────────────────────────┘
```

---

## 📁 Repository Structure

```
AsegyaPay/
├── src/
│   ├── backend/
│   │   ├── SharedKernel/              # Shared domain primitives & utilities
│   │   ├── ApiGateway/                # YARP reverse proxy + rate limiting
│   │   └── Services/
│   │       ├── AuthService/           # OAuth 2.1, JWT, MFA, OpenID Connect
│   │       ├── MerchantService/       # Merchant onboarding, KYC, API keys
│   │       ├── PaymentService/        # Core payment processing engine
│   │       ├── SettlementService/     # Settlement engine & reconciliation
│   │       ├── RefundService/         # Refund & chargeback management
│   │       ├── SubscriptionService/   # Recurring billing & subscription plans
│   │       ├── FraudService/          # AI-powered fraud detection engine
│   │       ├── NotificationService/   # Email, SMS, webhook notifications
│   │       ├── PayoutService/         # Instant & bulk payouts
│   │       └── AnalyticsService/      # Real-time analytics & reporting
│   └── frontend/
│       ├── merchant-portal/           # Next.js 14 merchant dashboard
│       ├── admin-portal/              # Next.js 14 admin dashboard
│       └── checkout/                  # Embeddable checkout UI
├── infrastructure/
│   ├── docker/                        # Docker Compose for local dev
│   ├── kubernetes/                    # K8s manifests
│   └── terraform/                     # IaC for Azure/AWS
├── docs/
│   ├── architecture.md
│   ├── api/                           # OpenAPI specs
│   └── guides/                        # Deployment & integration guides
└── .github/workflows/                 # CI/CD pipelines
```

---

## 🚀 Quick Start

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [PostgreSQL 16](https://www.postgresql.org/)
- [Redis 7](https://redis.io/)

### Local Development (Docker Compose)

```bash
# Clone and navigate
git clone https://github.com/saritamaurya-asegya/AsegyaPay.git
cd AsegyaPay

# Start all infrastructure services
docker compose -f infrastructure/docker/docker-compose.yml up -d

# Start backend services
cd src/backend
dotnet restore AsegyaPay.sln
dotnet run --project ApiGateway/AsegyaPay.ApiGateway

# Start merchant portal
cd src/frontend/merchant-portal
npm install && npm run dev  # http://localhost:3000

# Start admin portal
cd src/frontend/admin-portal
npm install && npm run dev  # http://localhost:3001
```

### Environment Variables

Copy `.env.example` to `.env` and configure:

```env
# Database
POSTGRES_CONNECTION=Host=localhost;Database=asegyapay;Username=postgres;******
REDIS_CONNECTION=localhost:6379

# JWT
JWT_SECRET=your-256-bit-secret
JWT_ISSUER=https://auth.asegyapay.com
JWT_AUDIENCE=https://api.asegyapay.com

# Kafka
KAFKA_BOOTSTRAP_SERVERS=localhost:9092

# Encryption
ENCRYPTION_KEY=your-aes-256-key

# Payment Providers
STRIPE_SECRET_KEY=sk_test_...
RAZORPAY_KEY_ID=rzp_test_...
```

---

## 🔌 API Overview

### Base URL
```
https://api.asegyapay.com/v1
```

### Authentication
All APIs use ******** tokens. Obtain a token via `/auth/token`.

```http
Authorization: ******
```

### Core Payment APIs

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/payments` | Create a payment order |
| `GET` | `/payments/{id}` | Get payment details |
| `POST` | `/payments/{id}/capture` | Capture an authorized payment |
| `POST` | `/payments/{id}/refund` | Refund a payment |
| `GET` | `/payments/{id}/status` | Get payment status |
| `POST` | `/payments/verify` | Verify payment signature |

### Subscription APIs

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/subscriptions` | Create a subscription |
| `GET` | `/subscriptions/{id}` | Get subscription details |
| `PATCH` | `/subscriptions/{id}` | Update subscription |
| `DELETE` | `/subscriptions/{id}` | Cancel subscription |
| `POST` | `/plans` | Create a billing plan |

### Payout APIs

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/payouts` | Initiate a payout |
| `POST` | `/payouts/bulk` | Bulk payouts |
| `GET` | `/payouts/{id}` | Get payout status |

### Merchant APIs

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/merchants` | Register merchant |
| `GET` | `/merchants/me` | Get merchant profile |
| `POST` | `/merchants/kyc` | Submit KYC documents |
| `POST` | `/merchants/api-keys` | Generate API key |
| `POST` | `/merchants/webhooks` | Configure webhook |

---

## 🛡️ Security

AsegyaPay is designed with **security-first** principles:

- **PCI DSS Level 1** compliant architecture
- **AES-256** encryption for sensitive data at rest
- **TLS 1.3** for all data in transit
- **OAuth 2.1 + OpenID Connect** for authentication
- **JWT** with short expiry + refresh token rotation
- **MFA** support (TOTP, SMS, Email)
- **HSM** integration for key management
- **Card tokenization** — raw card data never touches application servers
- **Rate limiting** per API key and IP
- **DDoS protection** via cloud WAF
- **Audit logging** for all sensitive operations
- **Secrets management** via Azure Key Vault / AWS Secrets Manager

---

## 🎯 Payment Methods Supported

| Category | Methods |
|----------|---------|
| **Cards** | Visa, Mastercard, Amex, RuPay, Diners |
| **UPI** | UPI, UPI Intent, UPI QR, UPI AutoPay |
| **Wallets** | Paytm, PhonePe, Amazon Pay, Mobikwik |
| **Net Banking** | 50+ banks |
| **EMI** | Credit Card EMI, Debit Card EMI, Cardless EMI |
| **BNPL** | ZestMoney, Simpl, LazyPay |
| **International** | International cards, PayPal |
| **Digital** | Apple Pay, Google Pay |

---

## 📊 Microservices

### Payment Service
Core payment processing engine supporting:
- Multiple payment methods
- 3DS authentication
- Payment routing & cascading
- Retry logic with smart routing
- Real-time status updates via WebSocket

### Fraud Detection Service
AI-powered fraud engine:
- ML-based risk scoring (0-1000)
- Real-time rule engine (< 50ms evaluation)
- Device fingerprinting
- Velocity checks
- Geo-anomaly detection
- Card testing attack prevention

### Subscription Service
Recurring billing engine:
- Multiple billing cycles (daily/weekly/monthly/annual)
- Usage-based & metered billing
- Smart dunning management
- Proration on plan changes
- Trial period management
- Coupon & discount engine

### Settlement Service
Automated settlement engine:
- T+1, T+2 settlement cycles
- Multi-bank settlement support
- Automatic reconciliation
- Fee calculation engine
- Tax computation

---

## 🌐 Frontend Portals

### Merchant Portal (Next.js 14)
- Real-time dashboard with analytics
- Transaction management
- Settlement reports
- Refund management
- API key & webhook management
- Team management with RBAC
- KYC document upload

### Admin Portal (Next.js 14)
- Merchant management & approval
- Real-time transaction monitoring
- Fraud alert management
- Settlement management
- Compliance & audit reports
- Revenue analytics

---

## 🚢 Deployment

### Kubernetes (Production)

```bash
# Apply namespace and configs
kubectl apply -f infrastructure/kubernetes/namespace.yaml
kubectl apply -f infrastructure/kubernetes/configmaps/

# Deploy services
kubectl apply -f infrastructure/kubernetes/services/

# Apply ingress
kubectl apply -f infrastructure/kubernetes/ingress.yaml
```

### CI/CD Pipeline

- **Build**: Docker multi-stage builds
- **Test**: Unit + Integration tests on every PR
- **Security**: SAST, DAST, dependency scanning
- **Deploy**: ArgoCD GitOps to Kubernetes
- **Monitor**: Prometheus + Grafana dashboards

---

## 📈 Performance

| Metric | Target |
|--------|--------|
| Transaction Throughput | 100,000+ TPS |
| API Latency (p99) | < 100ms |
| Uptime SLA | 99.99% |
| Payment Success Rate | > 98% |
| Fraud Detection Latency | < 50ms |

---

## 📚 Documentation

- [System Architecture](docs/architecture.md)
- [API Reference](docs/api/openapi.yaml)
- [Database Schema](docs/architecture.md#database-schema)
- [Security Architecture](docs/guides/security.md)
- [Deployment Guide](docs/guides/deployment.md)
- [Production Roadmap](docs/guides/roadmap.md)

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/payment-method-xyz`
3. Commit changes: `git commit -m 'feat: add XYZ payment method'`
4. Push to branch: `git push origin feature/payment-method-xyz`
5. Open a Pull Request

### Commit Convention

We follow [Conventional Commits](https://www.conventionalcommits.org/):
- `feat:` — New feature
- `fix:` — Bug fix
- `docs:` — Documentation only
- `chore:` — Build/config changes
- `test:` — Test additions/changes

---

## 📄 License

MIT License — see [LICENSE](LICENSE) for details.

---

<p align="center">Built with ❤️ by the AsegyaPay Team | Inspired by Stripe, Razorpay, and PayU</p>
