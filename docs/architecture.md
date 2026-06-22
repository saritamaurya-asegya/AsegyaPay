# AsegyaPay System Architecture

## Overview

AsegyaPay is built on a **microservices architecture** following **Domain-Driven Design (DDD)**, **Clean Architecture**, and **CQRS** patterns.

---

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           Client Applications                                │
│  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────────────┐  │
│  │  Merchant Portal │  │   Admin Portal   │  │    Mobile / 3rd Party    │  │
│  │  (Next.js 14)    │  │  (Next.js 14)    │  │  SDK / REST / GraphQL    │  │
│  └────────┬─────────┘  └────────┬─────────┘  └───────────┬──────────────┘  │
└───────────┼──────────────────────┼────────────────────────┼─────────────────┘
            │                      │                        │
            └──────────────────────┴────────────────────────┘
                                   │  HTTPS / TLS 1.3
                    ┌──────────────▼──────────────┐
                    │         API Gateway          │
                    │    (YARP Reverse Proxy)       │
                    │  Rate Limiting · Auth · WAF  │
                    └──────────────┬───────────────┘
                                   │  Internal Network
        ┌──────────────────────────┼──────────────────────────┐
        │                          │                          │
  ┌─────▼──────┐           ┌───────▼──────┐           ┌──────▼─────┐
  │    Auth    │           │   Merchant   │           │  Payment   │
  │  Service   │           │   Service    │           │  Service   │
  └────────────┘           └──────────────┘           └────────────┘
  ┌────────────┐           ┌──────────────┐           ┌────────────┐
  │ Settlement │           │ Subscription │           │   Fraud    │
  │  Service   │           │   Service    │           │  Service   │
  └────────────┘           └──────────────┘           └────────────┘
  ┌────────────┐           ┌──────────────┐           ┌────────────┐
  │   Payout   │           │ Notification │           │ Analytics  │
  │  Service   │           │   Service    │           │  Service   │
  └────────────┘           └──────────────┘           └────────────┘
        │                          │                          │
        └──────────────────────────┼──────────────────────────┘
                                   │
            ┌──────────────────────▼──────────────────────┐
            │            Apache Kafka (Event Bus)          │
            │    payment.captured · fraud.detected · ...   │
            └──────────────┬───────────────────────────────┘
                           │
        ┌──────────────────┼──────────────────┐
        │                  │                  │
  ┌─────▼──────┐   ┌───────▼──────┐   ┌──────▼──────┐
  │ PostgreSQL │   │    Redis     │   │Elasticsearch│
  │  (per svc) │   │  (cache/lock)│   │  (search)   │
  └────────────┘   └──────────────┘   └─────────────┘
