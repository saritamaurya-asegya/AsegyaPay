# AsegyaPay

> Enterprise-Grade Payment Gateway Platform

[![Build](https://github.com/saritamaurya-asegya/AsegyaPay/actions/workflows/ci.yml/badge.svg)](https://github.com/saritamaurya-asegya/AsegyaPay/actions)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Overview

AsegyaPay is a next-generation, cloud-native payment gateway platform designed to compete with PayU, Razorpay, Stripe, and Adyen. It enables businesses of all sizes to accept, process, manage, and reconcile digital payments globally.

### Key Capabilities

- **100,000+ TPS** architecture with <100ms API latency
- **99.99% uptime** with active-active multi-region deployment
- **PCI DSS Level 1** ready security architecture
- **AI-powered** fraud detection and smart payment routing
- **Developer-first** with comprehensive SDKs and documentation

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        API Gateway                                │
│                   (Rate Limiting, Auth, Routing)                  │
└──────────┬──────────────────────────────────────────┬───────────┘
           │                                          │
┌──────────▼──────────┐                    ┌─────────▼──────────┐
│   Merchant Portal   │                    │   Admin Portal      │
│   (Next.js/React)   │                    │   (Next.js/React)   │
└─────────────────────┘                    └────────────────────┘
           │                                          │
┌──────────▼──────────────────────────────────────────▼───────────┐
│                     Microservices Layer                           │
├─────────────┬──────────────┬──────────────┬─────────────────────┤
│ Auth Service│Payment Service│Merchant Svc  │Settlement Service   │
├─────────────┼──────────────┼──────────────┼─────────────────────┤
│Refund Service│Subscription │Fraud Service │Notification Service │
├─────────────┼──────────────┼──────────────┼─────────────────────┤
│Payout Service│Analytics Svc│Reporting Svc │AI Service           │
└─────────────┴──────────────┴──────────────┴─────────────────────┘
           │                                          │
┌──────────▼──────────────────────────────────────────▼───────────┐
│                     Data Layer                                    │
├─────────────┬──────────────┬──────────────┬─────────────────────┤
│ PostgreSQL  │    Redis     │Elasticsearch │  Apache Kafka       │
└─────────────┴──────────────┴──────────────┴─────────────────────┘
```

## Technology Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 9, C#, Clean Architecture, CQRS, MediatR, DDD |
| Frontend | React, Next.js 14, TypeScript, Tailwind CSS |
| Mobile | Flutter |
| Database | PostgreSQL 16, Redis 7, Elasticsearch 8 |
| Messaging | Apache Kafka, RabbitMQ |
| Cloud | Kubernetes, Docker, Azure/AWS |
| Monitoring | Prometheus, Grafana, OpenTelemetry, Jaeger |
| CI/CD | GitHub Actions, ArgoCD |

## Project Structure

```
AsegyaPay/
├── src/
│   ├── ApiGateway/              # API Gateway (Ocelot/YARP)
│   ├── Services/                # Microservices
│   │   ├── AuthService/         # Authentication & Authorization
│   │   ├── PaymentService/      # Payment processing
│   │   ├── MerchantService/     # Merchant management
│   │   ├── SettlementService/   # Settlement engine
│   │   ├── RefundService/       # Refund processing
│   │   ├── SubscriptionService/ # Subscription billing
│   │   ├── NotificationService/ # Notifications (Email/SMS/Push)
│   │   ├── FraudService/        # AI fraud detection
│   │   ├── AnalyticsService/    # Analytics & metrics
│   │   ├── ReportingService/    # Report generation
│   │   ├── AuditService/        # Audit logging
│   │   ├── AIService/           # AI/ML features
│   │   └── PayoutService/       # Payouts & disbursements
│   ├── Shared/                  # Shared libraries
│   │   ├── AsegyaPay.Common/    # Common utilities
│   │   ├── AsegyaPay.Contracts/ # Shared contracts/DTOs
│   │   └── AsegyaPay.Security/  # Security utilities
│   └── Frontend/
│       ├── merchant-portal/     # Merchant dashboard (Next.js)
│       ├── admin-portal/        # Admin dashboard (Next.js)
│       └── checkout/            # Payment checkout UI
├── docs/                        # Documentation
│   ├── architecture/            # Architecture diagrams
│   ├── api-specs/               # OpenAPI specifications
│   ├── database/                # Database schemas
│   └── deployment/              # Deployment guides
├── infrastructure/              # IaC & DevOps
│   ├── docker/                  # Docker configurations
│   ├── kubernetes/              # K8s manifests
│   └── terraform/               # Terraform modules
├── tests/                       # Test suites
│   ├── Unit/                    # Unit tests
│   ├── Integration/             # Integration tests
│   └── E2E/                     # End-to-end tests
└── .github/workflows/           # CI/CD pipelines
```

## Supported Payment Methods

| Category | Methods |
|----------|---------|
| Cards | Credit Cards, Debit Cards, International Cards, Tokenized Cards |
| Digital Wallets | Apple Pay, Google Pay, PayTM, PhonePe |
| Bank Transfers | UPI, Net Banking, NEFT, IMPS |
| BNPL | Buy Now Pay Later integrations |
| EMI | Card EMI, No-cost EMI |
| QR | QR Code payments (Bharat QR, UPI QR) |

## Quick Start

### Prerequisites

- .NET 9 SDK
- Node.js 20+
- Docker & Docker Compose
- PostgreSQL 16
- Redis 7

### Development Setup

```bash
# Clone the repository
git clone https://github.com/saritamaurya-asegya/AsegyaPay.git
cd AsegyaPay

# Start infrastructure services
docker-compose up -d

# Run the API Gateway
cd src/ApiGateway
dotnet run

# Run the Merchant Portal
cd src/Frontend/merchant-portal
npm install && npm run dev
```

## API Overview

```bash
# Create a payment
POST /api/v1/payments
{
  "amount": 10000,
  "currency": "INR",
  "method": "card",
  "description": "Order #12345"
}

# Verify payment
GET /api/v1/payments/{payment_id}

# Create refund
POST /api/v1/refunds
{
  "payment_id": "pay_xxxxx",
  "amount": 5000
}
```

## Security

- PCI DSS Level 1 compliant architecture
- AES-256 encryption at rest
- TLS 1.3 in transit
- OAuth 2.1 + OpenID Connect
- Hardware Security Module (HSM) integration
- Real-time fraud detection with ML models

## Compliance

- PCI DSS
- GDPR
- ISO 27001
- SOC 2 Type II
- RBI Guidelines
- PSD2/SCA

## Contributing

Please read [CONTRIBUTING.md](docs/CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
