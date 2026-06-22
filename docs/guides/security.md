# Security Architecture

## Overview

AsegyaPay is designed to be **PCI DSS Level 1** compliant and follows security best practices across all layers.

---

## Security Layers

```
┌─────────────────────────────────────────────────────┐
│                  Edge Security                       │
│  WAF · DDoS Protection · TLS 1.3 · Rate Limiting   │
└────────────────────┬────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────┐
│              Authentication Layer                    │
│   OAuth 2.1 · JWT · MFA · Session Management       │
└────────────────────┬────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────┐
│              Authorization Layer                     │
│         RBAC · Resource-Based AuthZ               │
└────────────────────┬────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────┐
│              Application Security                    │
│   Input Validation · OWASP · Secrets Mgmt · Audit  │
└────────────────────┬────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────┐
│              Data Security                           │
│   AES-256 Encryption · Tokenization · HSM          │
└────────────────────┬────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────┐
│           Infrastructure Security                    │
│   K8s Security · Network Policies · SIEM          │
└─────────────────────────────────────────────────────┘
```

---

## Authentication

### JWT Configuration
- **Algorithm**: HS256 (HMAC-SHA256) for internal, RS256 for external
- **Access token TTL**: 1 hour
- **Refresh token TTL**: 30 days (rotated on each use)
- **Issuer**: `https://auth.asegyapay.com`
- **Claims**: `sub`, `email`, `role`, `merchant_id`, `jti`, `iat`, `exp`

### Password Policy
- Minimum 8 characters
- Must contain: uppercase, lowercase, number, special character
- Hashing: **PBKDF2-HMAC-SHA512** with 100,000 iterations
- Never stored in plaintext

### MFA Support
- **TOTP** (RFC 6238) — compatible with Google Authenticator, Authy
- **SMS OTP** — 6-digit, 5-minute TTL
- **Email OTP** — fallback option

### Account Lockout
- 5 failed attempts → 30-minute lockout
- Lockout state stored in database
- Automated alert to account owner

---

## API Key Security

API keys follow a secure design:
1. Raw key generated using `RandomNumberGenerator.GetBytes(32)`
2. Format: `rzp_{type}_{hex_random}`
3. **Only the prefix is stored in plaintext** for display
4. Full key is **SHA-256 hashed** before storage
5. Key shown only once at creation — cannot be recovered

```
Raw Key:   rzp_live_a3b4c5d6e7f8a3b4c5d6e7f8a3b4c5d6e7f8a3b4
Stored:    prefix: "rzp_live_a3b4", hash: "SHA256(raw_key)"
```

---

## Data Encryption

### At Rest
- Sensitive fields (PAN, bank accounts, MFA secrets) encrypted with **AES-256-GCM**
- Encryption keys managed via **Azure Key Vault** or **AWS KMS**
- Database-level encryption for backup files

### In Transit
- **TLS 1.3** mandatory for all connections
- Perfect Forward Secrecy (ECDHE key exchange)
- HSTS with preloading

### Card Data
- Raw card data **never touches application servers**
- Tokenization via PCI DSS-compliant vault
- Card BIN available for fraud checks only

---

## Infrastructure Security

### Kubernetes Hardening

```yaml
securityContext:
  runAsNonRoot: true      # Run as UID 1000
  runAsUser: 1000
  fsGroup: 2000
  allowPrivilegeEscalation: false
  readOnlyRootFilesystem: true  # Immutable container FS
  capabilities:
    drop: [ALL]           # Drop all Linux capabilities
```

### Network Policies
- Default **deny all** between namespaces
- Services only accept traffic from the API Gateway
- Database access restricted to application service accounts
- Egress restricted to known endpoints

### Secrets Management
- No secrets in environment variables or config files
- All secrets stored in **Azure Key Vault / AWS Secrets Manager**
- Secrets injected at runtime via Kubernetes CSI driver
- Secret rotation automated

---

## Webhook Security

All webhook events are signed with **HMAC-SHA256**:

```
Signature: HMAC-SHA256(webhook_secret, timestamp + "." + payload)
Header: AsegyaPay-Signature: t=1234567890,v1=signature_hex
```

Merchants should:
1. Verify the signature before processing
2. Check the timestamp (reject events > 5 minutes old)
3. Use HTTPS endpoints only

---

## Fraud Detection

The fraud detection service operates in **< 50ms** using:
- ML model (Gradient Boosting) for risk scoring (0-1000)
- Rule engine (velocity, geo, device checks)
- IP intelligence (VPN/proxy/TOR detection)

**Risk levels:**
- 0-300: Low → Allow
- 301-600: Medium → Allow (log)
- 601-800: High → 3DS challenge
- 801-1000: Critical → Block

---

## Audit Logging

All sensitive operations are logged:
- Authentication events (login, logout, MFA)
- Payment operations (create, capture, refund)
- Merchant configuration changes
- API key creation/revocation
- Admin actions

Audit logs are:
- Immutable (append-only)
- Stored in Elasticsearch
- Retained for 7 years (compliance)
- Exportable for regulators

---

## Compliance

| Standard | Status | Notes |
|----------|--------|-------|
| PCI DSS Level 1 | Architecture Ready | SAQ-D merchant requirements met |
| GDPR | Implemented | Data minimization, right to erasure |
| ISO 27001 | Roadmap Q2 2025 | ISMS documentation in progress |
| SOC 2 Type II | Roadmap Q3 2025 | Controls implemented |
| RBI Guidelines | Implemented | For India operations |
| FATF | Implemented | AML/KYC controls |

---

## Vulnerability Management

- **Weekly** dependency scanning (GitHub Dependabot)
- **Per-PR** CodeQL SAST scanning
- **Monthly** penetration testing (external firm)
- **Quarterly** vulnerability assessments
- Bug bounty program at https://hackerone.com/asegyapay

---

## Incident Response

1. **Detection**: SIEM alert or automated monitoring
2. **Triage**: On-call engineer assesses severity (P1-P4)
3. **Contain**: Isolate affected systems, revoke compromised credentials
4. **Eradicate**: Remove threat, patch vulnerability
5. **Recover**: Restore service with monitoring
6. **Lessons learned**: Post-mortem within 48h, update playbooks

**P1 (Critical) SLA**: Response < 15min, Resolution < 4h