```

---

## Service Responsibilities

### API Gateway
- **Technology**: ASP.NET Core 9 + YARP
- **Responsibilities**:
  - Reverse proxy and load balancing
  - JWT token validation
  - Rate limiting (per IP and API key)
  - Request/response logging
  - WAF integration

### Auth Service
- **Technology**: ASP.NET Core 9
- **Responsibilities**:
  - User registration and authentication
  - JWT access token issuance (1h TTL)
  - Refresh token management (30d TTL, rotated)
  - MFA (TOTP + SMS)
  - Password hashing (PBKDF2-HMAC-SHA512, 100K iterations)
  - OAuth 2.1 / OpenID Connect

### Payment Service
- **Technology**: ASP.NET Core 9 + PostgreSQL
- **Responsibilities**:
  - Payment order creation
  - Payment routing (smart routing to best gateway)
  - 3DS authentication orchestration
  - Payment status management
  - Refund processing
  - Webhook notifications
  - Idempotency key enforcement

### Merchant Service
- **Technology**: ASP.NET Core 9 + PostgreSQL
- **Responsibilities**:
  - Merchant registration and onboarding
  - KYC document collection and verification
  - API key management (SHA-256 hashed)
  - Webhook configuration
  - Team and role management

### Settlement Service
- **Technology**: ASP.NET Core 9 + PostgreSQL
- **Responsibilities**:
  - T+1/T+2 settlement cycle management
  - Fee calculation (MDR, GST)
  - Bank transfer initiation
  - Settlement reconciliation
  - UTR number tracking

### Fraud Detection Service
- **Technology**: ASP.NET Core 9 + ML.NET + Redis
- **Responsibilities**:
  - Real-time risk scoring (< 50ms target)
  - ML model inference
  - Rule engine evaluation (velocity, geo, device)
  - IP intelligence (VPN/proxy/TOR detection)
  - Device fingerprinting
  - Chargeback risk assessment

### Subscription Service
- **Technology**: ASP.NET Core 9 + PostgreSQL
- **Responsibilities**:
  - Billing plan management
  - Subscription lifecycle (create, pause, cancel)
  - Recurring charge orchestration
  - Smart dunning management
  - Trial period handling
  - Proration on upgrades/downgrades

### Payout Service
- **Technology**: ASP.NET Core 9 + PostgreSQL
- **Responsibilities**:
  - Instant payouts (UPI, bank transfer)
  - Bulk payout processing
  - Scheduled payouts
  - Payout status tracking
  - UTR reconciliation

### Notification Service
- **Technology**: ASP.NET Core 9 + Kafka consumer
- **Responsibilities**:
  - Webhook delivery with retry logic (exponential backoff)
  - Email notifications (transactional)
  - SMS notifications
  - Push notifications
  - Notification template management

### Analytics Service
- **Technology**: ASP.NET Core 9 + Elasticsearch
- **Responsibilities**:
  - Real-time transaction analytics
  - Revenue reporting
  - Merchant dashboard data
  - Settlement analytics
  - Fraud analytics
  - Export generation (CSV, PDF)

---

## Database Schema

### Payment Service (`asegyapay_payments`)

```sql
-- Payments table
CREATE TABLE payments.payments (
    id              UUID PRIMARY KEY,
    order_id        VARCHAR(64) NOT NULL,
    merchant_id     VARCHAR(64) NOT NULL,
    customer_id     VARCHAR(64),
    amount          DECIMAL(18,2) NOT NULL,
    currency        CHAR(3) NOT NULL,
    status          VARCHAR(32) NOT NULL,
    method          VARCHAR(32) NOT NULL,
    gateway         VARCHAR(32) NOT NULL DEFAULT 'Internal',
    gateway_tx_id   VARCHAR(128),
    gateway_order_id VARCHAR(128),
    description     VARCHAR(255) NOT NULL,
    failure_reason  VARCHAR(512),
    callback_url    VARCHAR(2048),
    redirect_url    VARCHAR(2048),
    metadata        JSONB NOT NULL DEFAULT '{}',
    fraud_score     INTEGER NOT NULL DEFAULT 0,
    captured_amount DECIMAL(18,2),
    captured_currency CHAR(3),
    refunded_amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    refunded_currency CHAR(3),
    created_at      TIMESTAMPTZ NOT NULL,
    updated_at      TIMESTAMPTZ NOT NULL,
    authorized_at   TIMESTAMPTZ,
    captured_at     TIMESTAMPTZ,
    expires_at      TIMESTAMPTZ,
    UNIQUE (order_id, merchant_id)
);

CREATE INDEX ix_payments_merchant_id ON payments.payments (merchant_id);
CREATE INDEX ix_payments_merchant_status ON payments.payments (merchant_id, status);
CREATE INDEX ix_payments_created_at ON payments.payments (created_at DESC);

