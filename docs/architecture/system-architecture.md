# AsegyaPay System Architecture

## Overview

AsegyaPay is built on a cloud-native microservices architecture following Domain-Driven Design (DDD), CQRS, and Event Sourcing patterns.

## Architecture Principles

1. **Microservices-first**: Each bounded context is an independent service
2. **Event-driven**: Services communicate via Apache Kafka events
3. **API Gateway**: Single entry point with YARP reverse proxy
4. **Clean Architecture**: Each service follows the clean architecture layers
5. **CQRS**: Command Query Responsibility Segregation for optimal read/write
6. **Security by design**: Zero-trust networking, encryption everywhere

## System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              CDN (CloudFront/Azure CDN)                       │
└──────────────────────────────────┬──────────────────────────────────────────┘
                                   │
┌──────────────────────────────────▼──────────────────────────────────────────┐
│                         Load Balancer (L7)                                    │
│                    (WAF, DDoS Protection, TLS 1.3)                           │
└──────────┬───────────────────────┬───────────────────────┬──────────────────┘
           │                       │                       │
┌──────────▼─────────┐ ┌─────────▼──────────┐ ┌─────────▼──────────┐
│  Merchant Portal   │ │   Admin Portal     │ │   Checkout Page    │
│  (Next.js)         │ │   (Next.js)        │ │   (Next.js)        │
└──────────┬─────────┘ └─────────┬──────────┘ └─────────┬──────────┘
           │                      │                       │
┌──────────▼──────────────────────▼───────────────────────▼──────────────────┐
│                          API Gateway (YARP)                                  │
│              (Authentication, Rate Limiting, Request Routing)                 │
└──┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────┬────────────┘
   │    │    │    │    │    │    │    │    │    │    │    │    │
   ▼    ▼    ▼    ▼    ▼    ▼    ▼    ▼    ▼    ▼    ▼    ▼    ▼
┌─────┐┌─────┐┌─────┐┌─────┐┌─────┐┌─────┐┌─────┐┌─────┐┌─────┐
│Auth ││Pay- ││Merch││Settl││Refund││Subsc││Notif││Fraud││Payo-│
│Svc  ││ment ││ant  ││ment ││Svc   ││ript ││Svc  ││Svc  ││ut   │
│     ││Svc  ││Svc  ││Svc  ││      ││Svc  ││     ││     ││Svc  │
└──┬──┘└──┬──┘└──┬──┘└──┬──┘└──┬──┘└──┬──┘└──┬──┘└──┬──┘└──┬──┘
   │       │      │      │      │      │      │      │      │
   └───────┴──────┴──────┴──────┴──────┴──────┴──────┴──────┘
                                 │
┌────────────────────────────────▼────────────────────────────────────────────┐
│                         Event Bus (Apache Kafka)                              │
│                     (Event Sourcing, CQRS, Saga Orchestration)               │
└──────────┬─────────────────────┬────────────────────────┬──────────────────┘
           │                     │                        │
┌──────────▼─────────┐ ┌────────▼──────────┐ ┌──────────▼──────────┐
│   PostgreSQL 16    │ │   Redis 7         │ │   Elasticsearch 8   │
│   (Primary Data)   │ │   (Cache/Session) │ │   (Search/Analytics)│
└────────────────────┘ └───────────────────┘ └─────────────────────┘
```

## Service Responsibilities

| Service | Responsibility |
|---------|---------------|
| Auth Service | JWT tokens, OAuth2, MFA, API key management |
| Payment Service | Payment creation, authorization, capture, routing |
| Merchant Service | Merchant onboarding, KYC, configuration |
| Settlement Service | Daily settlements, reconciliation |
| Refund Service | Full/partial refunds, dispute management |
| Subscription Service | Recurring billing, plan management |
| Notification Service | Email, SMS, Push, Webhooks |
| Fraud Service | ML-based fraud detection, risk scoring |
| Payout Service | Bank transfers, UPI payouts, bulk disbursements |
| Analytics Service | Real-time analytics, reporting |
| Audit Service | Audit trail, compliance logging |
| AI Service | GenAI features, predictions, smart routing |

## Data Flow: Payment Processing

```
1. Client → API Gateway → Payment Service (Create Order)
2. Payment Service → Fraud Service (Risk Assessment)
3. If approved: Payment Service → Payment Gateway (Process)
4. Payment Gateway → Payment Service (Callback)
5. Payment Service → Kafka (PaymentCompleted event)
6. Settlement Service ← Kafka (Consume event)
7. Notification Service ← Kafka (Send webhook/email)
8. Analytics Service ← Kafka (Update metrics)
```

## Security Architecture

- **Network**: Zero-trust with mutual TLS between services
- **Authentication**: OAuth 2.1 + OpenID Connect via Auth Service
- **Authorization**: Policy-based access control (RBAC + ABAC)
- **Encryption**: AES-256 at rest, TLS 1.3 in transit
- **Secrets**: HashiCorp Vault / Azure Key Vault
- **Tokenization**: PCI-compliant card tokenization via HSM
- **Monitoring**: SIEM integration for security events

## Deployment Architecture

- **Container Orchestration**: Kubernetes (AKS/EKS)
- **Service Mesh**: Istio for mTLS, traffic management
- **CI/CD**: GitHub Actions + ArgoCD (GitOps)
- **Multi-Region**: Active-Active with global load balancing
- **Auto-scaling**: HPA based on CPU/memory/custom metrics