-- Refunds table
CREATE TABLE payments.refunds (
    id              UUID PRIMARY KEY,
    payment_id      UUID NOT NULL REFERENCES payments.payments(id),
    merchant_id     VARCHAR(64) NOT NULL,
    amount          DECIMAL(18,2) NOT NULL,
    currency        CHAR(3) NOT NULL,
    reason          VARCHAR(255) NOT NULL,
    status          VARCHAR(32) NOT NULL,
    initiated_by    VARCHAR(128) NOT NULL,
    gateway_refund_id VARCHAR(128),
    failure_reason  VARCHAR(512),
    created_at      TIMESTAMPTZ NOT NULL,
    updated_at      TIMESTAMPTZ NOT NULL,
    processed_at    TIMESTAMPTZ
);
```

### Merchant Service (`asegyapay_merchants`)

```sql
CREATE TABLE merchants.merchants (
    id                    UUID PRIMARY KEY,
    business_name         VARCHAR(256) NOT NULL,
    business_email        VARCHAR(256) NOT NULL UNIQUE,
    business_phone        VARCHAR(20),
    business_type         VARCHAR(64) NOT NULL,
    website               VARCHAR(2048),
    description           TEXT,
    status                VARCHAR(32) NOT NULL DEFAULT 'Pending',
    kyc_status            VARCHAR(32) NOT NULL DEFAULT 'NotSubmitted',
    pan_number            VARCHAR(10),
    gst_number            VARCHAR(15),
    settlement_bank_account JSONB,
    country               CHAR(2) NOT NULL DEFAULT 'IN',
    mdr_rate              DECIMAL(5,4) NOT NULL DEFAULT 0.0200,
    settlement_cycle_days SMALLINT NOT NULL DEFAULT 2,
    created_at            TIMESTAMPTZ NOT NULL,
    updated_at            TIMESTAMPTZ NOT NULL,
    approved_at           TIMESTAMPTZ
);

CREATE TABLE merchants.api_keys (
    id          UUID PRIMARY KEY,
    merchant_id UUID NOT NULL REFERENCES merchants.merchants(id),
    name        VARCHAR(128) NOT NULL,
    type        VARCHAR(10) NOT NULL,
    prefix      VARCHAR(20) NOT NULL,
    key_hash    VARCHAR(64) NOT NULL UNIQUE,
    is_revoked  BOOLEAN NOT NULL DEFAULT FALSE,
    created_by  VARCHAR(128) NOT NULL,
    created_at  TIMESTAMPTZ NOT NULL,
    revoked_at  TIMESTAMPTZ,
    last_used_at TIMESTAMPTZ
);
```

### Auth Service (`asegyapay_auth`)

```sql
CREATE TABLE auth.users (
    id                    UUID PRIMARY KEY,
    email                 VARCHAR(256) NOT NULL UNIQUE,
    password_hash         VARCHAR(512) NOT NULL,
    first_name            VARCHAR(64) NOT NULL,
    last_name             VARCHAR(64) NOT NULL,
    phone_number          VARCHAR(20),
    role                  VARCHAR(32) NOT NULL,
    merchant_id           VARCHAR(64),
    is_email_verified     BOOLEAN NOT NULL DEFAULT FALSE,
    is_phone_verified     BOOLEAN NOT NULL DEFAULT FALSE,
    is_mfa_enabled        BOOLEAN NOT NULL DEFAULT FALSE,
    mfa_secret            VARCHAR(64),
    is_active             BOOLEAN NOT NULL DEFAULT TRUE,
    is_locked             BOOLEAN NOT NULL DEFAULT FALSE,
    failed_login_attempts INTEGER NOT NULL DEFAULT 0,
    locked_until          TIMESTAMPTZ,
    created_at            TIMESTAMPTZ NOT NULL,
    updated_at            TIMESTAMPTZ NOT NULL,
    last_login_at         TIMESTAMPTZ
);

CREATE TABLE auth.refresh_tokens (
    id          UUID PRIMARY KEY,
    user_id     UUID NOT NULL REFERENCES auth.users(id),
    token       VARCHAR(256) NOT NULL UNIQUE,
    expires_at  TIMESTAMPTZ NOT NULL,
    is_revoked  BOOLEAN NOT NULL DEFAULT FALSE,
    created_at  TIMESTAMPTZ NOT NULL
);

CREATE INDEX ix_refresh_tokens_token ON auth.refresh_tokens(token);
CREATE INDEX ix_refresh_tokens_user_id ON auth.refresh_tokens(user_id);
```

---

## Event-Driven Communication

Services communicate asynchronously via **Apache Kafka** topics:

| Topic | Producer | Consumers |
|-------|----------|-----------|
| `payment.created` | Payment Service | Fraud Service, Analytics Service |
| `payment.captured` | Payment Service | Settlement Service, Notification Service, Analytics Service |
| `payment.failed` | Payment Service | Notification Service, Analytics Service |
| `payment.refunded` | Payment Service | Settlement Service, Notification Service |
| `fraud.high-risk` | Fraud Service | Payment Service (block), Notification Service |
| `merchant.approved` | Merchant Service | Notification Service, Auth Service |
| `subscription.renewed` | Subscription Service | Payment Service, Notification Service |
| `payout.initiated` | Payout Service | Notification Service |
| `payout.completed` | Payout Service | Settlement Service, Notification Service |

---

## Security Architecture

### Authentication & Authorization
- **OAuth 2.1** for third-party integrations
- **JWT ****** (HS256/RS256) with 1-hour expiry
- **Refresh token rotation** — each refresh issues a new token pair
- **MFA** — TOTP (RFC 6238) + SMS OTP
- **RBAC** — roles: MerchantOwner, MerchantAdmin, MerchantStaff, SuperAdmin

### Data Encryption
- **AES-256-GCM** for sensitive data at rest (PAN, bank accounts)
- **TLS 1.3** mandatory for all connections
- **HSM** for production key management
- **Card tokenization** — raw card data via secure vault only

### API Security
- Rate limiting: 100 req/min default, configurable per merchant
- Idempotency keys for payment creation (Idempotency-Key header)
- Webhook signature verification (HMAC-SHA256)
- DDoS protection via cloud WAF

### Infrastructure Security
- All K8s pods run as non-root (UID 1000)
- Read-only root filesystem in containers
- Network policies restrict pod-to-pod communication
- Secrets stored in Azure Key Vault / AWS Secrets Manager

---

## Scalability & Performance

### Horizontal Scaling
- All services are **stateless** — scale horizontally with HPA
- **Redis** for session and distributed locking
- **Database connection pooling** via PgBouncer

### Caching Strategy
- L1: In-memory (IMemoryCache) for hot data (<1ms)
- L2: Redis distributed cache (<5ms)
- L3: Database with read replicas

### Performance Targets
| Metric | Target |
|--------|--------|
| Payment API latency (p99) | < 100ms |
| Fraud check latency (p99) | < 50ms |
| Throughput | 100,000+ TPS (clustered) |
| Uptime SLA | 99.99% |
| Cache hit rate | > 80% |

---

## Deployment Architecture

### Kubernetes Setup
- **Multi-AZ** deployment across 3 availability zones
- **Active-active** configuration for zero downtime
- **HPA** auto-scales services based on CPU/memory
- **PDB** ensures minimum 2 replicas always available

### CI/CD Pipeline
1. `git push` → GitHub Actions triggered
2. Build & unit test
3. Security scan (CodeQL + dependency audit)
4. Docker image built and pushed
5. ArgoCD deploys to staging (GitOps)
6. Smoke tests on staging
7. Manual approval gate
8. ArgoCD deploys to production (rolling update)

---

## Monitoring & Observability

| Layer | Tool |
|-------|------|
| **Metrics** | Prometheus + Grafana |
| **Tracing** | OpenTelemetry + Jaeger |
| **Logging** | Serilog → Elasticsearch → Kibana |
| **Alerting** | Grafana Alertmanager + PagerDuty |
| **SLA monitoring** | Prometheus + custom dashboards |
